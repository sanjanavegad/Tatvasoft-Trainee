using Helperland.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public IActionResult Contact()
        {
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
        public IActionResult Create_Account()
        {
            return View();
        }

    }
}
