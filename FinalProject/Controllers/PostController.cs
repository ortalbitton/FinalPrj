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
        private readonly CommentService _commentService;
        private readonly int pageSize;

        public PostController(PostService postService, UserService userService, CommentService commentService)
        {
            _postService = postService;
            _userService = userService;
            _commentService = commentService;
            pageSize = 10;
        }



        public IActionResult Home(bool? isAuthenticated, int pageNumber)
        {
            ViewBag.isAuthenticated = isAuthenticated;

            ViewBag.fail_delete = "true";

            int totalPages = _postService.getPostList().ToPagedList(pageNumber, pageSize).PageCount;

            int totalpostPerPage = _postService.getPostList().ToPagedList(pageNumber, pageSize).Count;

            if (totalpostPerPage == 0 && totalPages >= 1)
                pageNumber = totalPages;


            return View(_postService.getPostList().OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));
        }


        public IActionResult Auto_Delete(int pageNumber, string postId, string commentId)
        {

            var user = _userService.getUserByKey(HttpContext.Session.GetString("Mail"));


            if (postId != null)//Reply
            {

                _postService.addUserToBlockList(postId, user.email);

                if (_postService.checkBlockList(postId) == 200 || _postService.checkBlockList(postId) ==  (int)(_userService.getUserList().Count *0.5) )
                {
                    _postService.removePost(postId);
                    ViewBag.fail_delete = "false";
                }

            }

            if (commentId != null)//Reply
            {
                _commentService.addUserToBlockList(commentId, HttpContext.Session.GetString("Mail"));

                if (_commentService.checkBlockList(commentId) == 200 || _commentService.checkBlockList(commentId) == (int)(_userService.getUserList().Count * 0.5))
                {

                    foreach (var post in _postService.getPostList().Where(x => x.comList != null))
                    {
                        foreach (var comment in post.comList.Where(c => c.Id == commentId).ToList())
                        {
                            post.comList.Remove(comment);
                        }
                        _postService.updatePost(post.Id, post);
                    }

                    _commentService.removeComment(commentId);

                    ViewBag.fail_delete = "false";

                }
            }


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
        public IActionResult EditPost(string PostId, int pageNumberOfPost, int pageNumberOfSRT)
        {
            ViewBag.name = _postService.getPostById(PostId).name;
            ViewBag.pageNumberOfPost = pageNumberOfPost;
            ViewBag.pageNumberOfSRT = pageNumberOfSRT;
            ViewData["PostId"] = PostId;
            return View(_postService.getPostById(PostId));
        }

        // POST: Posts/EditPost/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPost(Post postIn, int pageNumberOfPost, int pageNumberOfSRT)
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
                return RedirectToAction("Profile", "User", new { isAuthenticated = true, pageNumberOfPost, pageNumberOfSRT });

            }
            catch
            {
                return View();
            }
        }

        // GET: Posts/DeletePost/5
        public IActionResult DeletePost(string PostId, int pageNumberOfPost)
        {
            try
            {
                var post = _postService.getPostById(PostId);
                _postService.removePost(post.Id);
                return RedirectToAction("Profile", "User", new { isAuthenticated = true, pageNumberOfPost });
            }
            catch
            {
                return View();
            }
        }

    }
}