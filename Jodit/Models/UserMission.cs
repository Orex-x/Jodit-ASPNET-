using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jodit.Models
{
    [Table("jodit_user_mission")]
    public class UserMission
    {
        [Key]
        public int IdUserMission { get; set; }

        public Group Group { get; set; }
        public User Author { get; set; }
        
        public User Executor { get; set; }
        public Mission Mission { get; set; }
        public string Status { get; set; }
    }
}