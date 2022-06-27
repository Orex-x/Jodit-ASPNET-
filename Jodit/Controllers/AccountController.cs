using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Jodit.Models;
using Jodit.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jodit.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationContext _db;
        public AccountController(ApplicationContext context)
        {
            _db = context;
        }
        
        
        public IActionResult Login()
        {
            return View();
        }
        
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
       {
           if (ModelState.IsValid)
           {
               User findUser =  _db.Users.FirstOrDefault(u => u.Email == model.Email);
                
               if (findUser == null)
               {
                   if (model.Password.Length > 7)
                   {
                       var hasher = new PasswordHasher<User>();
                       User user = new User
                       {
                           FirstName = model.FirstName,
                           SecondName = model.SecondName,
                           LastName = model.LastName,
                           Login = model.Login,
                           Phone = model.Phone,
                           Email = model.Email,
                       };
                       user.UserPassword = hasher.HashPassword(user, model.Password);
                       await Authenticate(user.Email);
                   
                   
                       _db.Users.Add(user);
                       _db.SaveChanges();
                       return RedirectToAction("Account", "Account");
                   } 
                   ModelState.AddModelError("", "Пароль должен содержать минимум 8 символов");

               }
           }
           return View(model);
       }
       
        [HttpPost]
       public async Task<IActionResult> Login(LoginModel model)
       {
           if (ModelState.IsValid)
           {
               User user =  _db.Users.FirstOrDefault(u => u.Email == model.Email);
               if (user != null)
               {
                   var hasher = new PasswordHasher<User>();
                   var s = hasher
                       .VerifyHashedPassword(user, user.UserPassword, model.Password);
                   
                    if (s == PasswordVerificationResult.Success)
                    {
                        await Authenticate(user.Email); 
                        return RedirectToAction("Account", "Account");
                    }
               }
               ModelState.AddModelError("", "Некорректные логин и(или) пароль");
           }
           return View(model);
       }
       
      private async Task Authenticate(string userName)
       {
           // создаем один claim
           var claims = new List<Claim>
           {
               new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
           };
           // создаем объект ClaimsIdentity
           ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie",
               ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
           // установка аутентификационных куки
           await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
       }
        
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
        
        [Authorize]
        public IActionResult Account()
        {
            var userName = User.Identity.Name;
            User user = _db.Users
                .Include(x => x.UserGroups)
                .ThenInclude(x => x.Group)
                .FirstOrDefault(u => u.Email == userName);
                
            AccountModel model = new AccountModel()
            {
                User = user
            };
            return View(model);
        }
        
        [Authorize]
        public IActionResult GroupInvitations()
        { 
            var userName = User.Identity.Name;
            User user = _db.Users.FirstOrDefault(u => u.Email == userName);
            
           
           var groupInvites = _db.GroupInvites
               .Include(u => u.Group)  // подгружаем данные по группам
               .Include(c => c.InvitingUser)
               .Where(c => c.InvitedUser.IdUser == user.IdUser) 
               .ToList();

           return View(groupInvites);
        }

        
        [Authorize]
        public async Task<IActionResult> AcceptGroupInvitations(int idGroupInvitations)
        {
            GroupInvite groupInvite = _db.GroupInvites
                .Include(x => x.Group)
                .Include(x => x.InvitingUser)
                .Include(x => x.InvitedUser)
                .FirstOrDefault(gi => gi.IdGroupInvite == idGroupInvitations);
            
            if (groupInvite != null)
            {
                groupInvite.InvitedUser.UserGroups.Add(new UserGroup
                {
                    Group = groupInvite.Group, 
                    IsAdmin = false, 
                    User = groupInvite.InvitedUser
                });
                _db.GroupInvites.Remove(groupInvite);
                _db.SaveChanges();
            }
            
            return RedirectToAction("GroupInvitations", "Account");
        }
        
        [Authorize]
        public async Task<IActionResult> RefuseGroupInvitations(int idGroupInvitations)
        {
            GroupInvite groupInvite = _db.GroupInvites
                .Include(x => x.Group)
                .Include(x => x.InvitingUser)
                .Include(x => x.InvitedUser)
                .FirstOrDefault(gi => gi.IdGroupInvite == idGroupInvitations);
           
            if (groupInvite != null)
            {
                _db.GroupInvites.Remove(groupInvite);
                _db.SaveChangesAsync();
            }
            return RedirectToAction("GroupInvitations", "Account");
        }
    }
}