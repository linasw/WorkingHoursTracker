using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    public class WorkingHoursViewModel : Screen
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private WHTDbContext _dbContext;
        private BindableCollection<EmployeeModel> _employees;
        private BindableCollection<HoursModel> _hours;
        private SnackbarMessageQueue _messageQueue;
        private string _normalHoursSum;
        private string _overtimeHoursSum;
        private DateTime _selectedDate;
        private string _selectedDateHoursSum;
        private EmployeeModel _selectedEmployee;
        private DateTime? _timeFrom;
        private DispatcherTimer _timer;
        private DateTime? _timeTo;

        public WorkingHoursViewModel()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(timer_tick);
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Start();
            _selectedDate = DateTime.Now;
            Employees = new BindableCollection<EmployeeModel>();
            _selectedEmployee = Employees.First();
            Hours = new BindableCollection<HoursModel>();
            Hours.Refresh();
            MessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(5000));
        }

        public bool CanTimeUpdate
        {
            get
            {
                var toReturn = (TimeFrom != null && TimeTo != null && SelectedDate != null && SelectedEmployee != null && TimeFrom.Value.Ticks < TimeTo.Value.Ticks);
                return toReturn;
            }
        }

        public BindableCollection<EmployeeModel> Employees
        {
            get
            {
                using (_dbContext = new WHTDbContext())
                {
                    var tempEmployees = _dbContext.Employees;
                    _employees = new BindableCollection<EmployeeModel>(tempEmployees);
                }
                return _employees;
            }
            set
            {
                _employees = value;
                NotifyOfPropertyChange(() => Employees);
            }
        }

        public BindableCollection<HoursModel> Hours
        {
            get
            {
                using (_dbContext = new WHTDbContext())
                {
                    var tempHours = _dbContext.Hours;
                    _hours = new BindableCollection<HoursModel>(tempHours);
                    var tempTime = _hours.Where(x => x.WorkingDate.Date.Equals(SelectedDate.Date) && x.EmployeeId.Equals(SelectedEmployee.Id));

                    if (tempTime.Any())
                    {
                        TimeFrom = new DateTime(tempTime.Select(x => x.From).ToList().First().Ticks);
                        TimeTo = new DateTime(tempTime.Select(x => x.To).ToList().First().Ticks);
                    }
                    else
                    {
                        TimeFrom = null;
                        TimeTo = null;
                    }
                }
                return _hours;
            }
            set
            {
                _hours = value;
                NotifyOfPropertyChange(() => Hours);
            }
        }

        public SnackbarMessageQueue MessageQueue
        {
            get
            {
                return _messageQueue;
            }
            set
            {
                _messageQueue = value;
                NotifyOfPropertyChange(() => MessageQueue);
            }
        }
        public string NormalHoursSum
        {
            get
            {
                var daysOfSelectedMonth = Hours.Where(x => x.WorkingDate.Month.Equals(SelectedDate.Month) && x.EmployeeId.Equals(SelectedEmployee.Id));

                if (daysOfSelectedMonth.Any())
                {
                    var listOfDays = daysOfSelectedMonth.ToList();
                    decimal hoursSum = 0;
                    foreach (var day in listOfDays)
                    {
                        hoursSum += day.Normal;
                    }

                    return _normalHoursSum = hoursSum.ToString("N2");
                }
                else
                {
                    return "0";
                }
            }
            set
            {
                _normalHoursSum = value;
                NotifyOfPropertyChange(() => NormalHoursSum);
            }
        }
        public string OvertimeHoursSum
        {
            get
            {
                var daysOfSelectedMonth = Hours.Where(x => x.WorkingDate.Month.Equals(SelectedDate.Month) && x.EmployeeId.Equals(SelectedEmployee.Id));

                if (daysOfSelectedMonth.Any())
                {
                    var listOfDays = daysOfSelectedMonth.ToList();
                    decimal hoursSum = 0;
                    foreach (var day in listOfDays)
                    {
                        hoursSum += day.Overtime;
                    }

                    return _overtimeHoursSum = hoursSum.ToString("N2");
                }
                else
                {
                    return "0";
                }
            }
            set
            {
                _overtimeHoursSum = value;
                NotifyOfPropertyChange(() => OvertimeHoursSum);
            }
        }
        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;
                Hours.Refresh();
                NotifyOfPropertyChange(() => SelectedDate);
                NotifyOfPropertyChange(() => SelectedDateHoursSum);
                NotifyOfPropertyChange(() => NormalHoursSum);
                NotifyOfPropertyChange(() => OvertimeHoursSum);
                NotifyOfPropertyChange(() => CanTimeUpdate);
            }
        }
        public string SelectedDateHoursSum
        {
            get
            {
                if (TimeFrom != null && TimeTo != null)
                {
                    TimeSpan interval = (TimeSpan)(TimeTo - TimeFrom);
                    _selectedDateHoursSum = interval.TotalHours.ToString("N2");
                    return _selectedDateHoursSum;
                }
                else
                {
                    return "0";
                }
            }
        }
        public EmployeeModel SelectedEmployee
        {
            get
            {
                return _selectedEmployee;
            }
            set
            {
                _selectedEmployee = value;
                NotifyOfPropertyChange(() => SelectedEmployee);
                Hours.Refresh();
                NotifyOfPropertyChange(() => SelectedDateHoursSum);
                NotifyOfPropertyChange(() => NormalHoursSum);
                NotifyOfPropertyChange(() => OvertimeHoursSum);
                NotifyOfPropertyChange(() => CanTimeUpdate);
            }
        }
        public String Time
        {
            get
            {
                return DateTime.Now.ToLongDateString() + ", " + DateTime.Now.ToLongTimeString();
            }
        }
        public DateTime? TimeFrom
        {
            get
            {
                return _timeFrom;
            }
            set
            {
                _timeFrom = value;
                NotifyOfPropertyChange(() => TimeFrom);
                NotifyOfPropertyChange(() => CanTimeUpdate);
            }
        }
        public DateTime? TimeTo
        {
            get
            {
                return _timeTo;
            }
            set
            {
                _timeTo = value;
                NotifyOfPropertyChange(() => TimeTo);
                NotifyOfPropertyChange(() => CanTimeUpdate);
            }
        }
        public void TimeUpdate()
        {
            using (_dbContext = new WHTDbContext())
            {
                if (_dbContext.Hours.Any(x =>
                    x.WorkingDate.Year.Equals(SelectedDate.Year) &&
                    x.WorkingDate.Month.Equals(SelectedDate.Month) &&
                    x.WorkingDate.Day.Equals(SelectedDate.Day) &&
                    x.EmployeeId.Equals(SelectedEmployee.Id)))
                {
                    var toUpdate = _dbContext.Hours.Single(x =>
                    x.WorkingDate.Year.Equals(SelectedDate.Year) &&
                    x.WorkingDate.Month.Equals(SelectedDate.Month) &&
                    x.WorkingDate.Day.Equals(SelectedDate.Day) &&
                    x.EmployeeId.Equals(SelectedEmployee.Id));

                    toUpdate.From = TimeFrom.Value.TimeOfDay;
                    toUpdate.To = TimeTo.Value.TimeOfDay;
                    _dbContext.SaveChanges();
                    MessageQueue.Enqueue("ZAKTUALIZOWANO");
                }
                else
                {
                    HoursModel tempHours = new HoursModel();
                    tempHours.WorkingDate = SelectedDate;
                    tempHours.From = TimeFrom.Value.TimeOfDay;
                    tempHours.To = TimeTo.Value.TimeOfDay;
                    tempHours.EmployeeId = SelectedEmployee.Id;
                    var totalHours = (TimeTo - TimeFrom).Value.TotalHours;
                    if (totalHours <= 8)
                    {
                        tempHours.Normal = (decimal)totalHours;
                    }
                    else
                    {
                        tempHours.Normal = 8;
                        tempHours.Overtime = (decimal)(totalHours - 8);
                    }
                    _dbContext.Hours.Add(tempHours);
                    _dbContext.SaveChanges();
                    MessageQueue.Enqueue("DODANO");
                }
            }
        }
        private void timer_tick(object sender, EventArgs e)
        {
            NotifyOfPropertyChange(() => Time);
        }
    }
}
