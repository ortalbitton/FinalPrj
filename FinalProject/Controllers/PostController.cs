using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.Models;
using FinalProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PagedList;

namespace FinalProject.Controllers
{
    public class PostController : Controller
    {

        private readonly PostService _postService;
        private readonly UserService _userService;
        private readonly int pageSize;

        public PostController(PostService postService,UserService userService)
        {
            _postService = postService;
            _userService = userService;
            pageSize = 10;

        }

       

        public IActionResult Home(bool? isAuthenticated, int? pageNumber)
        {
            ViewBag.isAuthenticated = isAuthenticated;

            ViewBag.userName = _userService.getUserByKey(HttpContext.Session.GetString("Mail")).name; //userName then connected

            return View(_postService.getPostList().OrderByDescending(x => x.Id).ToPagedList(pageNumber ?? 1, pageSize));
        }

        public IActionResult Refresh(int pageNumber,int totalPages)
        {
            int totalpostPerPage = _postService.getPostList().OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize).Count;
            int totalpost = _postService.getPostList().Count;

            if (totalpostPerPage == 0 && totalPages!=1)
                pageNumber = totalPages - 1;

            return RedirectToAction("Home", "Post", new { isAuthenticated = true, pageNumber });
        }

        /// GET: Posts/NewPost
        public IActionResult NewPost(int pageNumber)
        {
            ViewBag.pageNumber = pageNumber;
            return View();
        }

        // POST: Posts/NewPost
        [HttpPost]
        public IActionResult NewPost(Post post)
        {
            try
            {

                int pageNumber = 1;

                _postService.createPost(post, HttpContext.Session.GetString("Mail"));

                return RedirectToAction("Home", "Post", new { isAuthenticated = true, pageNumber });

            }
            catch
            {
                return View();
            }
        }

        // GET: Posts/EditPost/5
        public IActionResult EditPost(string PostId, int pageNumber)
        {
            ViewBag.name = _postService.getPostById(PostId).name;
            ViewBag.pageNumber = pageNumber;
            ViewData["PostId"] = PostId;
            return View(_postService.getPostById(PostId));
        }

        // POST: Posts/EditPost/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPost(Post postIn, int pageNumber)
        {
            try
            {
                var post = _postService.getPostById(postIn.Id);

                if (post == null)
                {
                    return NotFound();
                }

                post.text = postIn.text;

                _postService.updatePost(post.Id, post);
                return RedirectToAction("Home", "Post", new { isAuthenticated = true, pageNumber });

            }
            catch
            {
                return View();
            }
        }

        // GET: Posts/DeletePost/5
        public IActionResult DeletePost(string PostId, int pageNumber)
        {
            try
            {
                var post = _postService.getPostById(PostId);
                _postService.removePost(post.Id);
                return RedirectToAction("Home", "Post", new { isAuthenticated = true, pageNumber });
            }
            catch
            {
                return View();
            }
        }

    }
}