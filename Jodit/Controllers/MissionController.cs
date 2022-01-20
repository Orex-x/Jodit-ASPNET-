using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jodit.Models;
using Jodit.ViewModels;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jodit.Controllers
{
    public class MissionController : Controller
    {
        public enum STATUS
        {
            PENDING,
            REFUSE,
            TAKE,
            PASS
            
        }
        
        private ApplicationContext db;
          public MissionController(ApplicationContext context)
          {
              db = context;
          }
          
          
          public IActionResult CreateMission(int id)
          {
              var userName = User.Identity.Name;
              User mainUser = db.Users.FirstOrDefault(i => i.Email == userName);
              Group group = db.Groups.FirstOrDefault(i => i.IdGroup == id);
             
              db.Entry(group)
                  .Collection(c => c.Users)
                  .Load();
              
              List<ChooseUser> list = new List<ChooseUser>();
              foreach (var user in group.Users)
              {
                  if (user.IdUser != mainUser.IdUser)
                  {
                      list.Add(new ChooseUser(){User = user, Checkbox = new InputCheckbox(){Value = false}});
                  }
              } 
              MissionModel model = new MissionModel
              {
                  ChooseUsers = list,
                  Group = group,
                  User = mainUser
              };
              
              return View(model);  
          } 
          
          [HttpPost]
          public async Task<IActionResult> CreateMission(MissionModel model)
          {
              if (ModelState.IsValid)
              {
                  var userName = User.Identity.Name;
                  User author = db.Users.FirstOrDefault(i => i.Email == userName);
                  model.Mission.DateOfCreation = DateTime.Now.Date;
                  Group group = db.Groups.FirstOrDefault(i => i.IdGroup == model.Group.IdGroup);

                  if (author != null)
                  {
                      db.Missions.Add(model.Mission);
                      foreach (var chooseUser in model.ChooseUsers)
                      {
                          if (chooseUser.Checkbox.Value)
                          {
                              User executer = db.Users.FirstOrDefault(i => i.IdUser == chooseUser.User.IdUser);
                              author.Authors.Add(new UserMission()
                              {
                                  Author = author, 
                                  Executor = executer, 
                                  Mission = model.Mission, 
                                  Group = group,
                                  Status =  STATUS.PENDING.ToString()
                              });
                          }
                      }
                  }
                  await db.SaveChangesAsync();
                  return RedirectToAction("ListGroups", "Group");
                 
              }
              return RedirectToAction("ListGroups", "Group");
          }
          
          public IActionResult ListMissions()
          {
              var userName = User.Identity.Name;
              User mainUser = db.Users.FirstOrDefault(i => i.Email == userName);
          
              
              var executorsMissions = db.UserMissions
                  .Include(c => c.Group)
                  .Include(c => c.Mission)
                  .Include(c => c.Author)
                  .Include(c => c.Executor)
                  .Where(c => c.Executor.IdUser == mainUser.IdUser)
                  .ToList();
              
              
              var b = db.UserMissions
                  .Include(c => c.Group)
                  .Include(c => c.Mission)
                  .Include(c => c.Author)
                  .Include(c => c.Executor)
                  .Where(c => c.Author.IdUser == mainUser.IdUser)
                  .ToList();

              var authorsMissions = new List<UserMission>();
              foreach (var authorsMission in b)
                  if (authorsMissions.FirstOrDefault(x => x.MissionId == authorsMission.MissionId) == null)
                      authorsMissions.Add(authorsMission);

              MissionModel model = new MissionModel
              {
                  authorsMissions = authorsMissions,
                  executorsMissions = executorsMissions
              };
              
              return View(model);  
          } 
          
          public IActionResult ListExecutors(int id)
          {
              var a = db.UserMissions
                  .Include(c => c.Executor)
                  .Where(c => c.MissionId == id).ToList();
              
              List<User> executors = new List<User>();
              foreach (var userMission in a)
              {
                  executors.Add(userMission.Executor);
              }
              
              UserModel model = new UserModel()
              {
                  Users = executors
              };
              
              return View("../User/ListUsers", model);
          }
          
          public IActionResult ListUserMissions(int id)
          {
              var a = db.UserMissions
                  .Include(c => c.Executor)
                  .Where(c => c.MissionId == id).ToList();
              return View(a);
          } 
          
          public async Task<IActionResult> RefuseMission(int idUserMission)
          {
              var userMission = db.UserMissions.FirstOrDefault(a => a.IdUserMission == idUserMission);
              userMission.Status = STATUS.REFUSE.ToString(); 
              await db.SaveChangesAsync();

              return RedirectToAction("ListMissions", "Mission");
          }
          
          public async Task<IActionResult> PassMission(int idUserMission)
          {
              var userMission = db.UserMissions.FirstOrDefault(a => a.IdUserMission == idUserMission);
              userMission.Status = STATUS.PASS.ToString(); 
              await db.SaveChangesAsync();

              return RedirectToAction("ListMissions", "Mission");
          }
          
          public async Task<IActionResult> deleteMission(int idUserMission)
          {
              var userMission = db.UserMissions.FirstOrDefault(a => a.IdUserMission == idUserMission);
              db.UserMissions.Remove(userMission);
              await db.SaveChangesAsync();
              return RedirectToAction("ListMissions", "Mission");
          }
          
          public async Task<IActionResult> returnMission(int idUserMission)
          {
              var userMission = db.UserMissions.FirstOrDefault(a => a.IdUserMission == idUserMission);
              userMission.Status = STATUS.TAKE.ToString(); 
              await db.SaveChangesAsync();
              return RedirectToAction("ListMissions", "Mission");
          }
        
          
          public async Task<IActionResult> TakeMission(int idUserMission)
          {
              var userName = User.Identity.Name;
              User user = db.Users.FirstOrDefault(i => i.Email == userName);
              var userMission = db.UserMissions.FirstOrDefault(a => a.IdUserMission == idUserMission);
              
              var a = db.UserMissions
                  .Include(c => c.Executor)
                  .Where(c => c.MissionId == userMission.MissionId).ToList();
              
              
              var newList = a.Where(x=>x.Executor.IdUser != user.IdUser).ToList();
              foreach (var um in newList)
              {
                  db.UserMissions.Remove(um);
              }
              userMission.Status = STATUS.TAKE.ToString(); 
              await db.SaveChangesAsync();
           
              return RedirectToAction("ListMissions", "Mission");
          }
        
    }
}