using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jodit.Models
{
    [Table("jodit_UserChatID")]
    public class UserChatId
    {
        [Key]
        public int IdUserChatId{ get; set; }
        public User User { get; set; }
        public string ChatId { get; set; }
        public string Key { get; set; }
    }
}