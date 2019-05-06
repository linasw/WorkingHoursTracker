using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    [Table("YearMonth")]
    public class YearMonthModel
    {
        [Key, Column(Order = 0)]
        public int Year { get; set; }
        [Key, Column(Order = 1)]
        public int Month { get; set; }
        public int MonthsWorkingHours { get; set; }
        public string InternalMonthsWeekendDays { get; set; }
        [NotMapped]
        public HashSet<int> MonthsWeekendDays
        {
            get
            {
                return new HashSet<int>(Array.ConvertAll(InternalMonthsWeekendDays.Split(';'), int.Parse));
            }
            set
            {
                var _data = value;
                InternalMonthsWeekendDays = string.Join(";", _data.Select(p => p.ToString()).ToArray());
            }
        }
    }
}
