﻿using System;
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
        private readonly int pageSize;

        public PostController(PostService postService)
        {
            _postService = postService;
            pageSize = 10;

        }

       

        public IActionResult Home(bool? isAuthenticated, int? pageNumber)
        {
            ViewBag.isAuthenticated = isAuthenticated;

            return View(_postService.getPostList().OrderByDescending(x => x.Id).ToPagedList(pageNumber ?? 1, pageSize));
        }

        /// GET: Posts/Create
        public IActionResult NewPost(int pageNumber)
        {
            ViewBag.pageNumber = pageNumber;
            return View();
        }

        // POST: Posts/Create
        [HttpPost]
        public IActionResult NewPost(Post post, int pageNumber)
        {
            try
            {

                 pageNumber = 1;

                _postService.createPost(post, HttpContext.Session.GetString("Mail"));

                return RedirectToAction("Home", "Post", new { isAuthenticated = true, pageNumber });

            }
            catch
            {
                return View();
            }
        }

        // GET: Posts/Edit/5
        public IActionResult EditPost(string PostId, int pageNumber)
        {
            ViewBag.name = _postService.getPostById(PostId).name;
            ViewBag.pageNumber = pageNumber;
            ViewData["PostId"] = PostId;
            return View(_postService.getPostById(PostId));
        }

        // POST: Posts/Edit/5
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

    }
}