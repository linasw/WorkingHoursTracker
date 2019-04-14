using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.Models.Enums;

namespace WPFUI.Models
{
    [Table("Vacation")]
    public class VacationModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "Date")]
        public DateTime From { get; set; }
        [Required]
        [Column(TypeName = "Date")]
        public DateTime To { get; set; }
        [Required]
        public VacationTypes Type { get; set; }
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }

        public virtual EmployeeModel Employee { get; set; }
    }
}
