using Caliburn.Micro;
using System.Windows;

namespace WPFUI.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private WHTDbContext _dbContext;

        public ShellViewModel()
        {
            _dbContext = new WHTDbContext();
            _dbContext.InitDummyData();
            ActivateItem(new WorkingHoursViewModel());
        }

        public void Quit()
        {
            _dbContext.Dispose();
            Application.Current.Shutdown();
        }

        public void Minimaze()
        {
            WindowState = WindowState.Minimized;
        }

        public void LoadWorkHours()
        {
            ActivateItem(new WorkingHoursViewModel());
        }

        public void LoadVacations()
        {
            ActivateItem(new VacationsViewModel());
        }

        public void LoadMonthOverview()
        {
            ActivateItem(new MonthOverviewViewModel());
        }

        public void BackUp()
        {
            ActivateItem(new BackUpViewModel());
        }

        private WindowState _windowState;

        public WindowState WindowState
        {
            get { return _windowState; }
            set
            {
                _windowState = value;
                NotifyOfPropertyChange(() => WindowState);
            }
        }
    }
}