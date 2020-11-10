using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.Models;
using FinalProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    public class UserController : Controller
    {

        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            ViewBag.Fail = false;
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(User user)
        {
            //כשיש התאמה
            if (_userService.signIn(user).ToList().Count > 0)
            {
                HttpContext.Session.SetString("Mail", user.email);
                return RedirectToAction("Index", "Post");
            }


            ViewBag.Fail = true;

            return View(user);
        }
    }
}