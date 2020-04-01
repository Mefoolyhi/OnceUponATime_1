using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OnceUponATime_1
{
    public class JsonParser : IDisposable
    {
        private string Filename;
        private string BaseDirectory;

        public JsonParser(string baseDirectory) => BaseDirectory = baseDirectory;

        public void SetFilename(string filename)
        {
            Filename = filename;
        }

        public Series GetNext()
        {

            return JsonConvert.DeserializeObject<Series>(File.ReadAllText(Path.Combine(BaseDirectory,Filename)));

        }
        

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}