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

        public string ExtractTextFromPic(String directoryPath)
        {
            string image = directoryPath + "thumb.jpeg";
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

        public string getDictionaryPath()
        {
            string path = _env.ContentRootPath + "/wwwroot/json/categorySRT.json";

            return path;
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

            foreach (var dictionary in jsonString)
            {
                if(jsonString[dictionary.Key] !=null)
                    srts.AddRange(JsonConvert.DeserializeObject<List<Srt>>(dictionary.Value.ToString())); //all values from object 
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


        public byte[] Download(ObjectId fileId)
        {

            byte[] content = bucket.DownloadAsBytesAsync(fileId).Result;

            return content;


        }
    }
}
