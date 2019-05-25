using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class EmployeeHours
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public TimeSpan? From { get; set; }
        public TimeSpan? To { get; set; }

        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
    }
}
