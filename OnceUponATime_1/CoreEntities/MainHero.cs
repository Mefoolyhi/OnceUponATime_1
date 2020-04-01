using System.Collections.Generic;
using Newtonsoft.Json;

namespace OnceUponATime_1
{
    public class MainHero 
    {
        public Dictionary<string, int> Sympathies { get; }
        public string MainLover { get; private set; }
        [JsonProperty("maxSympathy")]
        private int _maxSympathy;
        public readonly string Name;

        public MainHero(string name, string mainLover = "", int maxSympathy = 0, Dictionary<string, int> symp = null)
        {
            Name = name;
            Sympathies = symp ?? new Dictionary<string, int>();
            _maxSympathy = maxSympathy;
            MainLover = mainLover;
        }

        public void SetSympathies(Dictionary<string, int> symp)
        {
            foreach (var pair in symp)
            {
                if (Sympathies.TryGetValue(pair.Key, out var delta))
                    Sympathies[pair.Key] += delta;
                else 
                    Sympathies.Add(pair.Key, pair.Value);
                var value = Sympathies[pair.Key];
                if (value <= _maxSympathy) continue;
                _maxSympathy = value;
                MainLover = pair.Key;
            }
        }
    }
}