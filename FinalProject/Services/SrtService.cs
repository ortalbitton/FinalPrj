using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Diagnostics;

namespace FinalProject.Services
{
    public class SrtService
    {
        private IHostingEnvironment _env;

        public SrtService(IHostingEnvironment env)
        {
            _env = env;
        }


        public void deleteDir(string directoryPath)
        {
            Directory.Delete(directoryPath, true);
        }

        public string getSrtPath(IFormFile videoFile, string directoryPath)
        {

            string nameOfSrt = Path.GetFileNameWithoutExtension(videoFile.FileName) + ".srt";
            nameOfSrt = nameOfSrt.Replace(" ", "_");

            return directoryPath + nameOfSrt;
        }

        public void extractPic(string videoName, int frameNumber, string directoryPath)
        {

            Process myProcess = new Process();
            //argument - the command
            //workingDirectory - where to save the picture
            var startInfo = new ProcessStartInfo
            {
                FileName = _env.ContentRootPath + "/wwwroot/ffmpeg/ffmpeg.exe",
                Arguments = "-y -i " + videoName + " -vf select='eq(n," + frameNumber + ")' -vsync vfr " + directoryPath + "thumb.jpeg",
                WorkingDirectory = directoryPath,
                CreateNoWindow = true,
                UseShellExecute = false

            };

            using (var process = new Process { StartInfo = startInfo })
            {
                process.Start();
                process.WaitForExit();
            }

        }

        public void writeToSrt(string srtPath, string text)
        {
            //true means to append the text
            TextWriter tsw = new StreamWriter(srtPath, true, Encoding.UTF8);

            //Writing text to the file.
            tsw.WriteLine(text);

            //Close the file.
            tsw.Close();

        }

        public string getDirPath(IFormFile videoFile, string userName)
        {

            string videoName = videoFile.FileName.Replace(" ", "_");

            //create new file per user per video per time
            DateTime time = DateTime.Now;
            string t = time.Hour.ToString() + time.Minute.ToString() + time.Second.ToString();

            string directoryPath = _env.ContentRootPath + "/wwwroot/userFrames/" + t + "_" + userName + "_" + videoName + "/";

            Directory.CreateDirectory(directoryPath);
            return directoryPath;
        }

        public void saveVideo(IFormFile videoFile, string directoryPath)
        {
            try
            {
                string videoName = videoFile.FileName.Replace(" ", "_");

                //save video in the new directory
                string UploadPath = directoryPath + "/" + videoName;

                using (var stream = new FileStream(UploadPath, FileMode.Create))
                {
                    videoFile.CopyTo(stream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
