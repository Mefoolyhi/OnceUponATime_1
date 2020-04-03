using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace OnceUponATime_1
{
    public class JsonParser<T> : IDisposable
    where T : class
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
            _fileStream = new FileStream(Path.GetFullPath(_baseDirectory + filename), FileMode.Open, FileAccess.Read);
            _streamReader = new StreamReader(_fileStream, Encoding.UTF8);
        }

        [Serializable]
        class JsonT
        {    
           public List<T> SList;
        }
        public List<T> Get()
        {
            var myJsonObject = JsonConvert.DeserializeObject<JsonT>(_streamReader.ReadToEnd());
            return myJsonObject.SList;
        }

        public void SetTs(string filename, List<T> toThrow)
        {
            
            File.WriteAllText(Path.Combine(_baseDirectory,filename), JsonConvert.SerializeObject(toThrow));
        }

        public void SetT(string filename, T toThrow)
        {
            File.WriteAllText(Path.Combine(_baseDirectory,filename), JsonConvert.SerializeObject(toThrow));
        }

        public T GetOneT() => JsonConvert.DeserializeObject<T>(_streamReader.ReadToEnd());

        public void Dispose()
        {
            _fileStream.Close();
            _streamReader.Close();
        }
    }
}