using System.Collections.Generic;
using Jodit.Models;

namespace Jodit.ViewModels
{
    public class MissionModel
    {
        public Mission Mission { get; set; }
        
        public List<UserMission> ExecutorsMissions { get; set; }
        
        public List<UserMission> AuthorsMissions { get; set; }

        public List<ChooseUser> ChooseUsers { get; set; }

        public Group Group { get; set; }
         
        public User User { get; set; }
    }
}