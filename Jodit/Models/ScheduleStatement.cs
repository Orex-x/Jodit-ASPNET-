using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jodit.Models
{
    [Table("jodit_schedule_statement")]
    public class ScheduleStatement
    {
        [Key]
        public int IdScheduleStatement { get; set; }
        public User BeforeUser { get; set; }
        public Group Group { get; set; }
        public DateTime ReplacementDate { get; set; }
        public string Comment { get; set; }
    }
}