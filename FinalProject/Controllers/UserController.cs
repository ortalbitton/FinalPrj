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

        // GET: Users/Create
        public IActionResult SignUp()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        public IActionResult SignUp(User user)
        {
            try
            {
                _userService.createUser(user);

                HttpContext.Session.SetString("Mail", user.email);

                return RedirectToAction("Home", "Post" ,new { isAuthenticated = true, pageNumber = 1 });
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult SignIn(bool isAuthenticated)
        {
            ViewBag.isAuthenticated = isAuthenticated;
            ViewBag.Fail = false;
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(User user)
        {
         
            if (_userService.getUser(user).ToList().Count > 0)
            {
                HttpContext.Session.SetString("Mail", user.email);
                return RedirectToAction("Home", "Post", new { isAuthenticated = true, pageNumber = 1 });
            }


            ViewBag.Fail = true;

            return View(user);
        }

        [HttpGet]
        public IActionResult SignOut(bool isAuthenticated)
        {
            isAuthenticated = false;
            HttpContext.Session.SetString("Mail", "");
            return RedirectToAction("SignIn", "User", isAuthenticated);
        }

    }
}