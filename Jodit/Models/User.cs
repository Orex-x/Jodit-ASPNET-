using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jodit.Models
{
    [Table("jodit_user")]
    public class User
    {
        [Key]
        public int IdUser { get; set; }
        
        
        public string FirstName { get; set; }
        
        
        public string SecondName { get; set; }
        
        
        public string LastName { get; set; }
        
        
        public string Login { get; set; }
        
        
        public string Phone { get; set; }
        
        
        public string Email { get; set; }
        
        
        public string UserPassword { get; set; }
        
        
        
        // UserGroup
        public ICollection<Group> Groups { get; set; } = new List<Group>();
        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
        
        
        // GroupInvite
        [InverseProperty("InvitedUser")]
        public ICollection<GroupInvite> GroupInvitations { get; set; }  
        
        [InverseProperty("InvitingUser")]
        public ICollection<GroupInvite> GroupApplications { get; set; }
        
        
        // UserJoditTask
        [InverseProperty("Author")]
        public ICollection<UserMission> Authors { get; set; }
        
        [InverseProperty("Executor")]
        public ICollection<UserMission> Executors { get; set; }
        
        
        // ScheduleChange
        [InverseProperty("BeforeUser")]
        public ICollection<ScheduleChange> ScheduleChangesBeforeUsers { get; set; } 
        
        [InverseProperty("AfterUser")]
        public ICollection<ScheduleChange> ScheduleChangesAfterUsers { get; set; }
        
        
        // ScheduleStatement
        [InverseProperty("BeforeUser")]
        public ICollection<ScheduleStatement> BeforeUsers { get; set; }
   
    }
}