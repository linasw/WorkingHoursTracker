using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WPFUI.ViewModels
{
    public class WorkingHoursViewModel : Screen
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private DispatcherTimer _timer;


        private String _time;

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

        public WorkingHoursViewModel()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(timer_tick);
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Start();
        }

        private void timer_tick(object sender, EventArgs e)
        {
            NotifyOfPropertyChange(() => Time);
        }
    }
}
