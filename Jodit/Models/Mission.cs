using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jodit.Models
{
    [Table("jodit_mission")]
    public class Mission
    {
        [Key]
        public int IdMission { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public DateTime Deadline { get; set; }
        
        public DateTime DateOfCreation { get; set; }
        
        public ICollection<UserMission> UserMissions { get; set; } 
    }
}