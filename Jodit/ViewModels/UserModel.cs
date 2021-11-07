using System.Collections.Generic;
using Jodit.Models;

namespace Jodit.ViewModels
{
    public class UserModel
    {
        public IEnumerable<User> Users { get; set; }
        
        public User User { get; set; }
        public Group Group { get; set; }
    }
}