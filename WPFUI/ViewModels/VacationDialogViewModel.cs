using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.Models;
using WPFUI.Models.Enums;

namespace WPFUI.ViewModels
{
    public class VacationDialogViewModel : Screen
    {
        private WHTDbContext _dbContext;
        private DateTime _dateFrom;
        private DateTime _dateTo;
        private BindableCollection<EmployeeModel> _employees;
        private EmployeeModel _selectedEmployee;
        private VacationTypes _selectedVacationType;
        private BindableCollection<VacationTypes> _vacationTypes;

        public VacationDialogViewModel()
        {
            SelectedEmployee = Employees.First();
            SelectedVacationType = VacationTypes.First();
            DateFrom = DateTime.Now;
            DateTo = DateTime.Now;
        }
        public DateTime DateFrom
        {
            get
            {
                return _dateFrom;
            }
            set
            {
                _dateFrom = value;
                NotifyOfPropertyChange(() => DateFrom);
            }
        }
        public DateTime DateTo
        {
            get
            {
                return _dateTo;
            }
            set
            {
                _dateTo = value;
                NotifyOfPropertyChange(() => DateTo);
            }
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
            }
        }
        public VacationTypes SelectedVacationType
        {
            get
            {
                return _selectedVacationType;
            }
            set
            {
                _selectedVacationType = value;
                NotifyOfPropertyChange(() => SelectedVacationType);
            }
        }
        public BindableCollection<VacationTypes> VacationTypes
        {
            get
            {
                return _vacationTypes = new BindableCollection<VacationTypes>(Enum.GetValues(typeof(VacationTypes)).Cast<VacationTypes>());
            }
            set
            {
                _vacationTypes = value;
                NotifyOfPropertyChange(() => VacationTypes);
            }
        }
    }
}
