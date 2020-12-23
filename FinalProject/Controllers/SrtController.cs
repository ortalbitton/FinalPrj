using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.Models;
using FinalProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;
using PagedList;

namespace FinalProject.Controllers
{
    public class SrtController : Controller
    {
        private readonly SrtService _srtService;
        private readonly UserService _userService;
        string dictionaryPath;

        private readonly int countSRTPerPage;

        public SrtController(SrtService srtService, UserService userService)
        {
            _srtService = srtService;
            _userService = userService;
            dictionaryPath = _srtService.getDictionaryPath();

            countSRTPerPage = 10;
        }

        public IActionResult CreateSRT(bool? isAuthenticated)
        {
            ViewBag.isAuthenticated = isAuthenticated;

            List<SelectListItem> categoryList = new List<SelectListItem>();
            string[] categorys = _srtService.getCategoryList(dictionaryPath).ToArray();//get from dictionary
            for (int i = 0; i < categorys.Length; i++)
            {
                categoryList.Add(new SelectListItem
                {
                    Text = categorys[i].ToString()
                });
            }

            return View(categoryList);

            //return View();
        }

        [HttpPost]
        public IActionResult CreateSRT(IFormFile VideoFile, List<SelectListItem> categoryList)
        {

            List<string> selected = new List<string>();

            foreach (SelectListItem item in categoryList)
            {
                if (item.Selected)
                    selected.Add(item.Text.ToString());

            }


            string email = HttpContext.Session.GetString("Mail");
            string userName = _userService.getUserByKey(email).name;
            string directoryPath = _srtService.getDirPath(VideoFile, userName);

            _srtService.saveVideo(VideoFile, directoryPath);
             ObjectId fileId = Convert(VideoFile, directoryPath);

            //create מופע srt (לא במסד נתונים)
            Srt srt = new Srt();
            srt.name = Path.GetFileNameWithoutExtension(VideoFile.FileName);
            srt.date = DateTime.Now.ToString();
            srt.fileId = fileId.ToString();

            //save srt in json file
            _srtService.setCategory(selected, srt, dictionaryPath);

            //הוספת הקובץ לרשימת הקבצים של המשתמש
            _srtService.addSRTToUser(HttpContext.Session.GetString("Mail"), srt);

            ViewBag.isAuthenticated = true;

            return View(categoryList);


            //return View();
        }

        public string srtPath;
        public string videoPath;
        public string picPath;
        public string line;

        public ObjectId Convert(IFormFile videoFile, string directoryPath)
        {
            string text = "";
            srtPath = _srtService.getSrtPath(videoFile, directoryPath);

            //loop
            //create picture from specific frame of video
            _srtService.extractPic(videoFile.FileName.Replace(" ", "_"), 100, directoryPath);

            text = _srtService.ExtractTextFromPic(directoryPath);


            _srtService.writeToSrt(srtPath, text);
            //end of loop

            //save in GridFSBucket
            ObjectId fileId = _srtService.saveSRTBucket(srtPath);

            //delete folder
            _srtService.deleteDir(directoryPath);

            return fileId;
        }


        public IActionResult SearchSRT(bool? isAuthenticated, int pageNumber, string categoryName, string status)
        {
            ViewBag.isAuthenticated = isAuthenticated;

            ViewBag.categoryNames = _srtService.getCategoryList(dictionaryPath).ToList();

            ViewBag.status = status; //save in a ViewBag for PagedList

            ViewBag.categoryName = categoryName; //save in a ViewBag for PagedList

            ViewBag.fail = false;

            IPagedList<Srt> resultSRT = null;

            if (status != null)
            {
                resultSRT = _srtService.getSrtList(dictionaryPath).OrderByDescending(x => x.date).Where(x => x.name.Contains(status)).ToPagedList(pageNumber, countSRTPerPage);
                if(resultSRT.Count == 0)
                    ViewBag.fail = true;
                return View(resultSRT);
            }


            if (categoryName != null)
            {
                resultSRT = _srtService.searchBycategoryName(dictionaryPath,categoryName).OrderByDescending(x => x.date).ToPagedList(pageNumber, countSRTPerPage);
                if (resultSRT.Count == 0)
                    ViewBag.fail = true;
                return View(resultSRT);
             }

            return View(_srtService.getSrtList(dictionaryPath).OrderByDescending(x => x.date).ToPagedList(pageNumber, countSRTPerPage));

        }



        public void Download(string fileName, string fileId)
        {
   
            //return File();
        }
    }
}