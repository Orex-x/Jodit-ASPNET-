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
                                  Group = group
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
          
              
              var userMissions = db.UserMissions.Include(c => c.Group)
                  .Include(c => c.Mission)
                  .Include(c => c.Author)
                  .Include(c => c.Executor)
                  .Where(c => c.Executor.IdUser == mainUser.IdUser)
                  .ToList();
              
              
              MissionModel model = new MissionModel
              {
                  UserMissions = userMissions
              };
              
              return View(model);  
          } 
    }
}