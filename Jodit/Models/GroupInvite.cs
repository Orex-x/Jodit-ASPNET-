using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jodit.Models
{
    [Table("jodit_group_invite")]
    public class GroupInvite
    {
        [Key]
        public int IdGroupInvite { get; set; }
        public User InvitedUser { get; set; }
        public User InvitingUser { get; set; }
        public Group Group { get; set; }
        public string Title { get; set; }
        
    }
}