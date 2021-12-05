using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Jodit.api;
using Jodit.Models;
using Jodit.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jodit.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationContext db;
        public AccountController(ApplicationContext context)
        {
            db = context;
        }
        
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync
                    (u => u.Email == model.Email && u.UserPassword == model.Password);
                if (user != null)
                {
                    await Authenticate(model.Email); // аутентификация
 
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }*/
        
        
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
       [HttpPost]
       [ValidateAntiForgeryToken]
       public async Task<IActionResult> Register(RegisterModel model)
       {
           api.AccountController controller = new api.AccountController(db);
           var idUser =  controller.Post(model);
           if (idUser != null)
           {
               User user = await db.Users.FirstOrDefaultAsync(u => u.IdUser == idUser);
               await Authenticate(user.Email); 
               return RedirectToAction("Account", "Account");
           }
           return View(model);
       }

       [HttpPost]
       [ValidateAntiForgeryToken]
       public async Task<IActionResult> Login(LoginModel model)
       {
           if (ModelState.IsValid)
           {
               User user =  db.Users.FirstOrDefault
                   (u => u.Email == model.Email && u.UserPassword == model.Password);
               if (user != null)
               {
                   await Authenticate(user.Email); 
                   return RedirectToAction("Account", "Account");
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
            User user = db.Users.Where(i => i.Email == userName).FirstOrDefault();
            
            AccountModel model = new AccountModel()
            {
                user = user
            };
            return View(model);
        }
        
        [Authorize]
        public IActionResult GroupInvitations()
        { 
            /*var userName = User.Identity.Name;
            User user = db.Users.FirstOrDefault(i => i.Email == userName);
            
           
           var groupInvites = db.GroupInvites
               .Include(u => u.Group)  // подгружаем данные по группам
               .Include(c => c.InvitingUser)
               .Where(c => c.InvitedUserId == user.IdUser) 
               .ToList();
            
           
           user.GroupInvitations = groupInvites;
           AccountModel model = new AccountModel()
            {
                user = user
            };*/
            var userEmail = User.Identity.Name;
            api.AccountController controller = new api.AccountController(db);
            var list = controller.GetGroupInvites(userEmail).ToList();
            return View(list);
        }

        
        [Authorize]
        public async Task<IActionResult> AcceptGroupInvitations(int idGroupInvitations)
        {
            api.AccountController controller = new api.AccountController(db);
            controller.GroupInvitationsAccept(idGroupInvitations);
            return RedirectToAction("GroupInvitations", "Account");
        }
        
        [Authorize]
        public async Task<IActionResult> RefuseGroupInvitations(int idGroupInvitations)
        {
            api.AccountController controller = new api.AccountController(db);
            controller.GroupInvitationsRefuse(idGroupInvitations);
            return RedirectToAction("GroupInvitations", "Account");
        }
    }
}