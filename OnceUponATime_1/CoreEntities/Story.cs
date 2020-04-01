using System;
using System.Collections.Generic;

namespace OnceUponATime_1
{
    public class Story
    {
        public readonly string Name;
        private List<List<string>> _seasons;
        private int _currentSeason = 0;
        private int _currentSeries = -1;
        
        public Story(string name, List<List<string>> seasons)
        {
            Name = name;
            _seasons = seasons;
        }

        public void AddSeason(List<string> season) => _seasons.Add(season);

        public void SetSeries(int seasonNumber, int seriesNumber = 0)
        {
            _currentSeason = seasonNumber;
            _currentSeries = seriesNumber;
        }
        
        public string GetNextSeries()
        {
            if (_currentSeries + 1 >= _seasons[_currentSeason].Count)
                if (_seasons.Count <= _currentSeason + 1)
                    return null;
                else
                {
                    _currentSeason++;
                    _currentSeries = 0;
                }
            else
                _currentSeries++;
            return _seasons[_currentSeason][_currentSeries]; //пока возвращается строка. Допишу парсер, буду кидать серию (??)
        }
    }
}