using System.Collections.Generic;

namespace OnceUponATime_1
{
    public class Series
    {
        public string Story { get;}
        public int Season { get;}
        public int SeriesNumber { get; }
        public List<Person> Persons { get; }
        public string Filename { get; }
        
        public List<Scene> Scenes { get; }

        public Series(string story, string filename, int season, int seriesNumber, List<Person> persons, List<Scene> scenes)
        {
            Season = season;
            Story = story;
            Filename = filename;
            SeriesNumber = seriesNumber;
            Persons = persons;
            Scenes = scenes;
        }
        
    }
}