using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Jodit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Jodit.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext db;
        public HomeController(ApplicationContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }
        
        public IActionResult TelegramBot()
        {
             var userName = User.Identity.Name;
             var user = db.Users.FirstOrDefault(i => i.Email == userName);
             
             var UserChat = db.UserChatIds
                 .Include(x => x.User)
                 .FirstOrDefault(i => i.User.IdUser == user.IdUser);
             
             string key = KeyGenerator.GetRandomString();
            
             
             if (UserChat == null)
             {
                 UserChatID userChatId = new UserChatID()
                 {
                     Key = key,
                     User = user
                 };
                 db.UserChatIds.Add(userChatId);
                 db.SaveChanges();
             }
             else
             {
                 UserChat.Key = key;
                 db.UserChatIds.Update(UserChat);
                 db.SaveChanges();
             }


             ViewData["key"] = key;
             return View();
        }
        
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}