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

        

        public UserDateTime CalculateByDate(DateTime date)
        {
            
            var after = ScheduleChanges
                .FirstOrDefault(x => x.AfterUserDate == date);
            var before = ScheduleChanges
                .FirstOrDefault(x => x.BeforeUserDate == date);
            int indexDayOfWeek = (int) date.DayOfWeek;
            var rule = Rules.FirstOrDefault(x => x.Days.Contains(indexDayOfWeek));
            
            
            if (after != null)
            {
                return new UserDateTime()
                {
                    User = after.BeforeUser,
                    UserName = after.BeforeUser.FirstName,
                    DateTime = date
                };
            }
            if (before != null)
            {
                return new UserDateTime()
                {
                    User = after.AfterUser,
                    UserName = after.AfterUser.FirstName,
                    DateTime = date
                };
            }
            if (rule != null)
            {
                return new UserDateTime()
                {
                    User = rule.User,
                    UserName = rule.User.FirstName,
                    DateTime = date
                };
            }
            
            
            
            UsersWithoutRules.Clear();
            foreach (var item in UserGroups)
                if (Rules.FirstOrDefault(x => x.User.IdUser == item.User.IdUser) == null
                    && !item.IsAdmin)
                    UsersWithoutRules.Add(item.User);

            var colStatDays = 0;
            DateTime now = DateTime.Now.Date;
            while (now != date)
            {
                if (Rules.FirstOrDefault(x => x.Days.Contains((int) now.DayOfWeek)) != null)
                    colStatDays++;
                
                now = now.AddDays(1);
            }
            
            var a = (int)(date - DateOfCreation).TotalDays;
            a -= colStatDays;
            var b = a % UsersWithoutRules.Count;
            
            User u = UsersWithoutRules.ToList()[b];
            if (u != null)
            {
                UserDateTime us = new UserDateTime()
                {
                    User = u,
                    UserName = u.FirstName,
                    DateTime = date
                };
                return us;
            }
            return new UserDateTime();
        }


        
        public List<UserDateTime> CreateSchedule(DateTime date)
        {
            var list = new List<UserDateTime>();
            
            DateTime now = DateTime.Now.Date;
            
            
            //лист для хранения пользователей у которых нет правил
            UsersWithoutRules.Clear();
            foreach (var item in UserGroups)
                if (Rules.FirstOrDefault(x => x.User.IdUser == item.User.IdUser) == null
                    && !item.IsAdmin)
                    UsersWithoutRules.Add(item.User);


            if (UsersWithoutRules.Count > 0)
            {
                var userList = new List<User>();
                while (now != date)
                {
                    var i = Calculate(now);
                    User user = UsersWithoutRules.ToList()[i];
                    userList.Add(user);
                    now = now.AddDays(1);
                }
                now = DateTime.Now.Date;
               
                
                for (int i = 0; i < userList.Count; i++, now = now.AddDays(1))
                {
                    var ScheduleChangeAfterUserDate = ScheduleChanges
                        .FirstOrDefault(x => x.AfterUserDate == now);
                    var ScheduleChangeBeforeUserDate = ScheduleChanges
                        .FirstOrDefault(x => x.BeforeUserDate == now);
                    
                    int indexDayOfWeek = (int) now.DayOfWeek;
                    var rule = Rules.FirstOrDefault(x => x.Days.Contains(indexDayOfWeek));
                    if (ScheduleChangeAfterUserDate != null)
                    {
                        list.Add(new UserDateTime()
                        {
                            User = ScheduleChangeAfterUserDate.BeforeUser,
                            UserName = ScheduleChangeAfterUserDate.BeforeUser.FirstName,
                            DateTime = now
                        });
                    }
                    else if (ScheduleChangeBeforeUserDate != null)
                    {
                        list.Add(new UserDateTime()
                        {
                            User = ScheduleChangeBeforeUserDate.AfterUser,
                            UserName = ScheduleChangeBeforeUserDate.AfterUser.FirstName,
                            DateTime = now
                        });
                    }
                    else
                    {
                        if (rule != null)
                        {
                            list.Add(new UserDateTime()
                            {
                                User = rule.User,
                                UserName = rule.User.FirstName,
                                DateTime = now
                            });
                            i--;
                        }
                        else
                        {
                            list.Add(new UserDateTime()
                            {
                                User = userList[i],
                                UserName = userList[i].FirstName,
                                DateTime = now
                            });
                        }
                    }
                    
                }
            }
            else
                return new List<UserDateTime>();
            
            return list;
        }
        
        

        public int Calculate(DateTime date)
        {
            try
            {
                var a = (int)(date - DateOfCreation).TotalDays;
                var b = a % UsersWithoutRules.Count;
                return b;
            }
            catch (DivideByZeroException)
            {
                return 0;
            }
        }

    }
}