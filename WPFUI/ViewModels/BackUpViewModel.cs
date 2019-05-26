using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    public class BackUpViewModel : Screen
    {
        private WHTDbContext _dbContext;
        private BindableCollection<EmployeeModel> _employees;
        private string _filePath;
        private BindableCollection<HoursModel> _hours;
        private SnackbarMessageQueue _messageQueue;
        private DateTime _selectedDateTime;

        private BindableCollection<YearMonthModel> yearMonths;

        public BackUpViewModel()
        {
            SelectedDateTime = DateTime.Now;
            MessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(5000));
        }

        public BindableCollection<EmployeeModel> Employees
        {
            get
            {
                return _employees;
            }
            set
            {
                _employees = value;
                NotifyOfPropertyChange(() => Employees);
            }
        }
        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
                NotifyOfPropertyChange(() => FilePath);
            }
        }
        public BindableCollection<HoursModel> Hours
        {
            get
            {
                return _hours;
            }
            set
            {
                _hours = value;
                NotifyOfPropertyChange(() => Hours);
            }
        }
        public SnackbarMessageQueue MessageQueue
        {
            get
            {
                return _messageQueue;
            }
            set
            {
                _messageQueue = value;
                NotifyOfPropertyChange(() => MessageQueue);
            }
        }
        public DateTime SelectedDateTime
        {
            get
            {
                return _selectedDateTime;
            }
            set
            {
                _selectedDateTime = value;
                NotifyOfPropertyChange(() => SelectedDateTime);
            }
        }
        public BindableCollection<YearMonthModel> YearMonths
        {
            get
            {
                return yearMonths;
            }
            set
            {
                yearMonths = value;
                NotifyOfPropertyChange(() => YearMonths);
            }
        }

        public void BackUp(DateTime selectedDateTime, string filePath)
        {
            using (_dbContext = new WHTDbContext())
            {
                Employees = new BindableCollection<EmployeeModel>(_dbContext.Employees);
                Hours = new BindableCollection<HoursModel>(_dbContext.Hours.Where(x => x.WorkingDate.Year.Equals(selectedDateTime.Year) && x.WorkingDate.Month.Equals(selectedDateTime.Month)));
                YearMonths = new BindableCollection<YearMonthModel>(_dbContext.YearMonths.Where(x => x.Year.Equals(selectedDateTime.Year) && x.Month.Equals(selectedDateTime.Month)));
            }

            var fileName = $"{selectedDateTime.ToShortDateString()}_backup.xlsx";
            var daysInMonth = DateTime.DaysInMonth(selectedDateTime.Year, selectedDateTime.Month);
            var numOfEmployees = Employees.Count;
            var weekends = YearMonths.Select(x => x.MonthsWeekendDays).First();

            using (var p = new ExcelPackage(new System.IO.FileInfo($"{filePath}/{fileName}")))
            {
                var ws = p.Workbook.Worksheets.Add("BU");

                #region SET BORDER/STYLE ON WHOLE WORKSHEET
                var tempRange = ws.Cells[1, 1, daysInMonth + 4, numOfEmployees + 1];
                tempRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                tempRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                tempRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                tempRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                tempRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                tempRange.Style.Border.Left.Color.SetColor(Color.Black);
                tempRange.Style.Border.Right.Color.SetColor(Color.Black);
                tempRange.Style.Border.Top.Color.SetColor(Color.Black);
                tempRange.Style.Border.Bottom.Color.SetColor(Color.Black);
                #endregion

                #region SET WEEKENDS/HOLIDAYS
                for (int row = 1; row <= daysInMonth; row++) 
                {
                    var dt = new DateTime(selectedDateTime.Year, selectedDateTime.Month, row);
                    ws.Cells[row + 1, 1].Value = dt.ToLongDateString();

                    if (weekends.Contains(dt.Day))
                    {
                        var range = ws.Cells[row + 1, 1, row + 1, numOfEmployees + 1];
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.Brown);  //.Style.Font.Color.SetColor(System.Drawing.Color.Brown);
                    }
                }

                ws.Cells[daysInMonth + 2, 1].Value = "Podstawowe";
                ws.Cells[daysInMonth + 3, 1].Value = "Nadgodziny";
                ws.Cells[daysInMonth + 4, 1].Value = "Suma";
                ws.Cells[daysInMonth + 2, 1, daysInMonth + 4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells[daysInMonth + 2, 1, daysInMonth + 4, numOfEmployees + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[daysInMonth + 2, 1, daysInMonth + 4, numOfEmployees + 1].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                #endregion

                #region SET EMPLOYEES HOURS
                for (int employeeNo = 0; employeeNo < numOfEmployees; employeeNo++)
                {
                    var emp = Employees[employeeNo];
                    var cell = ws.Cells[1, employeeNo + 2];
                    cell.Value = emp.FirstName;

                    var empHours = Hours.Where(x => x.EmployeeId.Equals(emp.Id)).ToList();

                    double normalHoursSum = 0, overtimeHoursSum = 0, allHoursSum = 0;
                    foreach (var workDay in empHours)
                    {
                        var day = workDay.WorkingDate.Day;
                        TimeSpan interval = (TimeSpan)(workDay.To - workDay.From);
                        var totalHours = interval.TotalHours;
                        ws.Cells[day + 1, employeeNo + 2].Value = totalHours;

                        allHoursSum += totalHours;

                        if (weekends.Contains(day))
                        {
                            overtimeHoursSum += totalHours;
                            continue;
                        }

                        if (totalHours > 8)
                        {
                            normalHoursSum += 8;
                            overtimeHoursSum += totalHours - 8;
                        }
                        else
                        {
                            normalHoursSum += totalHours;
                        }
                    }

                    ws.Cells[daysInMonth + 2, employeeNo + 2].Value = normalHoursSum;
                    ws.Cells[daysInMonth + 3, employeeNo + 2].Value = overtimeHoursSum;
                    ws.Cells[daysInMonth + 4, employeeNo + 2].Value = allHoursSum;
                }
                #endregion

                ws.Cells[1, 1, daysInMonth, numOfEmployees + 1].AutoFitColumns();

                p.Save();
            }

            MessageQueue.Enqueue("Backup utworzony!");
        }

        public bool CanBackUp(DateTime selectedDateTime, string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || selectedDateTime == null)
            {
                return false;
            }
            return true;
        }
        public void ChooseFilePath()
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
            var result = dialog.ShowDialog();
            if (result.Value)
            {
                FilePath = dialog.SelectedPath;
            }
        }
    }
}
