using System.Collections.Generic;
using Jodit.Models;

namespace Jodit.ViewModels
{
    public class GroupModel
    {
         public IEnumerable<Group> Groups { get; set; }
         
         public Group Group { get; set; }
         
         public User User { get; set; }
    }
}