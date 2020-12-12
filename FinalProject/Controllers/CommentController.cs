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


        // GET: Comment/NewComment
        public IActionResult NewComment(string PostId, int pageNumber)
        {
            var user = _userService.getUserByKey(HttpContext.Session.GetString("Mail"));
            ViewData["PostId"] = PostId;
            ViewBag.pageNumber = pageNumber;

            return PartialView();
        }

        // POST: Comments/NewComment
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

        // GET: Comments/EditComment/5
        public IActionResult EditComment(string CommentId, int pageNumber)
        {
            ViewBag.name = _commentService.getCommentById(CommentId).name;
            ViewBag.pageNumber = pageNumber;
            ViewData["CommentId"] = CommentId;
            return View(_commentService.getCommentById(CommentId));
        }

        // POST: Comments/EditComment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditComment(Comment commentIn, int pageNumber)
        {
            try
            {
                var comment = _commentService.getCommentById(commentIn.Id);

                if (comment == null)
                {
                    return NotFound();
                }


                foreach (var post in _postService.getPostList().Where(x => x.comList != null))
                {
                    foreach(var c in  post.comList.Where(c => c.Id == comment.Id))
                    {
                        c.text= commentIn.text;
                    }
                    _postService.updatePost(post.Id, post);
                }

                return RedirectToAction("Home", "Post", new { isAuthenticated = true, pageNumber });

            }
            catch
            {
                return View();
            }
        }


        // GET: Comments/DeleteComment/5
        public IActionResult DeleteComment(string CommentId, int pageNumber)
        {
            try
            {
                foreach (var post in _postService.getPostList().Where(x => x.comList != null))
                {
                    foreach (var c in post.comList.Where(c => c.Id == CommentId).ToList())
                    {
                        post.comList.Remove(c);
                    }
                    _postService.updatePost(post.Id, post);
                }

                _commentService.removeComment(CommentId);

                return RedirectToAction("Home", "Post", new { isAuthenticated = true, pageNumber });
            }
            catch
            {
                return View();
            }
        }


    }
}