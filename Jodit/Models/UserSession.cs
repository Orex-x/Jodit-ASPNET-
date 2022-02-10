using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jodit.Models
{
    [Table("jodit_user_session")]
    public class UserSession
    {
        [Key]
        public int IdUserSession { get; set; }
        public User User { get; set; }
        public string IdSession { get; set; }
        
    }
}