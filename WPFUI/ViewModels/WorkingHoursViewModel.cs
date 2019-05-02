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
        private BindableCollection<HoursModel> _hours;
        private DateTime _selectedDate;
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

                    var tempTime = _hours.Where(x => x.WorkingDate.ToShortDateString().Equals(SelectedDate.ToShortDateString()) && x.EmployeeId.Equals(SelectedEmployee.Id));
                    if (tempTime.Any())
                    {
                        _timeFrom = new DateTime(tempTime.Select(x => x.From).ToList().First().Ticks);
                        _timeTo = new DateTime(tempTime.Select(x => x.To).ToList().First().Ticks);
                    }
                    else
                    {
                        _timeFrom = null;
                        _timeTo = null;
                    }
                    NotifyOfPropertyChange(() => TimeFrom);
                    NotifyOfPropertyChange(() => TimeTo);
                }
                return _hours;
            }
            set
            {
                _hours = value;
                NotifyOfPropertyChange(() => Hours);
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
                NotifyOfPropertyChange(() => SelectedDate);
                Hours.Refresh();
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
            }
        }
        public bool CanTimeUpdate(DateTime? selectedTimeFrom, DateTime? selectedTimeTo, DateTime selectedDate, EmployeeModel selectedEmployee)
        {
            if (selectedTimeFrom == null || selectedTimeTo == null || selectedDate == null || selectedEmployee == null)
            {
                return false;
            }
            return true;
        }
        public void TimeUpdate(DateTime? selectedTimeFrom, DateTime? selectedTimeTo, DateTime selectedDate, EmployeeModel selectedEmployee)
        {
            
        }
        private void timer_tick(object sender, EventArgs e)
        {
            NotifyOfPropertyChange(() => Time);
        }
    }
}
