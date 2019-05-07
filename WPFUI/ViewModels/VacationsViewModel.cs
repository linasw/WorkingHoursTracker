using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    public class VacationsViewModel : Screen
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private WHTDbContext _dbContext;
        private BindableCollection<EmployeeVacationModel> _employeesVacations;
        private BindableCollection<VacationModel> _vacations;

        public VacationsViewModel()
        {

        }
        public BindableCollection<EmployeeVacationModel> EmployeesVacations
        {
            get
            {
                using (_dbContext = new WHTDbContext())
                {
                    var tempEmployeesVacations = from v in _dbContext.Vacations
                                                 join em in _dbContext.Employees on v.EmployeeId equals em.Id into emv
                                                 from x in emv.DefaultIfEmpty()
                                                 select new EmployeeVacationModel
                                                 {
                                                     EmployeeId = v.EmployeeId,
                                                     VacationId = v.Id,
                                                     FirstName = x.FirstName,
                                                     LastName = x.LastName,
                                                     From = v.From,
                                                     To = v.To,
                                                     Type = v.Type
                                                 };

                    _employeesVacations = new BindableCollection<EmployeeVacationModel>(tempEmployeesVacations);
                }
                return _employeesVacations;
            }
            set
            {
                _employeesVacations = value;
                NotifyOfPropertyChange(() => EmployeesVacations);
            }
        }
        public BindableCollection<VacationModel> Vacations
        {
            get
            {
                using (_dbContext = new WHTDbContext())
                {
                    var tempVacations = _dbContext.Vacations;
                    _vacations = new BindableCollection<VacationModel>(tempVacations);
                }
                return _vacations;
            }
            set
            {
                _vacations = value;
                NotifyOfPropertyChange(() => Vacations);
            }
        }
    }
}
