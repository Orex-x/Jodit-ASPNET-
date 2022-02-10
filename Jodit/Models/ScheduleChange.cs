using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jodit.Models
{
    
    [Table("jodit_schedule_change")]
    public class ScheduleChange
    {
        [Key]
        public int IdScheduleChange { get; set; }
        public User BeforeUser { get; set; }
        public User AfterUser { get; set; }
        public Group Group { get; set; }
        public DateTime AfterUserDate { get; set; }
        public DateTime BeforeUserDate { get; set; }
    }
}