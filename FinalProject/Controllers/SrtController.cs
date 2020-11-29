using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    public class SrtController : Controller
    {
        public IActionResult CreateSRT(bool? isAuthenticated)
        {
            ViewBag.isAuthenticated = isAuthenticated;

            return View();
        }

        //[HttpPost]
        //public IActionResult CreateSRT()
        //{
        //    return View();
        //}
    }
}