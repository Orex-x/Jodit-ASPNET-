using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jodit.Models
{
    [Table("jodit_schedule_statement")]
    public class ScheduleStatement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("id_schedule_statement")]
        public int IdScheduleStatement { get; set; }
        
        [Column("before_user")]
        public int BeforeUserId { get; set; }
        public User BeforeUser { get; set; }

        [Column("group_id")]
        public int GroupId { get; set; }
        public Group Group { get; set; }
        
        [Column("replacement_date")]
        public DateTime ReplacementDate { get; set; }
        
        [Column("comment")]    
        public string Comment { get; set; }



    }
}