using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    public class SrtController : Controller
    {
        private readonly SrtService _srtService;
        private readonly UserService _userService;

        public SrtController(SrtService srtService, UserService userService)
        {
            _srtService = srtService;
            _userService = userService;
        }

        public IActionResult CreateSRT(bool? isAuthenticated)
        {
            ViewBag.isAuthenticated = isAuthenticated;

            return View();
        }

        [HttpPost]
        public IActionResult CreateSRT(IFormFile VideoFile)
        {
            string email = HttpContext.Session.GetString("Mail");
            string userName = _userService.getUserByKey(email).name;
            string directoryPath = _srtService.getDirPath(VideoFile, userName);

            _srtService.saveVideo(VideoFile, directoryPath);
            Convert(VideoFile, directoryPath);
            return View();
        }

        public string srtPath;
        public string videoPath;
        public string picPath;
        public string line;

        public void Convert(IFormFile videoFile, string directoryPath)
        {

            srtPath = _srtService.getSrtPath(videoFile, directoryPath);

            //loop
            //create picture from specific frame of video
            _srtService.extractPic(videoFile.FileName.Replace(" ", "_"), 100, directoryPath);

            _srtService.writeToSrt(srtPath, "fff");
            _srtService.writeToSrt(srtPath, "ddd");
            //end of loop


            //delete folder
            _srtService.deleteDir(directoryPath);
        }
    }
}