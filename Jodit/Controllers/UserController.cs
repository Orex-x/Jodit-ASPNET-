using System.Linq;
using System.Threading.Tasks;
using Jodit.Models;
using Jodit.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Jodit.Controllers
{
    public class UserController : Controller
    {
        private ApplicationContext db;
        public UserController(ApplicationContext context)
        {
            db = context;
        }

         public IActionResult ListUsers(int id)
         {
             Group group = db.Groups.FirstOrDefault(i => i.IdGroup == id);
             
             db.Entry(group)
                .Collection(c => c.Users)
                .Load();
             
             UserModel model = new UserModel
             {
                 Users = group.Users
             };
             return View(model);
         }
         

         
         [HttpGet]
         public IActionResult InviteUser(int id)
         {
             var userName = User.Identity.Name;
             User user = db.Users.FirstOrDefault(i => i.Email == userName);
             Group group = db.Groups.FirstOrDefault(i => i.IdGroup == id);
             UserModel model = new UserModel
             {
                 Users = db.Users,
                 Group = group,
                 User = user
             };
             return View(model);
         }
         
         [HttpGet]
         [ActionName("InviteUserFinal")]
         public async Task<IActionResult> InviteUser(int idUser, int idGroup)
         {
             User invitingUser = db.Users.FirstOrDefault(i => i.Email == User.Identity.Name); 
             User invitedUser = db.Users.FirstOrDefault(i => i.IdUser == idUser);   
             Group group = db.Groups.FirstOrDefault(i => i.IdGroup == idGroup);
             
             var foo = db.GroupInvites
                 .Where(us => us.InvitedUser.IdUser == invitedUser.IdUser)
                 .Where(us => us.InvitingUser.IdUser == invitingUser.IdUser)
                 .FirstOrDefault(gr => gr.GroupId == group.IdGroup);
             
             if (invitingUser != null && invitedUser != null && group != null && foo == null)
             {
                 invitingUser.GroupApplications.Add(new GroupInvite
                 {
                     Group = group, InvitedUser = invitedUser, 
                     InvitingUser = invitingUser, 
                     Title = "Вступай в группу"
                 });
                 await db.SaveChangesAsync();
             }
             return RedirectToAction("ListGroups", "Group");
         }
    }
}