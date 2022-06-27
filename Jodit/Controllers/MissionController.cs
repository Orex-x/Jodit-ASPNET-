using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jodit.Models;
using Jodit.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jodit.Controllers
{
    [Authorize]
    public class MissionController : Controller
    {
        public enum Status
        {
            Pending,
            Refuse,
            Take,
            Pass
            
        }
        
        private ApplicationContext _db;
          public MissionController(ApplicationContext context)
          {
              _db = context;
          }
          
          
          public IActionResult CreateMission(int id)
          {
              var userName = User.Identity.Name;
              User mainUser = _db.Users.FirstOrDefault(i => i.Email == userName);
              Group group = _db.Groups.FirstOrDefault(i => i.IdGroup == id);
             
              _db.Entry(group)
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
                  try
                  {
                      var userName = User.Identity.Name;
                      User author = _db.Users.FirstOrDefault(i => i.Email == userName);
                      model.Mission.DateOfCreation = DateTime.Now.Date;
                      Group group = _db.Groups.FirstOrDefault(i => i.IdGroup == model.Group.IdGroup);

                      if (author != null)
                      {
                          _db.Missions.Add(model.Mission);
                          foreach (var chooseUser in model.ChooseUsers)
                          {
                              if (chooseUser.Checkbox.Value)
                              {
                                  User executer = _db.Users.FirstOrDefault(i => i.IdUser == chooseUser.User.IdUser);
                                  author.Authors.Add(new UserMission()
                                  {
                                      Author = author, 
                                      Executor = executer, 
                                      Mission = model.Mission, 
                                      Group = group,
                                      Status =  Status.Pending.ToString()
                                  });
                              }
                          }
                      }
                      await _db.SaveChangesAsync();
                      return RedirectToAction("CreateMission", "Mission", ApplicationContext.IdCurrentGroup);
                  }
                  catch (Exception e)
                  {
                      
                  }
              }
              return RedirectToAction("CreateMission", "Mission", ApplicationContext.IdCurrentGroup);
          }
          
          public IActionResult ListMissions()
          {
              var userName = User.Identity.Name;
              User mainUser = _db.Users.FirstOrDefault(i => i.Email == userName);
          
              
              var executorsMissions = _db.UserMissions
                  .Include(c => c.Group)
                  .Include(c => c.Mission)
                  .Include(c => c.Author)
                  .Include(c => c.Executor)
                  .Where(c => c.Executor.IdUser == mainUser.IdUser)
                  .ToList();
              
              
              var b = _db.UserMissions
                  .Include(c => c.Group)
                  .Include(c => c.Mission)
                  .Include(c => c.Author)
                  .Include(c => c.Executor)
                  .Where(c => c.Author.IdUser == mainUser.IdUser)
                  .ToList();

              var authorsMissions = new List<UserMission>();
              foreach (var authorsMission in b)
                  if (authorsMissions.FirstOrDefault(x => x.Mission.IdMission == authorsMission.Mission.IdMission) == null)
                      authorsMissions.Add(authorsMission);

              MissionModel model = new MissionModel
              {
                  AuthorsMissions = authorsMissions,
                  ExecutorsMissions = executorsMissions
              };
              
              return View(model);  
          } 
          
          public IActionResult ListExecutors(int id)
          {
              var a = _db.UserMissions
                  .Include(c => c.Executor)
                  .Where(c => c.Mission.IdMission == id).ToList();

              

              List<User> executors = new List<User>();
              foreach (var userMission in a)
              {
                  executors.Add(userMission.Executor);
                  
              }
              
              UserModel model = new UserModel()
              {
                  Users = executors,
              };
              
              return View("../Group/ListExicuters", model);
          }
          
          public IActionResult ListUserMissions(int id)
          {
              var a = _db.UserMissions
                  .Include(c => c.Executor)
                  .Where(c => c.Mission.IdMission == id).ToList();
              return View(a);
          } 
          
          public async Task<IActionResult> RefuseMission(int idUserMission)
          {
              var userMission = _db.UserMissions.FirstOrDefault(a => a.IdUserMission == idUserMission);
              userMission.Status = Status.Refuse.ToString(); 
              await _db.SaveChangesAsync();

              return RedirectToAction("ListMissions", "Mission");
          }
          
          public async Task<IActionResult> PassMission(int idUserMission)
          {
              var userMission = _db.UserMissions.FirstOrDefault(a => a.IdUserMission == idUserMission);
              userMission.Status = Status.Pass.ToString(); 
              await _db.SaveChangesAsync();

              return RedirectToAction("ListMissions", "Mission");
          }
          
          public async Task<IActionResult> DeleteMission(int idUserMission)
          {
              var userMission = _db.UserMissions.FirstOrDefault(a => a.IdUserMission == idUserMission);
              _db.UserMissions.Remove(userMission);
              await _db.SaveChangesAsync();
              return RedirectToAction("ListMissions", "Mission");
          }
          
          public async Task<IActionResult> ReturnMission(int idUserMission)
          {
              var userMission = _db.UserMissions.FirstOrDefault(a => a.IdUserMission == idUserMission);
              userMission.Status = Status.Take.ToString(); 
              await _db.SaveChangesAsync();
              return RedirectToAction("ListMissions", "Mission");
          }
        
          
          public async Task<IActionResult> TakeMission(int idUserMission)
          {
              var userName = User.Identity.Name;
              User user = _db.Users.FirstOrDefault(i => i.Email == userName);
              var userMission = _db.UserMissions
                  .Include(x => x.Mission)
                  .FirstOrDefault(a => a.IdUserMission == idUserMission);
              
              var a = _db.UserMissions
                  .Include(c => c.Executor)
                  .Where(c => c.Mission.IdMission == userMission.Mission.IdMission).ToList();
              
              
              var newList = a.Where(x=>x.Executor.IdUser != user.IdUser).ToList();
              foreach (var um in newList)
              {
                  _db.UserMissions.Remove(um);
              }
              userMission.Status = Status.Take.ToString(); 
              await _db.SaveChangesAsync();
           
              return RedirectToAction("ListMissions", "Mission");
          }
        
    }
}