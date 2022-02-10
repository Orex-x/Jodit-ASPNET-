using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Jodit.Models;
using Microsoft.AspNetCore.Authorization;

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
             string key =  KeyGenerator.GetRandomString();
             UserChatID userChatId = new UserChatID()
             {
                 Key = key,
                 User = db.Users.FirstOrDefault(i => i.Email == userName)
             };

             db.UserChatIds.Add(userChatId);
             db.SaveChanges(); 
             
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