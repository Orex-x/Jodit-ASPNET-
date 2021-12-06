using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Jodit.Models;
using Jodit.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jodit.api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private ApplicationContext db;
        public AccountController(ApplicationContext context)
        {
            db = context;
        }
        
        [HttpGet]
        [Route("GetUser")]
        public User GetUser(string idSession)
        {
            UserSession session = db.UserSessions.FirstOrDefault(x => x.IdSession == idSession);
            if (session != null)
            {
                User user = db.Users.FirstOrDefault(i => i.IdUser == session.UserId);
                return user;
            }
            return new User();
        }
        
        
        [HttpGet]
        [Route("GetGroups")]
        public IEnumerable<Group> GetGroups(string idSession)
        {
            UserSession session = db.UserSessions.FirstOrDefault(x => x.IdSession == idSession);
            if (session != null)
            {
                User user = db.Users.FirstOrDefault(i => i.IdUser == session.UserId);
               
                db.Entry(user)
                    .Collection(c => c.Groups)
                    .Load();
                
                return user.Groups;
            }
            return new List<Group>();
        }
        
     
        
        
        [HttpGet]
        [Route("GetUserGroup")]
        public IEnumerable<UserGroup> GetUserGroup(string idSession)
        {
            UserSession session = db.UserSessions.FirstOrDefault(x => x.IdSession == idSession);
            if (session != null)
            {
                User user = db.Users.FirstOrDefault(i => i.IdUser == session.UserId);
                var userGroups = db.UserGroups
                    .Include(g => g.Group)
                    .Where(i => i.UserId == user.IdUser).ToList();
                return userGroups;
            }
            return new List<UserGroup>();
        }
        
        [HttpDelete]
        [Route("DeleteUser")]
        public void Delete(string idSession, int id)
        {
            UserSession session = db.UserSessions.FirstOrDefault(x => x.IdSession == idSession);
            if (session != null)
            {
                var user = db.Users.FirstOrDefault(i => i.IdUser == id);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }
        
        [HttpDelete]
        [Route("CloseSession")]
        public void Delete(string idSession)
        {
            UserSession session = db.UserSessions.FirstOrDefault(x => x.IdSession == idSession);
            if (session != null)
            {
                db.UserSessions.Remove(session);
                db.SaveChanges();
            }
        }

        [HttpDelete]
        [Route("GroupInvitationsRefuseAPI")]
        public void GroupInvitationsRefuse(string idSession, int id)
        {
           
        }
        
        [HttpPut]
        [Route("GroupInvitationsAcceptAPI")]
        public void GroupInvitationsAccept(string idSession, int id)
        {
           
        }
        
        [HttpPost]
        [Route("GetGroupInvites")]
        public IEnumerable<GroupInvite> GetGroupInvites(string userEmail)
        {
            User user = db.Users.FirstOrDefault(i => i.Email == userEmail);
            var groupInvites = db.GroupInvites
                .Include(u => u.Group)  // подгружаем данные по группам
                .Include(c => c.InvitingUser)
                .Where(c => c.InvitedUserId == user.IdUser);
            return groupInvites;
        }

        [HttpPost]
        public int Post([FromBody] RegisterModel model)
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
                    db.Users.Add(user);
                    db.SaveChanges();
                    return user.IdUser;
                }
            }
            return 0;
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
       
        public string GetRandomString()
        {
            int [] arr = new int [30]; 
            Random rnd = new Random();
            string str = "";
 
            for (int i=0; i<arr.Length; i++)
            {
                arr[i] = rnd.Next(33,125);
                str += (char) arr[i];
            }
            return str;
        }
    }
}