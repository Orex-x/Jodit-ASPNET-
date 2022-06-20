using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jodit.Models
{
    [Table("jodit_user_group")]
    public class UserGroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("id_user_group")]
        public int IdUserGroup { get; set; }
        
        
        [Column("user_id")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Column("group_id")]
        public int GroupId { get; set; }
        public Group Group { get; set; }
        
        
        
        [Column("is_admin")]
        public bool IsAdmin { get; set; }
    }
}