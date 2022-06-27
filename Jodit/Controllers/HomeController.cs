using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Jodit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Jodit.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext _db;
        public HomeController(ApplicationContext context)
        {
            _db = context;
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
             var user = _db.Users.FirstOrDefault(i => i.Email == userName);
             
             var userChat = _db.UserChatIds
                 .Include(x => x.User)
                 .FirstOrDefault(i => i.User.IdUser == user.IdUser);
             
             string key = KeyGenerator.GetRandomString();
            
             
             if (userChat == null)
             {
                 UserChatId userChatId = new UserChatId()
                 {
                     Key = key,
                     User = user
                 };
                 _db.UserChatIds.Add(userChatId);
                 _db.SaveChanges();
             }
             else
             {
                 userChat.Key = key;
                 _db.UserChatIds.Update(userChat);
                 _db.SaveChanges();
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