using Caliburn.Micro;
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
        private decimal _normalHoursSum;
        private decimal _overtimeHoursSum;
        private DateTime _selectedDate;
        private string _selectedDateHoursSum;
        private EmployeeModel _selectedEmployee;
        private System.Nullable<DateTime> _selectedTimeFrom;
        private System.Nullable<DateTime> _selectedTimeTo;
        private String _time;
        private DispatcherTimer _timer;

        public WorkingHoursViewModel()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(timer_tick);
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Start();
            Employees = new BindableCollection<EmployeeModel>();
            _selectedEmployee = Employees.First();
            _selectedDate = DateTime.Now;
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
        public decimal NormalHoursSum
        {
            get
            {
                _normalHoursSum = 0;
                using (_dbContext = new WHTDbContext())
                {
                    var daysOfSelectedMonth = _dbContext.Hours
                        .Where(x => x.WorkingDate.Month.Equals(SelectedDate.Month) && x.EmployeeId.Equals(SelectedEmployee.Id));

                    if (daysOfSelectedMonth.Any())
                    {
                        var listOfDays = daysOfSelectedMonth.ToList();
                        foreach (var item in listOfDays)
                        {
                            _normalHoursSum += item.Normal;
                        }
                    }
                }
                return _normalHoursSum;
            }
        }
        public decimal OvertimeHoursSum
        {
            get
            {
                _overtimeHoursSum = 0;
                using (_dbContext = new WHTDbContext())
                {
                    var daysOfSelectedMonth = _dbContext.Hours
                        .Where(x => x.WorkingDate.Month.Equals(SelectedDate.Month) && x.EmployeeId.Equals(SelectedEmployee.Id));

                    if (daysOfSelectedMonth.Any())
                    {
                        var listOfDays = daysOfSelectedMonth.ToList();
                        foreach (var item in listOfDays)
                        {
                            _overtimeHoursSum += item.Overtime;
                        }
                    }
                }
                return _overtimeHoursSum;
            }
        }
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                NotifyOfPropertyChange(() => SelectedDate);
                NotifyOfPropertyChange(() => SelectedTimeFrom);
                NotifyOfPropertyChange(() => SelectedTimeTo);
                NotifyOfPropertyChange(() => NormalHoursSum);
                NotifyOfPropertyChange(() => OvertimeHoursSum);
                NotifyOfPropertyChange(() => SelectedDateHoursSum);
            }
        }
        public string SelectedDateHoursSum
        {
            get
            {
                if (SelectedTimeFrom != null && SelectedTimeTo != null)
                {
                    TimeSpan interval = (TimeSpan)(SelectedTimeTo - SelectedTimeFrom);
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
                NotifyOfPropertyChange(() => SelectedTimeFrom);
                NotifyOfPropertyChange(() => SelectedTimeTo);
                NotifyOfPropertyChange(() => NormalHoursSum);
                NotifyOfPropertyChange(() => OvertimeHoursSum);
                NotifyOfPropertyChange(() => SelectedDateHoursSum);
            }
        }
        public System.Nullable<DateTime> SelectedTimeFrom
        {
            get
            {
                using (_dbContext = new WHTDbContext())
                {
                    var temp = _dbContext.Hours
                        .Where(x => x.WorkingDate.Equals(SelectedDate) && x.EmployeeId.Equals(SelectedEmployee.Id));

                    if (temp.Any())
                    {
                        _selectedTimeFrom = new DateTime(temp.Select(x => x.From).ToList().First().Ticks);
                    }
                    else
                    {
                        //_selectedTimeFrom = new DateTime(new TimeSpan(0, 0, 0).Ticks);
                        _selectedTimeFrom = null;
                    }
                }
                return _selectedTimeFrom;
            }
            set
            {
                _selectedTimeFrom = value;
                NotifyOfPropertyChange(() => SelectedTimeFrom);
            }
        }
        public System.Nullable<DateTime> SelectedTimeTo
        {
            get
            {
                using (_dbContext = new WHTDbContext())
                {
                    var temp = _dbContext.Hours
                        .Where(x => x.WorkingDate.Equals(SelectedDate) && x.EmployeeId.Equals(SelectedEmployee.Id));

                    if (temp.Any())
                    {
                        _selectedTimeTo = new DateTime(temp.Select(x => x.To).ToList().First().Ticks);
                    }
                    else
                    {
                        _selectedTimeTo = null;
                    }
                }
                return _selectedTimeTo;
            }
            set
            {

                _selectedTimeTo = value;
                NotifyOfPropertyChange(() => SelectedTimeTo);
            }
        }
        public String Time
        {
            get
            {
                return DateTime.Now.ToLongDateString() + ", " + DateTime.Now.ToLongTimeString();
            }
            set
            {
                _time = value;
                NotifyOfPropertyChange(() => Time);
            }
        }

        private void timer_tick(object sender, EventArgs e)
        {
            NotifyOfPropertyChange(() => Time);
        }
    }
}
