using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OnceUponATime_1
{
    public class JsonParser : IDisposable
    {
        private StreamReader _streamReader;
        private FileStream _fileStream;
        private readonly string _baseDirectory;

        public JsonParser()
        {
            _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        public void SetFilename(string filename)
        {
            _fileStream = new FileStream(Path.Combine(_baseDirectory, filename), FileMode.Open, FileAccess.Read);
            _streamReader = new StreamReader(_fileStream, Encoding.UTF8);
        }

        class JsonStories
        {    
           public List<Story> Stories;
        }
        public List<Story> GetStories()
        {
            var myJsonObject = JsonConvert.DeserializeObject<JsonStories>(_streamReader.ReadToEnd());
            return myJsonObject.Stories;
        }


        /* public IScene GetNextScene()
         {
              
         }
         
 */
        public void Dispose()
        {
            _fileStream.Close();
            _streamReader.Close();
        }
    }
}