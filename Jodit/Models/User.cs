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
        [Column("id_user")]
        public int IdUser { get; set; }
        
        
        [Column("first_name")]
        public string FirstName { get; set; }
        
        
        [Column("second_name")]
        public string SecondName { get; set; }
        
        
        [Column("last_name")]
        public string LastName { get; set; }
        
        
        [Column("login")]
        public string Login { get; set; }
        
        
        [Column("phone")]
        public string Phone { get; set; }
        
        
        [Column("email")]
        public string Email { get; set; }
        
        
        [Column("user_password")]
        public string UserPassword { get; set; }
        
        
        
        // UserGroup
        public List<Group> Groups { get; set; } = new List<Group>();
        public List<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
        
        
        // GroupInvite
        [InverseProperty("InvitedUser")]
        public List<GroupInvite> GroupInvitations { get; set; } = new List<GroupInvite>();
        
        [InverseProperty("InvitingUser")]
        public List<GroupInvite> GroupApplications { get; set; } = new List<GroupInvite>();
        
        
        // UserJoditTask
        [InverseProperty("Author")]
        public List<UserMission> Authors { get; set; } = new List<UserMission>();
        
        [InverseProperty("Executor")]
        public List<UserMission> Executors { get; set; } = new List<UserMission>();
    }
}