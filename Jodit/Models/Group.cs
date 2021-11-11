using System;
using System.Collections;
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
         
         [InverseProperty("Group")]
         public List<UserMission> UserMissions { get; set; }  = new List<UserMission>();

         
         public ArrayList CalculateToDate(DateTime date)
         {
             ArrayList list = new ArrayList();
             DateTime now = DateTime.Now.Date;
             while (now != date)
             {
                 var i = Calculate(now);
                 User user = Users[i];
                 list.Add("Date: " + now.ToShortDateString() + " user: " + user.FirstName + " " + user.SecondName);
                 now = now.AddDays(1);
             }
             return list;
         }

         public string CalculateByDate(DateTime date)
         {
             var i = Calculate(date);
             User user = Users[i];
             return "Date: " + date.ToShortDateString() + " user: " + user.FirstName + " " + user.SecondName;
         }
        
         public int Calculate(DateTime date)
         {
             // Считаю разницу в днях 
             var a = (int) (date - DateOfCreation).TotalDays;
             // Считаю n полных циклов дежурств прошло с момента создания группы
             var b = a / Users.Count;
             // Считаю количесвто дней, необходимых для прохождения n полных циклов
             var c = b * Users.Count;
             //нахожу разницу
             var d = a - c;
             return d;
         }
    }
}