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
        private string _filename;
        private readonly string _baseDirectory;

        public JsonParser()
        {
            _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        public void SetFilenameForReading(string filename)
        {
            _fileStream = new FileStream(Path.GetFullPath(_baseDirectory + filename), FileMode.Open, FileAccess.Read);
            _streamReader = new StreamReader(_fileStream, Encoding.UTF8);
        }

        public void SetFilenameForWriting(string filename) => 
            _filename = filename;
        
        public void SaveToFile(T toThrow) =>
            File.WriteAllText(Path.GetFullPath(_baseDirectory + _filename), JsonConvert.SerializeObject(toThrow));
        
        public T GetObject() => JsonConvert.DeserializeObject<T>(_streamReader.ReadToEnd());

        public void Dispose()
        {
            _fileStream.Dispose();
            _streamReader.Dispose();
        }
    }
}