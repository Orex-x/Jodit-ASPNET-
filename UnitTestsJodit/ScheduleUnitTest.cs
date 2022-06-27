using System;
using System.Collections.Generic;
using System.Linq;
using Jodit.Controllers;
using Jodit.Models;
using NUnit.Framework;

namespace UnitTestsJodit;

public class Tests
{
    [Test] 
    public void TestCreateScheduleCount10()
    {
        Group _group = new Group() {
            GroupName = "TestGroup",
            Description = "Description",
            UserGroups = new List<UserGroup>() {
                new UserGroup()
                {
                    User = new User()
                    {
                        FirstName = "Даня",
                        SecondName = "Даня",
                        LastName = "Даня",
                    },
                    IsAdmin = true
                    
                },
                new UserGroup()
                {
                    User = new User()
                    {
                        FirstName = "Саша",
                        SecondName = "Саша",
                        LastName = "Саша",
                    },
                    IsAdmin = false
                    
                },
                new UserGroup()
                {
                    User = new User()
                    {
                        FirstName = "Ваня",
                        SecondName = "Ваня",
                        LastName = "Ваня",
                    },
                    IsAdmin = false
                },
            },
            DateOfCreation = DateTime.Today.AddDays(-20)
        };

        var list = _group.CreateSchedule(DateTime.Now.Date.AddDays(10));
        
        Assert.IsTrue(list.Count == 10);
    }
    
    [Test] 
    public void TestCreateScheduleNotContainedAdmin()
    {
        Group _group = new Group() {
            GroupName = "TestGroup",
            Description = "Description",
            UserGroups = new List<UserGroup>() {
                new UserGroup()
                {
                    User = new User()
                    {
                        FirstName = "Даня",
                        SecondName = "Даня",
                        LastName = "Даня",
                    },
                    IsAdmin = true
                    
                },
                new UserGroup()
                {
                    User = new User()
                    {
                        FirstName = "Саша",
                        SecondName = "Саша",
                        LastName = "Саша",
                    },
                    IsAdmin = false
                    
                },
                new UserGroup()
                {
                    User = new User()
                    {
                        FirstName = "Ваня",
                        SecondName = "Ваня",
                        LastName = "Ваня",
                    },
                    IsAdmin = false
                },
            },
            DateOfCreation = DateTime.Today.AddDays(-20)
        };

        var list = _group.CreateSchedule(DateTime.Now.Date.AddDays(10));

        var item = list.FirstOrDefault(x => x.UserName.Equals("Даня"));

        Assert.IsNull(item);
    }
    
    [Test] 
    public void TestCreateScheduleWithRule()
    {

        User Dana = new User() {
            IdUser = 1,
            FirstName = "Даня",
            SecondName = "Даня",
            LastName = "Даня",
        };
        
        User Vanya = new User() {
            IdUser = 2,
            FirstName = "Ваня",
            SecondName = "Ваня",
            LastName = "Ваня",
        };
        
        User Sasha = new User() {
            IdUser = 3,
            FirstName = "Саша",
            SecondName = "Саша",
            LastName = "Саша",
        };
        
        Group _group = new Group() {
            GroupName = "TestGroup",
            Description = "Description",
            UserGroups = new List<UserGroup>() {
                new UserGroup()
                { 
                    User = Dana,
                    IsAdmin = true
                    
                },
                new UserGroup()
                {
                    User = Sasha,
                    IsAdmin = false
                    
                },
                new UserGroup()
                {
                    User = Vanya,
                    IsAdmin = false
                },
            },
            Rules = new List<Rule>()
            {
                new Rule()
                {
                    User = Sasha,
                    Days = new List<int>() {1}
                }
            },
            DateOfCreation = DateTime.Today.AddDays(-20)
        };

        var list = _group.CreateSchedule(DateTime.Now.Date.AddDays(6));

        var item = list.Where(x => x.UserName.Equals("Саша")).ToList();

        Assert.IsTrue(item.Count == 1);
    }
    
