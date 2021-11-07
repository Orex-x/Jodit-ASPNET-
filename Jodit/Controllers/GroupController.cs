using System;
using System.Linq;
using System.Threading.Tasks;
using Jodit.Models;
using Jodit.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jodit.Controllers
{
    public class GroupController : Controller
    {
        private ApplicationContext db;
        public GroupController(ApplicationContext context)
        {
            db = context;
        }
        
        
       

        public IActionResult ListGroups()
        {
            
            /*var userName = User.Identity.Name;
            User user = db.Users.Where(i => i.Email == userName).FirstOrDefault();

            
            // Загрузить связанные с ним заказы с помощью явной загрузки
            db.Entry(user)
                .Collection(c => c.Groups)
                .Load();
            
            GroupModel accountModel = new GroupModel
           {
               Groups = user.Groups
           };*/
            var userName = User.Identity.Name;
                User user = db.Users.FirstOrDefault(i => i.Email == userName);
                var userGroups = db.UserGroups
                    .Include(g => g.Group)
                    .Where(i => i.UserId == user.IdUser).ToList();
                
                
            GroupModel accountModel = new GroupModel
            {
                UserGroups = userGroups
            };
            return View(accountModel);
            
        }

        public IActionResult CreateGroup() => View();
        
         [HttpPost]
         public async Task<IActionResult> CreateGroup(Group group)
         {
             group.DateOfCreation = DateTime.Today;
             var userName = User.Identity.Name;
             User user = db.Users.FirstOrDefault(i => i.Email == userName);
             
             db.Groups.Add(group);
             if (user != null)
             {
                 user.UserGroups.Add(new UserGroup {Group = group, IsAdmin = true, User = user});
             }

             await db.SaveChangesAsync();
              return RedirectToAction("ListGroups", "Group");
         }
         
         public async Task<IActionResult> Details(int? id)
         {
             if (id != null)
             {
                 var userName = User.Identity.Name;
                 User user = db.Users.FirstOrDefault(i => i.Email == userName);
                 await db.Groups.Where(gr => gr.IdGroup == id).FirstOrDefaultAsync();
                 var userGroup = await db.UserGroups
                     .Where(i => i.GroupId == id)
                     .FirstOrDefaultAsync(i => i.UserId == user.IdUser);

                 if (userGroup != null)
                 {
                     return View(userGroup);
                 }
             }
             return NotFound();
         }
         
         
         public async Task<IActionResult> LeaveGroup(int? idUserGroup)
         {
             if (idUserGroup != null)
             {
                 var userGroup = await db.UserGroups.FirstOrDefaultAsync(i => i.IdUserGroup == idUserGroup);

                 if (userGroup != null)
                 {
                     db.Entry(userGroup)
                         .Reference(c => c.Group)
                         .Load();
            
                     db.Entry(userGroup)
                         .Reference(c => c.User)
                         .Load();

                     db.UserGroups.Remove(userGroup);
                     await db.SaveChangesAsync();
                     return RedirectToAction("ListGroups", "Group");
                 }
             }
             return NotFound();
         }
         
         
         public async Task<IActionResult> Edit(int? id)
         {
             if (id != null)
             {
                 var group = await db.Groups.FirstOrDefaultAsync(gr => gr.IdGroup == id);
                 if (group != null)
                 {
                     return View(group);
                 }
             }
             return NotFound();
         }

         [HttpPost]
         public async Task<IActionResult> Edit(Group group)
         {
             db.Groups.Update(group);
             await db.SaveChangesAsync();
             return RedirectToAction("ListGroups");
         }
         
         [HttpGet]
         [ActionName("Delete")]
         public async Task<IActionResult> ConfirmDelete(int? id)
         {
             if (id != null)
             {
                 var group = await db.Groups.FirstOrDefaultAsync(gr => gr.IdGroup == id);
                 if (group != null)
                     return View(group);
             }
             return NotFound();
         }
         
         [HttpPost]
         public async Task<IActionResult> Delete(int? id)
         {
             if (id != null)
             {
                 var group = await db.Groups.FirstOrDefaultAsync(gr => gr.IdGroup == id);

                 if (group != null)
                 {
                     
                     db.Entry(group)
                         .Collection(c => c.Users)
                         .Load();
                     
                     
                     db.Groups.Remove(group);
                     await db.SaveChangesAsync();
                     return RedirectToAction("ListGroups");
                 }
             }
             return NotFound();
         }
    }
}