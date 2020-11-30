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
    public class CommentController : Controller
    {

        private readonly CommentService _commentService;
        private readonly UserService _userService;
        private readonly PostService _postService;

        public CommentController(CommentService commentService, UserService userService, PostService postService)
        {
            _commentService = commentService;
            _userService = userService;
            _postService = postService;
        }


        // GET: Comment/Create
        public IActionResult NewComment(string PostId, int pageNumber)
        {
            var user = _userService.getUserByKey(HttpContext.Session.GetString("Mail"));
            ViewData["PostId"] = PostId;
            ViewBag.pageNumber = pageNumber;

            return PartialView();
        }

        // POST: Comments/Create
        [HttpPost]
        public IActionResult NewComment(Comment comment, int pageNumber)
        {
            try
            {

                _commentService.createComment(comment, HttpContext.Session.GetString("Mail"));

                _postService.addCommentToPost(comment.postId.Id, comment.Id, comment.name);

                return RedirectToAction("Home", "Post", new { isAuthenticated = true, pageNumber });


            }
            catch
            {
                return View();
            }
        }

    }
}