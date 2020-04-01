using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace OnceUponATime_1
{
    public class Story
    {
        public readonly string Name;
        public readonly MainHero Hero;
        [JsonProperty("seasons")]
        private List<int> _seasons;
        [JsonProperty("currentSeason")]
        private int _currentSeason;
        [JsonProperty("currentSeries")]
        private int _currentSeries;
        
        public Story(string name, List<int> seasons, MainHero hero, int currentSeason = 0, int currentSeries = 0)
        {
            Name = name;
            _seasons = seasons;
            Hero = hero;
            _currentSeason = currentSeason;
            _currentSeries = currentSeries;
        }
        
        public void SetSeries(int seasonNumber, int seriesNumber = 0)
        {
            if (seasonNumber < 1 || seasonNumber > _seasons.Count)
                throw new InvalidDataException();
            if (seriesNumber < 1 || seriesNumber > _seasons[seasonNumber])
                throw new InvalidDataException();
            _currentSeason = seasonNumber;
            _currentSeries = seriesNumber;
        }
        
        public string GetNextSeries()
        {
            if (_currentSeason == 0)
            {
                _currentSeason = 1;
                _currentSeries = 1;
            }
            else if (_currentSeries >= _seasons[_currentSeason - 1])
                if (_seasons.Count <= _currentSeason)
                    return null;
                else
                {
                    _currentSeason++;
                    _currentSeries = 1;
                }
            else
                _currentSeries++;

            return $@"\series\{GameLogic.StoryName}\{_currentSeason}_{_currentSeries}.json";
        }
    }
}