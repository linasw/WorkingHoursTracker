using Caliburn.Micro;
using System.Windows;

namespace WPFUI.ViewModels
{
    public class ShellViewModel : Screen
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public void Quit()
        {
            Application.Current.Shutdown();
        }

        public void Minimaze()
        {
            WindowState = WindowState.Minimized;
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