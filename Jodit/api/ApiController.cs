using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Jodit.Controllers;
using Jodit.Models;
using Jodit.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jodit.api
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : Controller
    {
        private ApplicationContext db;
        public ApiController(ApplicationContext context)
        {
            db = context;
        }
        
        [HttpGet]
        [Route("GetUserAPI")]
        public User GetUserAPI(string idSession)
        {
            UserSession session = db.UserSessions.FirstOrDefault(x => x.IdSession == idSession);
            if (session != null)
            {
                var user = db.Users.FirstOrDefault(u => u.IdUser == session.UserId);
                
                db.Entry(user)
                    .Collection(c => c.Groups)
                    .Load();

                var executers = db.UserMissions
                    .Include(u => u.Group)  // подгружаем данные по группам
                    .Include(c => c.Author)
                    .Include(a => a.Mission)
                    .Where(c => c.ExecutorId == user.IdUser)
                    .ToList();

                user.Executors = executers;

                return user; 
            }
            return new User();
        }
        
        
            
        [HttpGet]
        [Route("GetListMissionsExecutors")]
        public List<UserMission> GetListMissionsExecutors(string idSession)
        {
            UserSession session = db.UserSessions.FirstOrDefault(x => x.IdSession == idSession);
            if (session != null)
            {
                var user = db.Users.FirstOrDefault(u => u.IdUser == session.UserId);
                var executers = db.UserMissions
                    .Include(u => u.Group)  // подгружаем данные по группам
                    .Include(c => c.Author)
                    .Include(a => a.Mission)
                    .Where(c => c.ExecutorId == user.IdUser)
                    .ToList();
                return executers; 
            }
            return new List<UserMission>();
        }
        
        [HttpGet]
        [Route("GetMissionsExecutors")]
        public List<Mission> GetMissionsExecutors(string idSession)
        {
            UserSession session = db.UserSessions.FirstOrDefault(x => x.IdSession == idSession);
            if (session != null)
            {
                var user = db.Users.FirstOrDefault(u => u.IdUser == session.UserId);
                
                db.Entry(user)
                    .Collection(c => c.Executors)
                    .Load();
                List<Mission> missions = new List<Mission>();
                foreach (var userMission in user.Executors)
                {
                    missions.Add(userMission.Mission);
                }
                
                return missions; 
            }
            return new List<Mission>();
        }
        
        [HttpDelete]
        public void DeleteAPI(int id)
        {
            var user = db.Users.FirstOrDefault(i => i.IdUser == id);
            db.Users.Remove(user);
            db.SaveChanges();
        }

     
        [HttpPost]
        [Route("GetGroupInvitesAPI")]
        public IEnumerable<GroupInvite> GetGroupInvitesAPI(string userEmail)
        {
            User user = db.Users.FirstOrDefault(i => i.Email == userEmail);
            var groupInvites = db.GroupInvites
                .Include(u => u.Group)  // подгружаем данные по группам
                .Include(c => c.InvitingUser)
                .Where(c => c.InvitedUserId == user.IdUser);
            return groupInvites;
        }

        [HttpPost]
        [Route("RegisterAPI")]
        public string RegisterAPI([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User findUser =  db.Users.FirstOrDefault(u => u.Email == model.Email);
                
                if (findUser == null)
                {
                    User user = new User
                    {
                        FirstName = model.FirstName,
                        SecondName = model.SecondName,
                        LastName = model.LastName,
                        Login = model.Login,
                        Phone = model.Phone,
                        Email = model.Email,
                        UserPassword = model.Password
                    };
                    
                    string sessionId = GetRandomString();
                    db.UserSessions.Add(new UserSession() {User = user, IdSession = sessionId});
                    db.Users.Add(user);
                    db.SaveChanges();
                    return sessionId;
                }
            }
            return "";
        }
        
        
        [HttpPost]
        [Route("LoginAPI")]
        public string LoginAPI([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user =  db.Users.FirstOrDefault
                    (u => u.Email == model.Email && u.UserPassword == model.Password);
                if (user != null)
                {
                    UserSession session = db.UserSessions.FirstOrDefault(x => x.UserId == user.IdUser);
                    if (session == null)
                    {
                        string sessionId = GetRandomString();
                        db.UserSessions.Add(new UserSession() {User = user, IdSession = sessionId});
                        db.SaveChanges();
                        return sessionId;
                    }
                }
            }
            return "";
        }
        
        [HttpDelete]
        [Route("LogoutAPI")]
        public bool LogoutAPI(string idSession)
        {
            UserSession session = db.UserSessions.FirstOrDefault(x => x.IdSession == idSession);
            if (session != null)
            {
                db.Entry(session)
                    .Reference(c => c.User)
                    .Load();
                db.UserSessions.Remove(session); 
                db.SaveChanges();
                return true;
            }
            return false;
        }
        
        
        
        [HttpPost]
        [Route("AddGroup")]
        public int AddGroup(string idSession, [FromBody] Group group)
        {
            UserSession session = db.UserSessions.FirstOrDefault(x => x.IdSession == idSession);
            if (group != null && session != null)
            {
                group.DateOfCreation = DateTime.Today;
                var user = db.Users.FirstOrDefault(u => u.IdUser == session.UserId);
               
                db.Groups.Add(group);
                if (user != null)
                {
                    user.UserGroups.Add(new UserGroup {Group = group, IsAdmin = true, User = user});
                }
                db.SaveChanges();
                return group.IdGroup;
            }
            return -1;
        }
        
    
        
           
        [HttpGet]
        [Route("CreateMission")]
        public bool CreateMission(MissionModel model, string idSession)
        {
            UserSession session = db.UserSessions.FirstOrDefault(x => x.IdSession == idSession);
            if (session != null)
            {
                try
                {
                    var author = db.Users.FirstOrDefault(u => u.IdUser == session.UserId);
                    model.Mission.DateOfCreation = DateTime.Now.Date;
                    Group group = db.Groups.FirstOrDefault(i => i.IdGroup == model.Group.IdGroup);

                    if (author != null)
                    {
                        db.Missions.Add(model.Mission);
                        foreach (var chooseUser in model.ChooseUsers)
                        {
                            if (chooseUser.Checkbox.Value)
                            {
                                User executer = db.Users
                                    .FirstOrDefault(i => i.IdUser == chooseUser.User.IdUser);
                                author.Authors.Add(new UserMission()
                                {
                                    Author = author, 
                                    Executor = executer, 
                                    Mission = model.Mission, 
                                    Group = group,
                                    Status =  MissionController.STATUS.PENDING.ToString()
                                });
                            }
                        }
                    }
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {

                }
            }
            return false;
        } 
        
        [HttpGet]
        [Route("TakeMission")]
        public bool TakeMission(int idMission, string idSession)
        {
            UserSession session = db.UserSessions.FirstOrDefault(x => x.IdSession == idSession);
            if (session != null)
            {
                try
                {
                    var user = db.Users.FirstOrDefault(u => u.IdUser == session.UserId);
                
                    var userMission = db.UserMissions
                        .Where(a => a.MissionId == idMission)
                        .FirstOrDefault(a => a.ExecutorId == user.IdUser);
              
                    var a = db.UserMissions
                        .Include(c => c.Executor)
                        .Where(c => c.MissionId == userMission.MissionId).ToList();
              
              
                    var newList = a.Where(x=>x.Executor.IdUser != user.IdUser).ToList();
                    foreach (var um in newList)
                    {
                        db.UserMissions.Remove(um);
                    }
                    userMission.Status = MissionController.STATUS.TAKE.ToString(); 
                    db.SaveChanges();
                }
                catch (Exception ee)
                {
                    return false;
                }
            }
            return true;
        }
        
        [HttpGet]
        [Route("RefuseMission")]
        public bool RefuseMission(int idMission, string idSession)
        {
            UserSession session = db.UserSessions.FirstOrDefault(x => x.IdSession == idSession);
            if (session != null)
            {
                try
                {
                    var user = db.Users.FirstOrDefault(u => u.IdUser == session.UserId);
                    var userMission = db.UserMissions
                        .Where(a => a.MissionId == idMission)
                        .FirstOrDefault(a => a.ExecutorId == user.IdUser);
                    userMission.Status = MissionController.STATUS.REFUSE.ToString(); 
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ee)
                {
                    
                }
            }
            return false;
        }
     
        [HttpGet]
        [Route("ReturnMission")]
        public bool ReturnMission(int idUserMission, string idSession)
        {
            UserSession session = db.UserSessions.FirstOrDefault(x => x.IdSession == idSession);
            if (session != null)
            {
                try
                {
                    var userMission = db.UserMissions
                        .FirstOrDefault(a => a.IdUserMission == idUserMission);
                    userMission.Status = MissionController.STATUS.TAKE.ToString();
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ee)
                {
                    
                }
            }
            return false;
        }
        
        [HttpGet]
        [Route("DeleteMission")]
        public bool DeleteMission(int idUserMission, string idSession)
        {
            UserSession session = db.UserSessions.FirstOrDefault(x => x.IdSession == idSession);
            if (session != null)
            {
                try
                {
                    var userMission = db.UserMissions
                        .FirstOrDefault(a => a.IdUserMission == idUserMission);
                    db.UserMissions.Remove(userMission);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ee)
                {
                    
                }
            }
            return false;
        }
        
        [HttpGet]
        [Route("PassMission")]
        public bool PassMission(int idUserMission, string idSession)
        {
            UserSession session = db.UserSessions.FirstOrDefault(x => x.IdSession == idSession);
            if (session != null)
            {
                try
                {
                    var userMission = db.UserMissions
                        .FirstOrDefault(a => a.IdUserMission == idUserMission);
                    userMission.Status = MissionController.STATUS.PASS.ToString(); 
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ee)
                {
                    
                }
            }
            return false;
        }
        
        [HttpGet]
        [Route("CreateScheduleChange")]
        public bool CreateScheduleChange(ScheduleChangeModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userAfter = db.Users
                        .FirstOrDefault(i => i.IdUser == model.ScheduleChange.AfterUser.IdUser);
                    var userBefore = db.Users
                        .FirstOrDefault(i => i.IdUser == model.ScheduleChange.BeforeUser.IdUser);

                    var statement = db.ScheduleStatements
                        .FirstOrDefault(i => i.IdScheduleStatement == model.IdScheduleStatement);
                 
                    var group = db.Groups
                        .FirstOrDefault(gr => gr.IdGroup == model.ScheduleChange.Group.IdGroup);
                 
                    var selectedValue = Request.Form["chooseDate"];
                    var parsedDate = DateTime.Parse(selectedValue);
                 
                    model.ScheduleChange.AfterUserDate = parsedDate;
                    model.ScheduleChange.BeforeUserDate = statement.ReplacementDate;
                    model.ScheduleChange.BeforeUser = userBefore;
                    model.ScheduleChange.AfterUser = userAfter;
                    model.ScheduleChange.Group = group;
                 
                 
                    group.ScheduleChanges.Add(model.ScheduleChange);
                    db.ScheduleStatements.Remove(statement);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    
                }
            }

            return false;
        }

        [HttpGet]
        [Route("LeaveGroup")]
        public bool LeaveGroup(int idUserGroup)
        {
            if (idUserGroup != null)
            {
                try
                {
                    var userGroup = db.UserGroups
                        .Include(x => x.Group)
                        .Include(x => x.User)
                        .FirstOrDefault(i => i.IdUserGroup == idUserGroup);

                    if (userGroup != null)
                    {
                        db.UserGroups.Remove(userGroup); 
                        db.SaveChangesAsync();
                        return true;
                    }
                }
                catch (Exception e) {}
            }
            return false;
        }
        
        [HttpGet]
        [Route("EditGroup")]
        public bool EditGroup(Group group)
        {
            try
            {
                db.Groups.Update(group); 
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                
            }
            return false;
        }
        
        [HttpGet]
        [Route("DeleteGroup")]
        public bool DeleteGroup(int IdGroup)
        {
            if (IdGroup != null)
            {
                try
                {
                    var group = db.Groups
                        .Include(x => x.Users)
                        .FirstOrDefault(gr => gr.IdGroup == IdGroup);

                    if (group != null)
                    {
                        db.Groups.Remove(group); 
                        db.SaveChanges();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    
                }
            }
            return false;
        }
        
        [HttpGet]
        [ActionName("InviteUser")]
        public bool InviteUser(int idInvitedUser, int idGroup, string idSession)
        {
            UserSession session = db.UserSessions.FirstOrDefault(x => x.IdSession == idSession);
            if (session != null)
            {
                var invitingUser = db.Users.FirstOrDefault(u => u.IdUser == session.UserId);
                
                User invitedUser = db.Users.FirstOrDefault(i => i.IdUser == idInvitedUser);   
                Group group = db.Groups.FirstOrDefault(i => i.IdGroup == idGroup);
             
                var groupInvite = db.GroupInvites
                    .Where(us => us.InvitedUser.IdUser == invitedUser.IdUser)
                    .Where(us => us.InvitingUser.IdUser == invitingUser.IdUser)
                    .FirstOrDefault(gr => gr.GroupId == group.IdGroup);
             
                if (invitingUser != null && invitedUser != null && group != null && groupInvite == null)
                {
                    invitingUser.GroupApplications.Add(new GroupInvite
                    {
                        Group = group, InvitedUser = invitedUser, 
                        InvitingUser = invitingUser, 
                        Title = "Вступай в группу"
                    }); 
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }
        
        public string GetRandomString()
        {
            int [] arr = new int [25]; 
            Random rnd = new Random();
            string str = "";
 
            for (int i=0; i<arr.Length; i++)
            {
                arr[i] = rnd.Next(65,90);
                str += (char) arr[i];
            }
            return str;
        }
    }
    
}