using Helperland.Data;
using Helperland.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Helperland.Controllers
{

    public class HomeController : Controller
    {

        private readonly HelperlandContext _helperlandContext;

        public HomeController(HelperlandContext helperlandContext)
        {
            _helperlandContext = helperlandContext;
        }
        [HttpGet]
        
        public IActionResult Index()
        {

            return View();
        }
        
        [HttpPost]
        public IActionResult Index(User user)
        {
            var p = _helperlandContext.Users.Where(c => c.Email == user.Email && c.Password == user.Password).ToList();
            if (p.Count == 1)
            {
                User userdetails = _helperlandContext.Users.Where(c => c.Email == user.Email && c.Password == user.Password && c.IsApproved == true).FirstOrDefault();
                if (userdetails != null)
                {
                    var Name = userdetails.FirstName + " " + userdetails.LastName;
                    ViewBag.userType = user.UserTypeId;
                    HttpContext.Session.SetString("isLoggedIn", "true");
                    HttpContext.Session.SetString("Name", Name);
                    HttpContext.Session.SetInt32("UserTypeId", userdetails.UserTypeId);
                    HttpContext.Session.SetInt32("UserId", userdetails.UserId);


                    if (p.FirstOrDefault().UserTypeId == 1)
                    {
                        HttpContext.Session.SetString("UserTypeId", userdetails.UserTypeId.ToString());
                        return RedirectToAction("Customer_Dashboard", "CustomerPages");
                    }
                    else if (p.FirstOrDefault().UserTypeId == 2)
                    {
                        HttpContext.Session.SetString("UserTypeId", userdetails.UserTypeId.ToString());
                        return RedirectToAction("Provider_Dashboard", "ProviderPages");

                    }
                    else if (p.FirstOrDefault().UserTypeId == 3)
                    {
                        HttpContext.Session.SetString("UserTypeId", userdetails.UserTypeId.ToString());
                        return RedirectToAction("AdminIndex", "Admin");
                    }
                }
                else
                {
                    TempData["approved"] = " not Approoved";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["loginemail"] = "Please Enter your Registed Email Address and Password.";
                //ViewBag.Message1 = "Please Enter your Registed Email Address and Password.";
                //TempData["LoginMessage"] = "Please Enter your Registed Email Address and Password.";
                ModelState.Clear();

            }
            return View();
        }
        
        public IActionResult SendLink(User user)
        {
            String ResetCode = Guid.NewGuid().ToString();

            var uriBuilder = new UriBuilder
            {
                Scheme = Request.Scheme,
                Host = Request.Host.Host,
                Port = Request.Host.Port ?? -1, //bydefault -1
                Path = $"/Home/ForgotPassword/{ResetCode}"
            };
            var link = uriBuilder.Uri.AbsoluteUri;

            var getUser = (from s in _helperlandContext.Users where s.Email == user.Email select s).FirstOrDefault();
            if (getUser != null)
            {
                getUser.ResetPasswordCode = ResetCode;
                _helperlandContext.SaveChanges();

                var subject = "Password Reset Request";
                var body = "Hi " + getUser.FirstName + ", <br/> You recently requested to reset the password for your account. Click the link below to reset ." +
                 "<br/> <br/><a href= '" + link + "' > " + link + " </a> <br/> <br/>" +
                "If you did not request for reset password please ignore this mail.";

                SendEmail(getUser.Email, body, subject);
                TempData["forgotmsg"] = "Reset password link has been sent to your email id";
                //ViewBag.Message2 = "Reset password link has been sent to your email id";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["forgoterror"] = "user does not exists.";
                //ViewBag.Message3 = "user does not exists.";
                ModelState.Clear();
            }
            return RedirectToAction("Index", "Home");
        }
        
        private void SendEmail(string emailaddress, string body, string subject)
        {
            using (MailMessage mm = new MailMessage("sanjanavegad123@gmail.com", emailaddress))
            {
                mm.Subject = subject;
                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    EnableSsl = true
                };
                NetworkCredential NetworkCred = new NetworkCredential("sanjanavegad123@gmail.com", "s@sanju123");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
            }
        }

        public IActionResult ForgotPassword(String id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            using (HelperlandContext ObjHelperlandContext = new HelperlandContext())
            {
                var user = ObjHelperlandContext.Users.Where(f => f.ResetPasswordCode == id).FirstOrDefault();
                if (user != null)
                {
                    ForgotPassword modal = new ForgotPassword();
                    modal.ResetCode = id;
                    return View(modal);
                }
                else
                {
                    return NotFound();
                }
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPassword model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using (HelperlandContext ObjHelperlandContext = new HelperlandContext())
                {
                    var user = ObjHelperlandContext.Users.Where(f => f.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if (user != null)
                    {
                        //you can encrypt password here, we are not doing it
                        user.Password = model.NewPassword;
                        //make resetpasswordcode empty string now
                        user.ResetPasswordCode = "";
                        //to avoid validation issues, disable it
                        ObjHelperlandContext.SaveChanges();
                        message = "New password updated successfully";
                    }
                }
            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Service_Provider()
        {
            return View();
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
                    ModelState.Clear();
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
            return View();
        }
        
        [HttpPost]
        public IActionResult Create_Account(User signup)
        {
            if (ModelState.IsValid)
            {
                if (_helperlandContext.Users.Where(s => s.Email == signup.Email).Count() == 0 && _helperlandContext.Users.Where(s => s.Mobile == signup.Mobile).Count() == 0)
                {
                    signup.UserTypeId = 1;
                    signup.CreatedDate = DateTime.Now;
                    signup.ModifiedDate = DateTime.Now;
                    signup.IsRegisteredUser = true;
                    signup.IsApproved = true;
                    signup.Status = 1;
                    signup.IsActive = true;
                    signup.ModifiedBy = 123;

                    _helperlandContext.Users.Add(signup);
                    _helperlandContext.SaveChanges();
                    ModelState.Clear();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Message = "This Email and Mobile are already registerd.";
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult Customer_Home()
        {
            return View("customer_home");
        }

        public IActionResult Logout()
        {
            
            HttpContext.Session.SetString("isLoggedIn", false.ToString());
            HttpContext.Session.Clear();
            TempData["LogOutMsg"] = "Logged out";
            return RedirectToAction("Index", "Home");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}