using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jodit.Models
{
    [Table("jodit_group_invite")]
    public class GroupInvite
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("id_group_invite")]
        public int IdGroupInvite { get; set; }
        
        [Column("invited_user")]
        public int InvitedUserId { get; set; }
        public User InvitedUser { get; set; }
        
        [Column("inviting_user")]
        public int InvitingUserId { get; set; }
        public User InvitingUser { get; set; }
        
        [Column("group_id")]
        public int GroupId { get; set; }
        public Group Group { get; set; }
        
        [Column("title")]
        public string Title { get; set; }
        
    }
}