using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jodit.Models
{
    [Table("jodit_group")]
    public class Group
    {
        [Key]
        [Column("id_group")]
        public int IdGroup { get; set; }

        [Column("group_name")]
        public string GroupName { get; set; }
        
        [Column("description")]
        public string Description { get; set; }
        
        [Column("date_of_creation")]
        public DateTime DateOfCreation { get; set; }
        
        [Column("is_private")]
        public bool IsPrivate { get; set; }
        
        
        public List<User> Users { get; set; } = new List<User>();
        public List<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
        
        [InverseProperty("Group")]
         public List<GroupInvite> GroupInvites { get; set; }  = new List<GroupInvite>();

        
    }
}