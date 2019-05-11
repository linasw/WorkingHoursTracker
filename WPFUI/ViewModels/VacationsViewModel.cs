﻿using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFUI.Models;
using WPFUI.Views;

namespace WPFUI.ViewModels
{
    public class VacationsViewModel : Screen
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private WHTDbContext _dbContext;
        private BindableCollection<EmployeeVacationModel> _employeesVacations;
        private EmployeeVacationModel _selectedEmployeeVacation;

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
        public EmployeeVacationModel SelectedEmployeeVacation
        {
            get
            {
                return _selectedEmployeeVacation;
            }
            set
            {
                _selectedEmployeeVacation = value;
                NotifyOfPropertyChange(() => SelectedEmployeeVacation);
            }
        }

        #region VACATION DIALOG
        public async void AddVacation()
        {
            var view = new VacationDialogView
            {
                DataContext = new VacationDialogViewModel()
            };

            var result = await DialogHost.Show(view, "RootDialog", OpenedEventHandler, ClosingEventHandler);
        }

        private void OpenedEventHandler(object sender, DialogOpenedEventArgs eventargs)
        {

        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter == false) return;
            eventArgs.Cancel();

            var dialogViewContent = (VacationDialogView)eventArgs.Content;
            var dialogData = (VacationDialogViewModel)dialogViewContent.DataContext;

            VacationModel tempVacation = new VacationModel
            {
                From = dialogData.DateFrom,
                To = dialogData.DateTo,
                Type = dialogData.SelectedVacationType,
                EmployeeId = dialogData.SelectedEmployee.Id
            };

            eventArgs.Session.UpdateContent(new ProgressDialog());

            using (_dbContext = new WHTDbContext())
            {
                _dbContext.Vacations.Add(tempVacation);
                _dbContext.SaveChanges();
            }
            NotifyOfPropertyChange(() => EmployeesVacations);

            Task.Delay(TimeSpan.FromSeconds(1))
                .ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion
    }
}
