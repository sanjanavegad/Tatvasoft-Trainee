using Helperland.Data; 
using Helperland.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Helperland.Controllers
{
    public class PublicPagesController : Controller
    {
        private readonly HelperlandContext _helperlandContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        

        public PublicPagesController(HelperlandContext helperlandContext, IWebHostEnvironment webHostEnvironment)
        {
           _helperlandContext = helperlandContext;
            _webHostEnvironment = webHostEnvironment;
        }


        public IActionResult FAQ()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Contact()
        {
            ContactU c = new ContactU();
            return View();
        }
        [HttpPost]
        public IActionResult Contact(ContactU contactU)
        {
            if (ModelState.IsValid)
            {
                if (contactU.AttechmentFile != null) 
                {
                    String folder = "AttechmentFiles/";
                    folder += Guid.NewGuid().ToString() + "_" + contactU.AttechmentFile.FileName;
                    String serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                    _ = contactU.AttechmentFile.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                    contactU.FileName = folder;
                }
                contactU.CreatedOn = DateTime.Now;
                _helperlandContext.ContactUs.Add(contactU);
                _ = _helperlandContext.SaveChangesAsync();
                return RedirectToAction("Contact");
            }
            return View();
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
