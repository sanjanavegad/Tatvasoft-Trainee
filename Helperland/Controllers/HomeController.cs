using Helperland.Data;
using Helperland.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
            using (HelperlandContext ObjHelperlandContext = new HelperlandContext())
            {
                string email = user.Email;
                var p = ObjHelperlandContext.Users.Where(c => c.Email == email && c.Password == user.Password).ToList();
                ModelState.Clear();
                if (p.Count == 1)
                {
                    User userdetails = ObjHelperlandContext.Users.Where(c => c.Email == user.Email && c.Password == user.Password).FirstOrDefault();
                    var Name = userdetails.FirstName + " " + userdetails.LastName;
                    ViewBag.userType = user.UserTypeId;
                    HttpContext.Session.SetString("isLoggedIn", "true");
                    HttpContext.Session.SetString("Name", Name);
                    HttpContext.Session.SetInt32("UserId", userdetails.UserId);

                     
                    if (p.FirstOrDefault().UserTypeId == 1)
                    {
                        HttpContext.Session.SetString("UserTypeId", user.UserTypeId.ToString());
                        return RedirectToAction("Customer_Dashboard", "CustomerPages");
                    }
                    else if (p.FirstOrDefault().UserTypeId == 2)
                    {
                        HttpContext.Session.SetString("UserTypeId", user.UserTypeId.ToString());
                        return RedirectToAction("Service_Provider", "Home");
                    }
                }
                else
                {
                    ViewBag.Message1 = "Please Enter your Registed Email Address and Password.";
                     ModelState.Clear();
                }
                String ResetCode = Guid.NewGuid().ToString();

                var uriBuilder = new UriBuilder
                {
                    Scheme = Request.Scheme,
                    Host = Request.Host.Host,
                    Port = Request.Host.Port ?? -1, //bydefault -1
                    Path = $"/Home/ForgotPassword/{ResetCode}"
                };
                var link = uriBuilder.Uri.AbsoluteUri;

                var getUser = (from s in ObjHelperlandContext.Users where s.Email == user.Email select s).FirstOrDefault();
                if (getUser != null)
                {
                    getUser.ResetPasswordCode = ResetCode;
                    ObjHelperlandContext.SaveChanges();

                    var subject = "Password Reset Request";
                    var body = "Hi " + getUser.FirstName + ", <br/> You recently requested to reset the password for your account. Click the link below to reset ." +
                     "<br/> <br/><a href= '" + link + "' > " + link + " </a> <br/> <br/>" +
                    "If you did not request for reset password please ignore this mail.";

                    SendEmail(getUser.Email, body, subject);

                    ViewBag.Message2 = "Reset password link has been sent to your email id";
                }
                else
                {
                    ViewBag.Message3 = "user does not exists.";
                    return View();
                }
                return View();
            }
            
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
            return View(model);
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
            if (ModelState.IsValid)
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

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Msg"] = "You have succesfully logged out";
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}