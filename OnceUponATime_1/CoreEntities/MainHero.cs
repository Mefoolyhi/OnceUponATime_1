using System.Collections.Generic;
using Newtonsoft.Json;

namespace OnceUponATime_1
{
    public class MainHero 
    {
        public Dictionary<string, int> Sympathies { get; }
        public string MainLover { get; private set; }
        [JsonProperty("maxSympathy")]
        public int MaxSympathy { get; private set; }
        public string Name;
        public int Logic { get; private set; }
        public int Intuition { get; private set; }
        

        public MainHero(string name, int logic = 0, int intuition = 0, string mainLover = "", int maxSympathy = 0, Dictionary<string, int> symp = null)
        {
            Name = name;
            Logic = logic;
            Intuition = intuition;
            Sympathies = symp ?? new Dictionary<string, int>();
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
                if (key.Equals(MainLover) && symp.TryGetValue(key, out var d))
                    continue;
                if (key.Equals("MainLover"))
                    key = MainLover;
                if (Sympathies.TryGetValue(key, out var delta))
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