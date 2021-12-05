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
                [Route("GetUsers")]
                public IEnumerable<User> Get(string idSession)
                {
                    UserSession session = db.UserSessions.FirstOrDefault(x => x.IdSession == idSession);
                    if (session != null)
                    {
                        var users = db.Users.ToList();
                        return users;
                    }
                    return new List<User>();
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
        public void Delete(int id)
        {
            var user = db.Users.FirstOrDefault(i => i.IdUser == id);
            db.Users.Remove(user);
            db.SaveChanges();
        }

        [HttpDelete]
        public void GroupInvitationsRefuse(int id)
        {
            GroupInvite groupInvite = db.GroupInvites.FirstOrDefault(i => i.IdGroupInvite == id);
            db.Entry(groupInvite)
                .Reference(c => c.Group)
                .Load();
            
            db.Entry(groupInvite)
                .Reference(c => c.InvitingUser)
                .Load();
            
            db.Entry(groupInvite)
                .Reference(c => c.InvitedUser)
                .Load();

            if (groupInvite != null)
            {
                db.GroupInvites.Remove(groupInvite);
            }
            db.SaveChangesAsync();
        }
        
        [HttpPut]
        public void GroupInvitationsAccept(int id)
        {
            GroupInvite groupInvite = db.GroupInvites.FirstOrDefault(i => i.IdGroupInvite == id);
            db.Entry(groupInvite)
                .Reference(c => c.Group)
                .Load();
            
            db.Entry(groupInvite)
                .Reference(c => c.InvitingUser)
                .Load();
            
            db.Entry(groupInvite)
                .Reference(c => c.InvitedUser)
                .Load();
            if (groupInvite != null)
            {
                groupInvite.InvitedUser.UserGroups.Add(new UserGroup {Group = groupInvite.Group, 
                    IsAdmin = false, User = groupInvite.InvitedUser});
                db.GroupInvites.Remove(groupInvite);
            }
            db.SaveChanges();
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
        [Route("Login")]
        public string Login([FromBody] LoginModel model)
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
                    ModelState.AddModelError("", "Сессия уже существует");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return "";
        }
       
        public string GetRandomString()
        {
            int [] arr = new int [25]; 
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