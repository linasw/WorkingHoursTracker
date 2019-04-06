using Caliburn.Micro;
using System.Windows;

namespace WPFUI.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public ShellViewModel()
        {
            ActivateItem(new WorkingHoursViewModel());
        }

        public void Quit()
        {
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

        }

        public void LoadMonthOverview()
        {

        }

        public void BackUp()
        {

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