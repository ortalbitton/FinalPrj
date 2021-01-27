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
using MongoDB.Bson;
using Microsoft.AspNetCore.Http.Internal;
using MongoDB.Driver.GridFS;
using FinalProject.HelpClasses;
using MongoDB.Driver;
using FinalProject.Models;
using Tesseract;

namespace FinalProject.Services
{
    public class SrtService
    {
        private IHostingEnvironment _env;
        private readonly IGridFSBucket bucket;
        private readonly IMongoCollection<User> _users;

        public SrtService(IHostingEnvironment env, IDatabaseSettings settings)
        {
            _env = env;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            bucket = new GridFSBucket(database);
            _users = database.GetCollection<User>("users");

        }
        public string ExtractTextFromPic(String directoryPath, int frameNum)
        {
            string image = directoryPath + "thumb" + frameNum + ".jpg";
            string tessPath = _env.ContentRootPath + "/wwwroot/tessdata/";
            string result = "";

            using (var engine = new TesseractEngine(tessPath, "eng"))
            {
                using (var img = Pix.LoadFromFile(image))
                {
                    var page = engine.Process(img);
                    result = page.GetText();
                }
            }
            return result;

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

        public int extractPic(string videoName, string dirPath, int numOfFrames)
        {

            // 1) Create Process Info
            var psi = new ProcessStartInfo();
            psi.FileName = @"C:\Users\Owner\AppData\Local\Programs\Python\Python37\python.exe";

            // 2) Provide script and arguments

            var script = _env.ContentRootPath + "/wwwroot/python/extractPic.py";
            var directoryPath = dirPath;
            var name = videoName;


            psi.Arguments = $"\"{script}\" \"{directoryPath}\" \"{name}\" \"{numOfFrames}\"";

            // 3) Process configuration
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            // 4) Execute process and get output
            var errors = "";
            var totalFrames = "";

            using (var process = Process.Start(psi))
            {
                errors = process.StandardError.ReadToEnd();
                totalFrames = process.StandardOutput.ReadToEnd();
            }
            //errors = "ss";
            if (errors != "")
            {
                return -1;
            }
            totalFrames = totalFrames.Replace("0\r\n", "");
            totalFrames = totalFrames.Replace("\r\n", "");
            return Int32.Parse(totalFrames);
        }

        public string fixText(string text)
        {
            string mid;
            if (text != "")
            {
                text = text.ToLowerInvariant();
                
                mid = text.Substring(0, 2);
                while (mid == "\n")
                {
                    text = mid;
                    mid = text.Substring(0, 2);
                }

                mid = text.Substring(text.Length - 2, 2);
                while (mid == "\n")
                {
                    text = mid;
                    mid = text.Substring(mid.Length - 2, 2);
                }
                text = text.Replace("‘", "'");
            }
            return text;
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

        public string saveVideo(IFormFile videoFile, string directoryPath)
        {

            string videoName = videoFile.FileName.Replace(" ", "_");

            //save video in the new directory
            string UploadPath = directoryPath + videoName;//+ "/"

            try
            {
                using (var stream = new FileStream(UploadPath, FileMode.Create))
                {
                    videoFile.CopyTo(stream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return UploadPath;
        }

        public string getDictionaryPath()
        {
            string path = _env.ContentRootPath + "/wwwroot/json/categorySRT.json";

            return path;
        }

        public int getFramesTotal(string videoPath, double fps)
        {
            string duration = "";
            double totalFrames = 0;


            string ffprobPath = _env.ContentRootPath + "/wwwroot/ffmpeg/";
            var ffProbe = new NReco.VideoInfo.FFProbe();
            ffProbe.ToolPath = ffprobPath;
            var videoInfo = ffProbe.GetMediaInfo(videoPath);

            duration = videoInfo.Duration.ToString();

            string[] time = duration.Split(':', '.');

            totalFrames += Convert.ToInt32(time[0]) * 3600 * fps;
            totalFrames += Convert.ToInt32(time[1]) * 60 * fps;
            totalFrames += Convert.ToInt32(time[2]) * fps;
            totalFrames += (Convert.ToInt32(time[3]) / 10000);

            return Convert.ToInt32(totalFrames);
        }

        public double getVidFPS(string VideoPath)
        {
            string ffprobPath = _env.ContentRootPath + "/wwwroot/ffmpeg/ffprob.exe";

            var ffprob = new NReco.VideoInfo.FFProbe();
            ffprob.ToolPath = _env.ContentRootPath + "/wwwroot/ffmpeg/";

            var videoInfo = ffprob.GetMediaInfo(VideoPath);
            float fps = videoInfo.Streams[0].FrameRate;

            return Convert.ToDouble(fps);
            
        }



        public List<string> getCategoryList(string dictionaryPath)
        {
            string json = File.ReadAllText(dictionaryPath);

            JObject obj = JObject.Parse(json);

            List<string> CategoryNames = new List<string>();

            foreach (var dictionary in obj)
                CategoryNames.Add(dictionary.Key);

            return CategoryNames;

        }


        public ObjectId saveSRTBucket(string srtPath)
        {
            using (var stream = File.OpenRead(srtPath))
            {
                FormFile formFile = new FormFile(stream, 0, stream.Length, "SrtFile", Path.GetFileName(stream.Name))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "text/srt"
                };

                //get the bytes from the content stream of the file
                byte[] theFileAsBytes = new byte[formFile.Length];
                using (BinaryReader theReader = new BinaryReader(formFile.OpenReadStream()))
                {
                    theFileAsBytes = theReader.ReadBytes((int)formFile.Length);
                }

                //כיון ששמירת קבצים במונגו נשמר באחסון GridFSBucket  בתוך המונגו 
                ObjectId srtFile = bucket.UploadFromBytes(formFile.FileName, theFileAsBytes);

                return srtFile;
            }

        }

        public void setCategory(List<string> selected, Srt srt, string dictionaryPath)
        {

            // JSON string
            string json = File.ReadAllText(dictionaryPath);

            //convert JSON to object dynamic
            dynamic jsonObj = JsonConvert.DeserializeObject(json);


            foreach (var CategoryName in selected)
            {

                List<Srt> srts = jsonObj[CategoryName].ToObject<List<Srt>>();


                if (srts.Count > 0)
                    srts.Add(new Srt { name = srt.name, date=srt.date, fileId = srt.fileId });
                else
                    srts = new List<Srt> { new Srt { name = srt.name, date =srt.date, fileId = srt.fileId } };

                jsonObj[CategoryName] = JArray.FromObject(srts);//update json file
            }

            // serialize JSON directly to a file
            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(dictionaryPath, output);//write after update json file

        }


        public void addSRTToUser(string email, Srt srt)
        {

            var userIn = _users.Find(u => u.email == email).SingleOrDefault();

            if (userIn.srtList != null)
                userIn.srtList.Add(new Srt { name = srt.name,date=srt.date, fileId = srt.fileId });
            else
                userIn.srtList = new List<Srt> { new Srt { name = srt.name,date=srt.date, fileId = srt.fileId } };

            _users.ReplaceOne(p => p.email == email, userIn);

        }

        //from dictionary
        public List<Srt> getSrtList(string dictionaryPath)
        {
            string json = File.ReadAllText(dictionaryPath);

            //convert string to JSON
            JObject jsonString = JObject.Parse(json);

            List<Srt> srts = new List<Srt>();

            //in order to view all srt files that have the same file name but not the same file
            HashSet<string> knownValues = new HashSet<string>();

            foreach (var dictionary in jsonString)
            {
    
                foreach(Srt srt in JsonConvert.DeserializeObject<List<Srt>>(dictionary.Value.ToString()))
                {
                    if(knownValues.Add(srt.fileId))
                        srts.Add(srt);

                }


            }         

            return srts;

        }


        public List<Srt> searchBycategoryName(string dictionaryPath,string categoryName)
        {
            string json = File.ReadAllText(dictionaryPath);

            //convert string to JSON
            JObject jsonString = JObject.Parse(json);

            List<Srt> srts = null;

            foreach (var dictionary in jsonString)
            {

                if (dictionary.Key == categoryName)
                {
                    JArray fileSRT = (JArray)jsonString[categoryName];//values from object according to the selected key
                    srts = fileSRT.ToObject<List<Srt>>();
                    break;
                }

            }

            return srts;
        }


        byte[] content;
        public byte[] Download(ObjectId fileId)
        {

            var filter = Builders<GridFSFileInfo<ObjectId>>
                                  .Filter.Eq(x => x.Id, fileId);

            var searchResult = bucket.FindAsync(filter);

            var fileEntry = searchResult.Result.SingleOrDefault();

            if (fileEntry == null)
                content = null;
            else
                content = bucket.DownloadAsBytesAsync(fileId).Result;

            return content;


        }
    }
}
