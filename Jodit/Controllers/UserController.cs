using System.Linq;
using System.Threading.Tasks;
using Jodit.Models;
using Jodit.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jodit.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private ApplicationContext _db;
        public UserController(ApplicationContext context)
        {
            _db = context;
        }

         public IActionResult ListUsers(int id)
         {
             Group group = _db.Groups
                 .Include(x => x.Users)
                 .FirstOrDefault(i => i.IdGroup == id);
             
             
             UserModel model = new UserModel
             {
                 Users = group.Users,
                 Group = group
             };
             return View(model);
         }
         

         

         public IActionResult InviteUser(int id)
         {
             var userName = User.Identity.Name;
             User user = _db.Users.FirstOrDefault(i => i.Email == userName);
             Group group = _db.Groups.FirstOrDefault(i => i.IdGroup == ApplicationContext.IdCurrentGroup);
             UserModel model = new UserModel
             {
                 Users = _db.Users,
                 Group = group,
                 User = user
             };
             return View(model);
         }
         
         [HttpGet]
         [ActionName("InviteUserFinal")]
         public async Task<IActionResult> InviteUser(int idUser, int idGroup)
         {
             User invitingUser = _db.Users.FirstOrDefault(i => i.Email == User.Identity.Name); 
             User invitedUser = _db.Users.FirstOrDefault(i => i.IdUser == idUser);   
             Group group = _db.Groups.FirstOrDefault(i => i.IdGroup == idGroup);
             
             var groupInvite = _db.GroupInvites
                 .Where(us => us.InvitedUser.IdUser == invitedUser.IdUser)
                 .Where(us => us.InvitingUser.IdUser == invitingUser.IdUser)
                 .FirstOrDefault(gr => gr.Group.IdGroup == group.IdGroup);
             
             if (invitingUser != null && invitedUser != null && group != null && groupInvite == null)
             {
                 invitingUser.GroupApplications.Add(new GroupInvite
                 {
                     Group = group, InvitedUser = invitedUser, 
                     InvitingUser = invitingUser, 
                     Title = "Вступай в группу"
                 });
                 await _db.SaveChangesAsync();
             }

             return RedirectToAction("InviteUser", idGroup);
         }
    }
}