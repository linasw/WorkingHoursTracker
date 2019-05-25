using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFUI.Models;
using WPFUI.Views;

namespace WPFUI.ViewModels
{
    public class MonthOverviewViewModel : Screen
    {
        private WHTDbContext _dbContext;
        private DateTime _selectedDate;
        private BindableCollection<EmployeeHours> _selectedDateEmployeesHours;
        private string _selectedMonthMissingInfo;
        private string _weekends;
        private BindableCollection<YearMonthModel> _yearMonth;
        private bool _yearOrMonthChanged;

        public MonthOverviewViewModel()
        {
            SelectedDate = DateTime.Now;
            GetAndSetAllInfoFromDatabase();
        }

        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                if (SelectedDate.Year.Equals(value.Year) && SelectedDate.Month.Equals(value.Month))
                {
                    _yearOrMonthChanged = false;
                }
                else
                {
                    _yearOrMonthChanged = true;
                }
                _selectedDate = value;
                NotifyOfPropertyChange(() => SelectedDate);
                NotifyOfPropertyChange(() => Weekends);
                NotifyOfPropertyChange(() => SelectedDateIsWeekend);
                NotifyOfPropertyChange(() => SelectedDateEmployeesHours);
                if (_yearOrMonthChanged)
                {
                    NotifyOfPropertyChange(() => SelectedMonthMissingInfo);
                }
            }
        }

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
                    return "NIE";
                }
            }
        }

        public string SelectedMonthMissingInfo
        {
            get
            {
                using (_dbContext = new WHTDbContext())
                {
                    var selectedMonthsHours = _dbContext.Hours.Where(x => x.WorkingDate.Year.Equals(SelectedDate.Year) && x.WorkingDate.Month.Equals(SelectedDate.Month));

                    if (!selectedMonthsHours.Any())
                    {
                        return _selectedMonthMissingInfo = "BRAK USTAWIONYCH GODZIN W CAŁYM MIESIĄCU.";
                    }

                    var daysOfMonth = DateTime.DaysInMonth(SelectedDate.Year, SelectedDate.Month);
                    var employees = _dbContext.Employees.ToList();
                    HashSet<int> listOfMissingDaysInfo = new HashSet<int>();

                    for (int i = 1; i <= daysOfMonth; i++)
                    {
                        var dayHours = _dbContext.Hours.Where(x => x.WorkingDate.Year.Equals(SelectedDate.Year) && x.WorkingDate.Month.Equals(SelectedDate.Month) && x.WorkingDate.Day.Equals(i));

                        if (!dayHours.Any())
                        {
                            listOfMissingDaysInfo.Add(i);
                            continue;
                        }

                        var dayHoursEmployeeIDList = dayHours.Select(x => x.EmployeeId).ToList();

                        foreach (var employee in employees)
                        {
                            if (dayHoursEmployeeIDList.Contains(employee.Id))
                            {
                                continue;
                            }
                            else
                            {
                                listOfMissingDaysInfo.Add(i);
                                break;
                            }
                        }
                    }

                    if (!listOfMissingDaysInfo.Any())
                    {
                        return _selectedMonthMissingInfo = "Wszystkie dni zostały uzupełnione.";
                    }

                    return _selectedMonthMissingInfo = string.Join(", ", listOfMissingDaysInfo);
                }
            }
        }

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

        private void GetAndSetAllInfoFromDatabase()
        {
            using (_dbContext = new WHTDbContext())
            {
                YearMonth = new BindableCollection<YearMonthModel>(_dbContext.YearMonths);
            }
        }

        #region MONTH OPTION DIALOG
        public async void MonthOptions()
        {
            var view = new MonthOptionsDialogView()
            {
                DataContext = new MonthOptionsDialogViewModel()
            };

            var result = await DialogHost.Show(view, "RootDialog", VacationOpenedEventHandler, VacationClosingEventHandler);
        }

        private void VacationOpenedEventHandler(object sender, DialogOpenedEventArgs eventargs)
        {

        }

        private void VacationClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter == false) return;
            eventArgs.Cancel();

            var dialogViewContent = (MonthOptionsDialogView)eventArgs.Content;
            var dialogData = (MonthOptionsDialogViewModel)dialogViewContent.DataContext;

            YearMonthModel tempYearMonth = new YearMonthModel
            {
                Year = dialogData.SelectedDate.Year,
                Month = dialogData.SelectedDate.Month,
                MonthsWorkingHours = dialogData.MonthsWorkingHours,
                InternalMonthsWeekendDays = string.Join(";", dialogData.WeekendDays)
            };

            eventArgs.Session.UpdateContent(new ProgressDialog());

            using (_dbContext = new WHTDbContext())
            {
                if (_dbContext.YearMonths.Any(x => x.Year.Equals(tempYearMonth.Year) && x.Month.Equals(tempYearMonth.Month)))
                {
                    _dbContext.Entry(tempYearMonth).State = System.Data.Entity.EntityState.Modified;
                } 
                else
                {
                    _dbContext.Entry(tempYearMonth).State = System.Data.Entity.EntityState.Added;
                }

                _dbContext.SaveChanges();
            }

            Task.Delay(TimeSpan.FromSeconds(1))
                .ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion
    }
}
