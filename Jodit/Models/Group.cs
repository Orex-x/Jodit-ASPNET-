using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
        
        public virtual List<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
        
        public virtual ICollection<ScheduleChange> ScheduleChanges { get; set; }
        
        public virtual ICollection<ScheduleStatement> ScheduleStatements { get; set; }


        public ArrayList CalculateToDate(DateTime date)
        {
            ArrayList list = new ArrayList();
            DateTime now = DateTime.Now.Date;
            while (now != date)
            {
                var i = Calculate(now);
                User user = Users.ToList()[i];
                //list.Add("Date: " + now.ToShortDateString() + " user: " + user.FirstName + " " + user.SecondName);
                list.Add(new UserDateTime() { User = user, DateTime = now, userName = user.FirstName });
                now = now.AddDays(1);
            }
            return list;
        }

        public UserDateTime CalculateByDate(DateTime date)
        {
            var i = Calculate(date);
            User user = Users.ToList()[i];
            return new UserDateTime() { User = user, DateTime = date };
            //  return new Dictionary<DateTime, User> { {date, user} };
            //  return "Date: " + date.ToShortDateString() + " user: " + user.FirstName + " " + user.SecondName;
        }

        public int Calculate(DateTime date)
        {
            // Считаю разницу в днях 
            var a = (int)(date - DateOfCreation).TotalDays;
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