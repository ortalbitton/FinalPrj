using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.HelpClasses;
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
        public Timing startTime;
        public Timing endTime;
        public SrtController(SrtService srtService, UserService userService)
        {
            startTime = new Timing();
            endTime = new Timing();
            _srtService = srtService;
            _userService = userService;
            dictionaryPath = _srtService.getDictionaryPath();

            countSRTPerPage = 10;
        }

        public IActionResult CreateSRT(bool? isAuthenticated)
        {
            ViewBag.isAuthenticated = isAuthenticated;

            ViewBag.fail = false;
            ViewBag.data = "";

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
        [RequestFormLimits(MultipartBodyLengthLimit = 2147483647)]
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

            string videoPath = _srtService.saveVideo(VideoFile, directoryPath);


            string fileId = Convert(VideoFile, directoryPath, videoPath);

            if (fileId == "")
            {
                ViewBag.fail = true;
                ViewBag.isAuthenticated = true;
                
                return View(categoryList);

            }
            else
            {
                //create מופע srt (לא במסד נתונים)
                Srt srt = new Srt();
                srt.name = Path.GetFileNameWithoutExtension(VideoFile.FileName);
                srt.date = DateTime.Now.ToString();
                srt.fileId = fileId;
                ViewBag.fail = false;
                //save srt in json file
                _srtService.setCategory(selected, srt, dictionaryPath);

                //הוספת הקובץ לרשימת הקבצים של המשתמש
                _srtService.addSRTToUser(HttpContext.Session.GetString("Mail"), srt);

                ViewBag.isAuthenticated = true;

                ViewBag.data = "success";
                return View(categoryList);

            }

            

            //return View();
        }

        public string srtPath;
        public string text;
        public string prev;
        public string midString;
        public int subNumber;
        public int currentFrame;
        public int numOfFrames;
        public double fps;
        public double fpsAdd;

        public string Convert(IFormFile videoFile, string directoryPath, string videoPath)
        {
            text = "";
            prev = "";
            subNumber = 1;
            currentFrame = 0;
            double fps = _srtService.getVidFPS(videoPath);
            numOfFrames = _srtService.getFramesTotal(videoPath, fps);
            int numOfJumps = 3;
            fpsAdd = (double)numOfJumps / fps;

            srtPath = _srtService.getSrtPath(videoFile, directoryPath);
            int totalFrames = _srtService.extractPic(videoFile.FileName.Replace(" ", "_"), directoryPath, numOfFrames);
            //totalFrames = -1;
            string fileId;

            while (currentFrame <= totalFrames)
            {
                text = _srtService.ExtractTextFromPic(directoryPath, currentFrame);

                text = _srtService.fixText(text);


                currentFrame = currentFrame + numOfJumps;
                //מקרה 1
                //(אדום)התחלת כתובית אחרי הפסקה או בפעם הראשונה
                if (prev == "" && text != "")
                {
                    _srtService.writeToSrt(srtPath, subNumber.ToString());
                    midString = startTime.printTime() + " " + "-->";
                    endTime.copyTime(startTime);
                    if (!endTime.plus(fpsAdd))
                    {
                        //there is a problem with the time
                        totalFrames = -1;
                    }

                }
                //מקרה 2
                //(סגול)התחלת כתובית מיד אחרי כתובית
                else if (text != prev && text != "" && prev != "")
                {
                    _srtService.writeToSrt(srtPath, midString + " " + endTime.printTime());
                    _srtService.writeToSrt(srtPath, prev);
                    _srtService.writeToSrt(srtPath, "");
                    subNumber++;
                    startTime.copyTime(endTime);
                    //we ended the previus sub

                    //start the next sub
                    _srtService.writeToSrt(srtPath, subNumber.ToString());
                    midString = startTime.printTime() + " " + "-->";
                    endTime.copyTime(startTime);
                    if (!endTime.plus(fpsAdd))
                    {
                        //there is a problem with the time
                        totalFrames = -1;
                    }

                }
                //מקרה 3
                //זמן ללא כתובית(כתום)
                else if (text == "" && prev == "")
                {
                    if (!startTime.plus(fpsAdd))
                    {
                        //there is a problem with the time
                        totalFrames = -1;
                    }
                    if (!endTime.plus(fpsAdd))
                    {
                        //there is a problem with the time
                        totalFrames = -1;
                    }

                }
                //מקרה 4
                //(ירוק)סיום כתובית ולא התחלה של כתובית
                else if (text == "" && prev != "")
                {
                    _srtService.writeToSrt(srtPath, midString + " " + endTime.printTime());
                    _srtService.writeToSrt(srtPath, text);
                    _srtService.writeToSrt(srtPath, "");
                    subNumber++;
                    startTime.copyTime(endTime);
                    if (!endTime.plus(fpsAdd))
                    {
                        //there is a problem with the time
                        totalFrames = -1;
                    }
                    //end prev sub

                }
                //מקרה 5
                //המשך של אותה כתובית(כחול במצגת)
                else if (text == prev && text != "")
                {
                    if (!endTime.plus(fpsAdd))
                    {
                        //there is a problem with the time
                        totalFrames = -1;
                    }
                }

                prev = text;
            }
            //אם הכתוביות האחרונות עד סוף הסרט
            if (text != "")
            {
                _srtService.writeToSrt(srtPath, midString + " " + endTime.printTime());
                _srtService.writeToSrt(srtPath, text);
            }

            if (totalFrames == -1)
            {
                fileId = "";
            }
            else
            {

                //save in GridFSBucket
                 fileId = _srtService.saveSRTBucket(srtPath).ToString();
            }
            

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
                resultSRT = _srtService.getSrtList(dictionaryPath).OrderByDescending(x => x.date).Where(x => x.name.ToLower().Contains(status.ToLower())).ToPagedList(pageNumber, countSRTPerPage);
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



        public IActionResult Download(string fileName, string fileId)
        {

            byte[] fileContent = _srtService.Download(ObjectId.Parse(fileId));

            return File(fileContent, "text/plain", fileName);
        }
    }
}