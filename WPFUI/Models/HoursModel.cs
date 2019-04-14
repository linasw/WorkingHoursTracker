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
        public decimal Normal { get; set; }
        [DefaultValue(0)]
        public decimal Overtime { get; set; }
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        
        public virtual EmployeeModel Employee { get; set; }
    }
}
