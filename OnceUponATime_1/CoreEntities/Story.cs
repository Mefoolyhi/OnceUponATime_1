using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace OnceUponATime_1
{
    public class Story
    {
        
        public readonly string Name;
        
        [JsonProperty("seasons")]
        private List<int> _seasons;
        public readonly MainHero Hero;
        
        //последняя ПРОЙДЕННАЯ серия
        [JsonProperty("currentSeason")]
        public int CurrentSeason { get; private set; }
        [JsonProperty("currentSeries")]
        public int CurrentSeries { get; private set; }
        
        public Story(string name, List<int> seasons, MainHero hero, int currentSeason = -1, int currentSeries = -1)
        {
            Name = name;
            _seasons = seasons;
            Hero = hero;
            CurrentSeason = currentSeason;
            CurrentSeries = currentSeries;
        }

        public List<int> Seasons => _seasons;
        
        public void SetSeries(int seasonNumber, int seriesNumber = -1)
        {
            if (seasonNumber < 0 || seasonNumber >= _seasons.Count)
                throw new InvalidDataException();
            if (seriesNumber < -1 || seriesNumber >= _seasons[seasonNumber])
                throw new InvalidDataException();
            CurrentSeason = seasonNumber;
            CurrentSeries = seriesNumber;
        }

        public void RollbackSeries() => CurrentSeries--;
        
        public string GetNextSeries()
        {
            if (CurrentSeason == -1)
            {
                CurrentSeason = 0;
                CurrentSeries = 0;
            }
            else if (CurrentSeries + 1 >= _seasons[CurrentSeason])
                if (_seasons.Count <= CurrentSeason + 1)
                    return null;
                else
                {
                    CurrentSeason++;
                    CurrentSeries = 0;
                }
            else
                CurrentSeries++;

            return $@"\series\{GameLogic.StoryName}\{(CurrentSeason + 1)}_{(CurrentSeries + 1)}.json";
        }
    }
}