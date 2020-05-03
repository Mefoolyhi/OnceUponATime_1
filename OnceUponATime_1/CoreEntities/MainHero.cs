using System.Collections.Generic;
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

        [JsonProperty("Sympathies")]
        public Dictionary<string, int> Sympathies { get; }

        public MainHero(string name, int logic = 0, int intuition = 0, string mainLover = "", int maxSympathy = 0, Dictionary<string, int> symp = null)
        {
            Name = name;
            Logic = logic;
            Intuition = intuition;
            Sympathies = symp ?? new Dictionary<string, int>();
            MaxSympathy = maxSympathy;
            MainLover = mainLover;
        }

        public bool TrySetLogicIntuition(int logic, int intuition)
        {
            if (logic < Logic || intuition < Intuition) return false;
            Logic = logic;
            Intuition = intuition;
            return true;
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
                if (Sympathies.ContainsKey(key))
                    Sympathies[key] += pair.Value;
                else 
                    Sympathies.Add(key, pair.Value);
                var value = Sympathies[key];
                if (value <= MaxSympathy) continue;
                MaxSympathy = value;
                MainLover = key;
            }
        }
    }
}