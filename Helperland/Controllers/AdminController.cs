using Helperland.Data;
using Helperland.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helperland.Controllers
{
    public class AdminController : Controller
    {
        private readonly HelperlandContext _helperlandContext;

        public AdminController(HelperlandContext helperlandContext)
        {
            _helperlandContext = helperlandContext;
        } 

        public IActionResult AdminIndex()
        {
            int id = (int)HttpContext.Session.GetInt32("UserId");
            var user = _helperlandContext.Users.Where(x => x.UserId == id).FirstOrDefault();
            user.Name = user.FirstName +" " + user.LastName;
            return View(user);
        }
        public IActionResult AdminDashboard()
        {
            List<User> userdetailes = _helperlandContext.Users.Where(x => x.UserTypeId != 3).ToList();
            ViewBag.data = userdetailes;
            return PartialView("User_ManagementPartial");
        }

        public IActionResult ApproveUser(int id)
        {
            User user = _helperlandContext.Users.Where(x => x.UserId == id).FirstOrDefault();
            return PartialView("ApprovedModelPartial", user);
        }

        public IActionResult FinalApproveUser(User user)
        {
            User users = _helperlandContext.Users.Where(x => x.UserId == user.UserId).FirstOrDefault();
            users.IsActive = true;
            users.IsApproved = true;
            users.Status = 1;
            var aprroveuser = _helperlandContext.Users.Update(users);
            _helperlandContext.SaveChanges();
            if (aprroveuser != null)
            {
                return Ok(Json("true"));
            }
            return Ok(Json("false"));
        }

        public IActionResult DeactivateUser(int id)
        {
            User users = _helperlandContext.Users.Where(x => x.UserId == id).FirstOrDefault();
            users.IsActive = false;
            users.Status = 0;
            var aprroveuser = _helperlandContext.Users.Update(users);
            _helperlandContext.SaveChanges();
            if (aprroveuser != null)
            {
                return Ok(Json("true"));
            }
            return Ok(Json("false"));
        }
        public IActionResult ActivateUser(int id)
        {
            User users = _helperlandContext.Users.Where(x => x.UserId == id).FirstOrDefault();
            users.IsActive = true;
            users.Status = 1;
            var aprroveuser = _helperlandContext.Users.Update(users);
            _helperlandContext.SaveChanges();
            if (aprroveuser != null)
            {
                return Ok(Json("true"));
            }
            return Ok(Json("false"));
        }



        public IActionResult User_Rquest()
        {
            return PartialView("User_RequestPartial");
        }
    }
}
