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
        private DateTime _selectedDate;
        private string _weekendDays;
        private BindableCollection<YearMonthModel> _yearsMonths;

        public MonthOptionsDialogViewModel()
        {
            SelectedDate = DateTime.Now;
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
                NotifyOfPropertyChange(() => MonthsWorkingHours);
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
                var query = YearMonths.Where(x => x.Year.Equals(SelectedDate.Year) && x.Month.Equals(SelectedDate.Month));
                if (query.Any())
                {
                    var first = query.First();
                    MonthsWorkingHours = first.MonthsWorkingHours;
                    WeekendDays = first.InternalMonthsWeekendDays;
                }
                else
                {
                    MonthsWorkingHours = 0;
                    WeekendDays = "WPISZ WEEKENDY";
                }
            }
        }        
        public string WeekendDays
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
        public BindableCollection<YearMonthModel> YearMonths
        {
            get
            {
                using (_dbContext = new WHTDbContext())
                {
                    var temp = _dbContext.YearMonths;
                    _yearsMonths = new BindableCollection<YearMonthModel>(temp);
                }
                return _yearsMonths;
            }
            set
            {
                _yearsMonths = value;
                NotifyOfPropertyChange(() => YearMonths);
            }
        }
    }
}
