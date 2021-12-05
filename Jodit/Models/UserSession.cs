using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jodit.Models
{
    [Table("jodit_user_session")]
    public class UserSession
    {
        [Key]
        [Column("id_user_session")]
        public int IdUserSession { get; set; }
        
        [Column("user_id")]
        public int UserId { get; set; }
        public User User { get; set; }
        
        [Column("id_session")]
        public string IdSession { get; set; }
        
    }
}