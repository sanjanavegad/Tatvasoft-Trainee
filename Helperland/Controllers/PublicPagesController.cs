using Helperland.Data; 
using Helperland.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Helperland.Controllers
{
    public class PublicPagesController : Controller
    {
        private readonly HelperlandContext _helperlandContext;

        public PublicPagesController(HelperlandContext helperlandContext)
        {
           _helperlandContext = helperlandContext;
        }

        public IActionResult FAQ()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Contact(ContactU c, String Lastname)
        {
            if (ModelState.IsValid)
            {
                using (HelperlandContext objHelperlandContext = new HelperlandContext())
                {
                    string lastname = Convert.ToString(Lastname);
                    c.Name = (c.Name + " " + lastname);
                    c.Message = ("This mail is sent by " + c.Name + " . <br/> Email:  " + c.Email + "<br/> Phone number:  " + c.PhoneNumber + "<br/> The main message of the mail is : " + c.Message);
                    c.CreatedOn = DateTime.Now;
                    objHelperlandContext.ContactUs.Add(c);
                    objHelperlandContext.SaveChanges();
                    SendEmail(c.Message, c.Subject);
                    // Int64 id = objEmployee.EmployeeID;
                    ModelState.Clear();
                }
                // return View(objEmployee);
            }
            return View();
        }

        private void SendEmail(string body, string subject)
        {
            using (MailMessage mm = new MailMessage("sanjanavegad123@gmail.com", "sanjanabavegad.4464@gmail.com"))
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
                ViewBag.message = "Email send to admin successfully";
            }
        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult Prices()
        {
            return View();
        }
        

    }
}
