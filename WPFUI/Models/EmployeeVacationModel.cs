using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.Models.Enums;

namespace WPFUI.Models
{
    public class EmployeeVacationModel
    {
        public int EmployeeId { get; set; }
        public int VacationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public VacationTypes Type { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
            set
            {
                var temp = value;
                var splits = temp.Split(' ');
                FirstName = splits[0];
                LastName = splits[1];
            }
        }
    }
}
