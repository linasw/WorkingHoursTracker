using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    public class MonthOverviewViewModel : Screen
    {
        private WHTDbContext _dbContext;
        private BindableCollection<YearMonthModel> _yearMonth;
        private DateTime _selectedDate;

        public MonthOverviewViewModel()
        {
            SelectedDate = DateTime.Now;
            GetAndSetAllInfoFromDatabase();
        }

        private void GetAndSetAllInfoFromDatabase()
        {
            using (_dbContext = new WHTDbContext())
            {
                _yearMonth = new BindableCollection<YearMonthModel>(_dbContext.YearMonths);
            }

            NotifyOfPropertyChange(() => YearMonth);
        }

        public BindableCollection<YearMonthModel> YearMonth
        {
            get
            {
                return _yearMonth;
            }
            set
            {
                _yearMonth = value;
                NotifyOfPropertyChange(() => YearMonth);
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
                NotifyOfPropertyChange(() => Weekends);
                NotifyOfPropertyChange(() => SelectedDateIsWeekend);
                NotifyOfPropertyChange(() => SelectedDateEmployeesHours);
                NotifyOfPropertyChange(() => SelectedMonthMissingInfo);
            }
        }

        private string _weekends;

        public string Weekends
        {
            get
            {
                if (!YearMonth.Where(x => x.Year.Equals(SelectedDate.Year) && x.Month.Equals(SelectedDate.Month)).Any())
                {
                    return "BRAK USTAWIONYCH WEEKENDÓW. USTAW JE KLIKAJĄC PRZYCISK NA GÓRZE";
                }

                var temp = YearMonth.Where(x => x.Year.Equals(SelectedDate.Year) && x.Month.Equals(SelectedDate.Month)).Select(x => x.MonthsWeekendDays).First().OrderBy(x => x);
                return _weekends = string.Join(", ", temp);
            }
        }

        public string SelectedDateIsWeekend
        {
            get
            {
                var temp = YearMonth.Where(x => x.Year.Equals(SelectedDate.Year) && x.Month.Equals(SelectedDate.Month));

                if (!temp.Any())
                {
                    return "BRAK USTAWIONYCH WEEKENDÓW. USTAW JE KLIKAJĄC PRZYCISK NA GÓRZE";
                }

                var tempDay = temp.Select(x => x.MonthsWeekendDays).First();

                if (tempDay.Contains(SelectedDate.Day))
                {
                    return "TAK";
                }
                else
                {
                    return  "NIE";
                }
            }
        }

        private BindableCollection<EmployeeHours> _selectedDateEmployeesHours;

        public BindableCollection<EmployeeHours> SelectedDateEmployeesHours
        {
            get
            {
                using (_dbContext = new WHTDbContext())
                {
                    var query = from employee in _dbContext.Employees
                                join hours in _dbContext.Hours.Where(x => x.WorkingDate.Year.Equals(SelectedDate.Year) && x.WorkingDate.Month.Equals(SelectedDate.Month) && x.WorkingDate.Day.Equals(SelectedDate.Day))
                                on employee.Id equals hours.EmployeeId into gj
                                from subhour in gj.DefaultIfEmpty()
                                select new EmployeeHours
                                {
                                    FirstName = employee.FirstName,
                                    LastName = employee.LastName,
                                    From = subhour.From == new TimeSpan(0, 0, 0) ? new TimeSpan(0, 0, 0) : subhour.From,
                                    To = subhour.To == new TimeSpan(0, 0, 0) ? new TimeSpan(0, 0, 0) : subhour.To
                                };

                    
                    _selectedDateEmployeesHours = new BindableCollection<EmployeeHours>(query);
                }

                return _selectedDateEmployeesHours;
            }
            set
            {
                _selectedDateEmployeesHours = value;
                NotifyOfPropertyChange(() => SelectedDateEmployeesHours);
            }
        }

        public string SelectedMonthMissingInfo
        {
            get
            {
                //if (string.IsNullOrEmpty(_selectedMonthMissingInfo) && !)
                using (_dbContext = new WHTDbContext())
                {
                    var selectedMonthsHours = _dbContext.Hours.Where(x => x.WorkingDate.Year.Equals(SelectedDate.Year) && x.WorkingDate.Month.Equals(SelectedDate.Month));

                    if (!selectedMonthsHours.Any())
                    {
                        return "BRAK USTAWIONYCH GODZIN W CAŁYM MIESIĄCU.";
                    }

                    HashSet<int> listOfMissingDaysInfo = new HashSet<int>();

                    //TODO - loop though each day in month and check if all employees entered their working hours

                    return string.Join(", ", listOfMissingDaysInfo);
                }
            }
        }

    }
}
