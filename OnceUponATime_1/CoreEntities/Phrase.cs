using Newtonsoft.Json;

namespace OnceUponATime_1
{
    public class Phrase
    {
        [JsonProperty("Person")]
        public string Person { get; }
        
        [JsonProperty("Text")]
        public string Text { get; }

        public Phrase(string person, string text)
        {
            Text = text;
            Person = person;
        }
    }
}