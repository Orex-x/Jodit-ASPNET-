using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jodit.Models
{
    [Table("jodit_user_mission")]
    public class UserMission
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("id_user_mission")]
        public int IdUserMission { get; set; }
        
        [Column("group_id")]
        public int GroupId { get; set; }
        public Group Group { get; set; }
        
        [Column("author_id")]
        public int AuthorId { get; set; }
        public User Author { get; set; }
        
        
        [Column("executor_id")]
        public int ExecutorId { get; set; }
        public User Executor { get; set; }
        
        [Column("mission_id")]
        public int MissionId { get; set; }
        public Mission Mission { get; set; }
        
        [Column("status")]
        public string Status { get; set; }
    }
}