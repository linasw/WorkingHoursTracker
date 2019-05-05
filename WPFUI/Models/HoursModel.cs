using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    [Table("Hours")]
    public class HoursModel
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "Date")]
        public DateTime WorkingDate { get; set; }
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
        [DefaultValue(0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public TimeSpan? Normal
        {
            get
            {
                if (From == null || To == null)
                {
                    return new TimeSpan(0, 0, 0);
                }

                var totalHours = (To - From);
                if (totalHours >= new TimeSpan(8, 0, 0))
                {
                    return new TimeSpan(8, 0, 0);
                }
                else
                {
                    return totalHours;
                }
            }
            private set { }
        }
        [DefaultValue(0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public TimeSpan? Overtime
        {
            get
            {
                if (From == null || To == null)
                {
                    return new TimeSpan(0, 0, 0);
                }

                var totalHours = (To - From);
                if (totalHours <= new TimeSpan(8, 0, 0))
                {
                    return new TimeSpan(0, 0, 0);
                }
                else
                {
                    var overtimeHours = totalHours - new TimeSpan(8, 0, 0);
                    return new TimeSpan(overtimeHours.Ticks);
                }
            }
            private set { }
        }
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        
        public virtual EmployeeModel Employee { get; set; }
    }
}
