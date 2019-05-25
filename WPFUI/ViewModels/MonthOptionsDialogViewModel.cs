using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    public class MonthOptionsDialogViewModel : Screen
    {
        private WHTDbContext _dbContext;

        private int _monthsWorkingHours;
        private BindableCollection<int> _normalDays;
        private DateTime _selectedDate;
        private YearMonthModel _selectedDatesYearMonth;
        private BindableCollection<int> _weekendDays;

        public MonthOptionsDialogViewModel()
        {

        }

        public int MonthsWorkingHours
        {
            get
            {
                return _monthsWorkingHours;
            }
            set
            {
                _monthsWorkingHours = value;
                SelectedDatesYearMonth.MonthsWorkingHours = value;
                NotifyOfPropertyChange(() => MonthsWorkingHours);
            }
        }

        public BindableCollection<int> NormalDays
        {
            get
            {
                return _normalDays;
            }
            set
            {
                _normalDays = value;
                NotifyOfPropertyChange(() => NormalDays);
            }
        }

        public DateTime SelectedDate
        {
            get
            {
                using (_dbContext = new WHTDbContext())
                {
                    //var temp = _dbContext.YearMonths.Where(x => x.Year.Equals(SelectedDate.Year) && x.Month.Equals(SelectedDate.Month));
                    
                    if (!_dbContext.YearMonths.Where(x => x.Year.Equals(SelectedDate.Year) && x.Month.Equals(SelectedDate.Month)).Any())
                    {
                        SelectedDatesYearMonth = new YearMonthModel
                        {
                            Year = SelectedDate.Year,
                            Month = SelectedDate.Month,
                            MonthsWorkingHours = 0,
                        };
                    }
                    else
                    {
                        var yearMonth = _dbContext.YearMonths.Where(x => x.Year.Equals(SelectedDate.Year) && x.Month.Equals(SelectedDate.Month)).First();
                        SelectedDatesYearMonth = yearMonth;
                        var weekendsInHashSet = yearMonth.MonthsWeekendDays;
                        WeekendDays = new BindableCollection<int>(weekendsInHashSet);
                        var daysInSelectedDateTime = DateTime.DaysInMonth(SelectedDate.Year, SelectedDate.Month);
                        HashSet<int> normalDays = new HashSet<int>();

                        for (int i = 1; i <= daysInSelectedDateTime; i++)
                        {
                            if (!weekendsInHashSet.Contains(i))
                            {
                                normalDays.Add(i);
                            }
                        }

                        NormalDays = new BindableCollection<int>(normalDays);
                    }
                }
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;
                NotifyOfPropertyChange(() => SelectedDate);
            }
        }

        public YearMonthModel SelectedDatesYearMonth
        {
            get
            {
                return _selectedDatesYearMonth;
            }
            set
            {
                _selectedDatesYearMonth = value;
                NotifyOfPropertyChange(() => SelectedDatesYearMonth);
            }
        }

        public BindableCollection<int> WeekendDays
        {
            get
            {
                return _weekendDays;
            }
            set
            {
                _weekendDays = value;
                NotifyOfPropertyChange(() => WeekendDays);
            }
        }

    }
}
