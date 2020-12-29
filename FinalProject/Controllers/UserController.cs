using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.HelpClasses;
using FinalProject.Models;
using FinalProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PagedList;

namespace FinalProject.Controllers
{
    public class UserController : Controller
    {

        private readonly UserService _userService;
        private readonly PostService _postService;

        public UserController(UserService userService,PostService postService)
        {
            _userService = userService;
            _postService = postService;
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


        // Profile
        public IActionResult Profile(bool? isAuthenticated,int? pageNumberOfSRT,int? pageNumberOfPost)
        {
            ViewBag.isAuthenticated = isAuthenticated;

            ViewBag.fail = false;

            var user = _userService.getUserByKey(HttpContext.Session.GetString("Mail"));

            if (user == null)
                return RedirectToAction(nameof(SignIn));

            ViewBag.userName = user.name; //userName then connected

            List<Post> posts = _postService.getUserPost(user.name);

            List<Srt> srts = user.srtList;


            ViewBag.pageNumberOfPost = pageNumberOfPost;
            ViewBag.pageNumberOfSRT = pageNumberOfSRT;


            SRTAndPost mymodel = new SRTAndPost();

            if (posts != null)
            {
                mymodel.postList = posts.OrderByDescending(x => x.Id).ToPagedList(pageNumberOfPost ?? 1, 10);
                if (posts.Count == 0)
                    ViewBag.fail = true;
            }


            if (srts != null)
                mymodel.srtList = srts.OrderByDescending(x => x.date).ToPagedList(pageNumberOfSRT ?? 1, 10);


            ViewData["posts"] = mymodel.postList;

            ViewData["srts"] = mymodel.srtList;


            return View();
        }

        //פונקצית עזר למחיקה
        public IActionResult Refresh(int pageNumberOfPost, int totalPages,int pageNumberOfSRT)
        {
            //totalPages is pagesOfpost
            var user = _userService.getUserByKey(HttpContext.Session.GetString("Mail"));
            int totalpostPerPage = _postService.getUserPost(user.name).OrderByDescending(x => x.Id).ToPagedList(pageNumberOfPost, 10).Count;

            if (totalpostPerPage == 0 && totalPages != 1)
                pageNumberOfPost = totalPages - 1;

            return RedirectToAction("Profile", "User", new { isAuthenticated = true, pageNumberOfPost, pageNumberOfSRT });
        }



    }
}