    [Test] 
    public void TestCreateScheduleWithScheduleChange()
    {

        User Dana = new User() {
            IdUser = 1,
            FirstName = "Даня",
            SecondName = "Даня",
            LastName = "Даня",
        };
        
        User Vanya = new User() {
            IdUser = 2,
            FirstName = "Ваня",
            SecondName = "Ваня",
            LastName = "Ваня",
        };
        
        User Sasha = new User() {
            IdUser = 3,
            FirstName = "Саша",
            SecondName = "Саша",
            LastName = "Саша",
        };
        
        Group _group = new Group() {
            GroupName = "TestGroup",
            Description = "Description",
            UserGroups = new List<UserGroup>() {
                new UserGroup()
                { 
                    User = Dana,
                    IsAdmin = true
                    
                },
                new UserGroup()
                {
                    User = Sasha,
                    IsAdmin = false
                    
                },
                new UserGroup()
                {
                    User = Vanya,
                    IsAdmin = false
                },
            },
            DateOfCreation = DateTime.Today.AddDays(-20)
        };

        var list_after = _group.CreateSchedule(DateTime.Now.Date.AddDays(10));

        _group.ScheduleChanges = new List<ScheduleChange>()
        {
            new ScheduleChange()
            {
                AfterUser = Sasha,
                BeforeUser = Vanya,
                AfterUserDate = DateTime.Now.Date.AddDays(2),
                BeforeUserDate = DateTime.Now.Date.AddDays(1)
            }
        };
        
        var list_before = _group.CreateSchedule(DateTime.Now.Date.AddDays(10));

        Assert.IsTrue(!list_after.SequenceEqual(list_before));
    }
    
    [Test] 
    public void TestCreateScheduleWithScheduleChangeSashaToday()
    {

        User Dana = new User() {
            IdUser = 1,
            FirstName = "Даня",
            SecondName = "Даня",
            LastName = "Даня",
        };
        
        User Vanya = new User() {
            IdUser = 2,
            FirstName = "Ваня",
            SecondName = "Ваня",
            LastName = "Ваня",
        };
        
        User Sasha = new User() {
            IdUser = 3,
            FirstName = "Саша",
            SecondName = "Саша",
            LastName = "Саша",
        };
        
        Group _group = new Group() {
            GroupName = "TestGroup",
            Description = "Description",
            UserGroups = new List<UserGroup>() {
                new UserGroup()
                { 
                    User = Dana,
                    IsAdmin = true
                    
                },
                new UserGroup()
                {
                    User = Sasha,
                    IsAdmin = false
                    
                },
                new UserGroup()
                {
                    User = Vanya,
                    IsAdmin = false
                },
                
            },
          
            DateOfCreation = DateTime.Today.AddDays(-20)
        };

        _group.ScheduleChanges = new List<ScheduleChange>()
        {
            new ScheduleChange()
            {
                AfterUser = Sasha,
                BeforeUser = Vanya,
                AfterUserDate = DateTime.Now.Date.AddDays(2),
                BeforeUserDate = DateTime.Now.Date.AddDays(1)
            }
        };

        var list = _group.CreateSchedule(DateTime.Now.Date.AddDays(10));

        var item = list[1];
        
        Assert.IsTrue(item.UserName.Equals("Саша"));
    }
    
    [Test] 
    public void TestCalculateByDateNotNull()
    {
        Group _group = new Group() {
            GroupName = "TestGroup",
            Description = "Description",
            UserGroups = new List<UserGroup>() {
                new UserGroup()
                {
                    User = new User()
                    {
                        FirstName = "Даня",
                        SecondName = "Даня",
                        LastName = "Даня",
                    },
                    IsAdmin = true
                    
                },
                new UserGroup()
                {
                    User = new User()
                    {
                        FirstName = "Саша",
                        SecondName = "Саша",
                        LastName = "Саша",
                    },
                    IsAdmin = false
                    
                },
                new UserGroup()
                {
                    User = new User()
                    {
                        FirstName = "Ваня",
                        SecondName = "Ваня",
                        LastName = "Ваня",
                    },
                    IsAdmin = false
                },
            },
            DateOfCreation = DateTime.Today.AddDays(-20)
        };

        var item = _group.CalculateByDate(DateTime.Now.Date.AddDays(10));
        
        Assert.NotNull(item);
    }
}