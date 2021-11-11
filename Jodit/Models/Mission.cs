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
        [Column("id_mission")]
        public int IdMission { get; set; }
        
        [Column("title")]
        public string Title { get; set; }
        
        [Column("description")]
        public string Description { get; set; }
        
        [Column("deadline")]
        public DateTime Deadline { get; set; }
        
        [Column("date_of_creation")]
        public DateTime DateOfCreation { get; set; }
        
        [InverseProperty("Mission")]
        public List<UserMission> UserMissions { get; set; }  = new List<UserMission>();
    }
}