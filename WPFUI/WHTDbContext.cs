namespace WPFUI
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using WPFUI.Models;

    public class WHTDbContext : DbContext
    {
        public WHTDbContext()
            : base("name=WHTDbContext")
        {
        }

        public DbSet<EmployeeModel> Employees { get; set; }
        public DbSet<HoursModel> Hours { get; set; }
        public DbSet<VacationModel> Vacations { get; set; }
        public DbSet<YearMonthModel> YearMonths { get; set; }

        public void InitDummyData()
        {
            Database.SetInitializer<WHTDbContext>(new DropCreateDatabaseAlways<WHTDbContext>());

            EmployeeModel tempEmp = new EmployeeModel{ FirstName = "Adam", LastName = "Oforek" };
            Employees.Add(tempEmp);

            tempEmp = new EmployeeModel { FirstName = "Marek", LastName = "Janek" };
            Employees.Add(tempEmp);

            tempEmp = new EmployeeModel { FirstName = "Aneta", LastName = "Polak" };
            Employees.Add(tempEmp);

            tempEmp = new EmployeeModel { FirstName = "Cesarz", LastName = "Juliusz" };
            Employees.Add(tempEmp);


            DateTime now = new DateTime();
            now = DateTime.Now.Date;
            HoursModel tempHours = new HoursModel
            {
                WorkingDate = now,
                From = new TimeSpan(8, 30, 0),
                To = new TimeSpan(17, 0, 0),
                EmployeeId = 1
            };
            Hours.Add(tempHours);

            tempHours = new HoursModel
            {
                WorkingDate = now.AddDays(-1),
                From = new TimeSpan(7, 00, 0),
                To = new TimeSpan(17, 0, 0),
                EmployeeId = 1
            };
            Hours.Add(tempHours);

            tempHours = new HoursModel
            {
                WorkingDate = now,
                From = new TimeSpan(6, 30, 0),
                To = new TimeSpan(10, 0, 0),
                EmployeeId = 2
            };
            Hours.Add(tempHours);

            tempHours = new HoursModel
            {
                WorkingDate = now.AddDays(-14),
                From = new TimeSpan(6, 30, 0),
                To = new TimeSpan(10, 0, 0),
                EmployeeId = 1
            };
            Hours.Add(tempHours);

            HashSet<int> weekends = new HashSet<int>();
            weekends.Add(4);
            weekends.Add(5);
            weekends.Add(1);
            weekends.Add(3);
            weekends.Add(11);
            weekends.Add(12);
            weekends.Add(18);
            weekends.Add(19);
            weekends.Add(25);
            weekends.Add(26);
            YearMonthModel yearMonth = new YearMonthModel { Year=2019, Month=5, MonthsWorkingHours=168, MonthsWeekendDays= new HashSet<int>(weekends)};
            YearMonths.Add(yearMonth);

            yearMonth = new YearMonthModel
            {
                Year = 2019,
                Month = 6,
                MonthsWorkingHours = 160,
                MonthsWeekendDays = new HashSet<int>(weekends)
            };
            YearMonths.Add(yearMonth);

            VacationModel vacation = new VacationModel
            {
                From = DateTime.Now,
                To = DateTime.Now.AddDays(7),
                Type = Models.Enums.VacationTypes.PŁATNY,
                EmployeeId = 1
            };
            Vacations.Add(vacation);

            vacation = new VacationModel
            {
                From = DateTime.Now,
                To = DateTime.Now.AddDays(3),
                Type = Models.Enums.VacationTypes.ZWOLNIENIE_LEKARSKIE,
                EmployeeId = 2
            };
            Vacations.Add(vacation);

            vacation = new VacationModel
            {
                From = DateTime.Now,
                To = DateTime.Now.AddDays(3),
                Type = Models.Enums.VacationTypes.OKOLICZNOŚCIOWY,
                EmployeeId = 3
            };
            Vacations.Add(vacation);

            vacation = new VacationModel
            {
                From = DateTime.Now,
                To = DateTime.Now.AddDays(3),
                Type = Models.Enums.VacationTypes.RODZICIELSKI,
                EmployeeId = 1
            };
            Vacations.Add(vacation);

            vacation = new VacationModel
            {
                From = DateTime.Now,
                To = DateTime.Now.AddDays(3),
                Type = Models.Enums.VacationTypes.WYPOCZYNKOWY,
                EmployeeId = 4
            };
            Vacations.Add(vacation);
            vacation = new VacationModel
            {
                From = DateTime.Now,
                To = DateTime.Now.AddDays(3),
                Type = Models.Enums.VacationTypes.ZWOLNIENIE_LEKARSKIE,
                EmployeeId = 2
            };
            Vacations.Add(vacation);
            vacation = new VacationModel
            {
                From = DateTime.Now,
                To = DateTime.Now.AddDays(3),
                Type = Models.Enums.VacationTypes.RODZICIELSKI,
                EmployeeId = 3
            };
            Vacations.Add(vacation);
            vacation = new VacationModel
            {
                From = DateTime.Now,
                To = DateTime.Now.AddDays(3),
                Type = Models.Enums.VacationTypes.ZWOLNIENIE_LEKARSKIE,
                EmployeeId = 4
            };
            Vacations.Add(vacation);
            this.SaveChanges();
        }
    }
}