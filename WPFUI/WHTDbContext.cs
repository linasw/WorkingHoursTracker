namespace WPFUI
{
    using System;
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

            this.SaveChanges();
        }
    }
}