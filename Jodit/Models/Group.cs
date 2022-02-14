using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jodit.Models
{
    [Table("jodit_group")]
    public class Group
    {
        [Key]
        public int IdGroup { get; set; }
        
        public string GroupName { get; set; }
        
        public string Description { get; set; }
        
        public DateTime DateOfCreation { get; set; }
        
        public bool IsPrivate { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
        
        [NotMapped]
        public virtual ICollection<User> UsersWithoutRules { get; set; } = new List<User>();
        
        public virtual List<UserGroup> UserGroups { get; set; } = new List<UserGroup>(); 
        
        public virtual List<Rule> Rules { get; set; } = new List<Rule>();

        public virtual ICollection<ScheduleChange> ScheduleChanges { get; set; } = new List<ScheduleChange>();

        public virtual ICollection<ScheduleStatement> ScheduleStatements { get; set; } = new List<ScheduleStatement>();


        public ArrayList CalculateToDate(DateTime date)
        {
            ArrayList list = new ArrayList();
            DateTime now = DateTime.Now.Date;
            
            UsersWithoutRules.Clear();

            foreach (var item in Users)
            {
                if (Rules.FirstOrDefault(x => x.User.IdUser == item.IdUser) == null)
                {
                    UsersWithoutRules.Add(item);
                }
            }
            
            while (now != date)
            {
                var i = Calculate(now);
                User user = UsersWithoutRules.ToList()[i];
                list.Add(new UserDateTime() { User = user, DateTime = now, userName = user.FirstName });
                now = now.AddDays(1);
            }
            return list;
        }

        public UserDateTime CalculateByDate(DateTime date)
        {
            UsersWithoutRules.Clear();

            foreach (var item in Users)
            {
                if (Rules.FirstOrDefault(x => x.User.IdUser == item.IdUser) == null)
                {
                    UsersWithoutRules.Add(item);
                }
            }

            var i = Calculate(date);
            User user = UsersWithoutRules.ToList()[i];
            return new UserDateTime() { User = user, DateTime = date };
        }

        public int Calculate(DateTime date)
        {
            // Считаю разницу в днях 
            var a = (int)(date - DateOfCreation).TotalDays;
            // Считаю n полных циклов дежурств прошло с момента создания группы
            var b = a / UsersWithoutRules.Count;
            // Считаю количесвто дней, необходимых для прохождения n полных циклов
            var c = b * UsersWithoutRules.Count;
            //нахожу разницу
            var d = a - c;
            //возвращаю индекс пользователя
            return d;
        }

    }
}