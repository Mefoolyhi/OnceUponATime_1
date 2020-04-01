using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Console.WriteLine("Loading...");
            var jp = new JsonParser(); 
            while (true)
            {
                jp.SetFilename("StoriesConfig.json");
                var stories = jp.GetStories();
                jp.Dispose();
                Console.WriteLine("Choose story or press Enter to exit");
                Console.WriteLine(string.Join(" ", stories));
                var s = Console.ReadLine();
                if (string.IsNullOrEmpty(s))
                    break;

                var story = stories[int.Parse(s)];
                var gl = new GameLogic(story);
            }
            /*
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            */
        }
    }
}