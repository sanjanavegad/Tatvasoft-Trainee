using Helperland.Data;
using Helperland.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Helperland.Controllers
{
     
    public class HomeController : Controller
    {

        private readonly HelperlandContext _helperlandContext;

        public HomeController(HelperlandContext helperlandContext)
        {
            _helperlandContext = helperlandContext;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(User user)
        {
        if (ModelState.IsValid)
        {
            using (HelperlandContext ObjHelperlandContext = new HelperlandContext())
            {
                    string email = user.Email;
                    var p = ObjHelperlandContext.Users.Where(c => c.Email == email && c.Password == user.Password).ToList();
                    ModelState.Clear();
                    if (p.Count == 1)
                    {
                        if (p.FirstOrDefault().UserTypeId == 1)
                        {
                            return RedirectToAction("Create_Account", "Home");
                        }
                        if (p.FirstOrDefault().UserTypeId == 2)
                        {
                            return RedirectToAction("Service_Provider", "Home");
                        }
                    }
            }

            return PartialView("LoginModel");
            }
        return View();
        }

        public IActionResult Service_Provider()
        {
            User signup = new User();
            return View(signup);
        }
        [HttpPost]
        public IActionResult Service_Provider(User signup)
        {
            if (ModelState.IsValid)
            {
                if (_helperlandContext.Users.Where(x => x.Email == signup.Email).Count() == 0 && _helperlandContext.Users.Where(x => x.Mobile == signup.Mobile).Count() == 0)
                {
                    signup.UserTypeId = 2;
                    signup.CreatedDate = DateTime.Now;
                    signup.ModifiedDate = DateTime.Now;
                    signup.IsRegisteredUser = true;
                    signup.ModifiedBy = 123;

                    _helperlandContext.Users.Add(signup);
                    _helperlandContext.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Message = "User already registed";
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult Create_Account()
        {
            User signup = new User();
            return View(signup);
        }

        [HttpPost]
        public IActionResult Create_Account(User signup)
        {
            if(ModelState.IsValid)
            {
                if (_helperlandContext.Users.Where(x => x.Email == signup.Email).Count() == 0 && _helperlandContext.Users.Where(x => x.Mobile == signup.Mobile).Count() == 0)
                {
                    signup.UserTypeId = 1;
                    signup.CreatedDate = DateTime.Now;
                    signup.ModifiedDate = DateTime.Now;
                    signup.IsRegisteredUser = true;
                    signup.ModifiedBy = 123;

                    _helperlandContext.Users.Add(signup);
                    _helperlandContext.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Message = "User already registed";
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult Customer_Home()
        {
            return View("customer_home");
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
