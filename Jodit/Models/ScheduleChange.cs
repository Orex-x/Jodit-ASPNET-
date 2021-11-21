using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jodit.Models
{
    
    [Table("jodit_schedule_change")]
    public class ScheduleChange
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("id_schedule_change")]
        public int IdScheduleChange { get; set; }
        
        [Column("before_user")]
        public int BeforeUserId { get; set; }
        public User BeforeUser { get; set; }
        
        [Column("after_user")]
        public int AfterUserId { get; set; }
        public User AfterUser { get; set; }
        
        [Column("group_id")]
        public int GroupId { get; set; }
        public Group Group { get; set; }
        
        [Column("after_user_date")]
        public DateTime AfterUserDate { get; set; }
        
        [Column("before_user_date")]
        public DateTime BeforeUserDate { get; set; }
    }
}