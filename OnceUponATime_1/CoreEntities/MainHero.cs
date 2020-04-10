using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace OnceUponATime_1
{
    public class MainHero
    {
        [JsonProperty("Name")]
        public string Name;

        [JsonProperty("Logic")]
        public int Logic { get; private set; }
        
        [JsonProperty("Intuition")]
        public int Intuition { get; private set; }
        
        [JsonProperty("MainLover")]
        public string MainLover { get; private set; }
        
        [JsonProperty("maxSympathy")]
        public int MaxSympathy { get; private set; }
        
        private Dictionary<string, int> _sympathies;
        [JsonProperty("Sympathies")]
        public Dictionary<string, int> Sympathies
        {
            get => _sympathies.ToDictionary(pair => pair.Key.Clone().ToString(),
                pair => pair.Value);
            set => _sympathies = value;
        }

        public MainHero(string name, int logic = 0, int intuition = 0, string mainLover = "", int maxSympathy = 0, Dictionary<string, int> symp = null)
        {
            Name = name;
            Logic = logic;
            Intuition = intuition;
            _sympathies = symp ?? new Dictionary<string, int>();
            MaxSympathy = maxSympathy;
            MainLover = mainLover;
        }

        public void SetLogicIntuition(int logic, int intuition)
        {
            if (logic < Logic || intuition < Intuition) return;
            Logic = logic;
            Intuition = intuition;
        }
        
        public void SetSympathies(Dictionary<string, int> symp)
        {
            foreach (var pair in symp)
            {
                var key = pair.Key;
                if (key.Equals(MainLover) && symp.ContainsKey(key))
                    continue;
                if (key.Equals("MainLover"))
                    key = MainLover;
                if (_sympathies.ContainsKey(key))
                    _sympathies[key] += pair.Value;
                else 
                    _sympathies.Add(key, pair.Value);
                var value = _sympathies[key];
                if (value <= MaxSympathy) continue;
                MaxSympathy = value;
                MainLover = key;
            }
        }
    }
}