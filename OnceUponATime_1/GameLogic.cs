using System;

namespace OnceUponATime_1
{
    public class GameLogic
    {
        public static string StoryName;
        private readonly Story _story;
        private readonly JsonParser jp;

        public GameLogic(Story story)
        {
            _story = story;
            StoryName = story.Name;
            jp = new JsonParser();
            ProcessSerie(story.GetNextSeries());
        }

        private void ProcessSerie(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                Console.WriteLine("Oooops We don't have series");
            
        }
        
    }
}