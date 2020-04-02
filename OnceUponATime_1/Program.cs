using System;
using System.Threading;

namespace OnceUponATime_1
{
    static class Program
    {
        [STAThread]
        public static void Main()
        {
            
            //properties - app time - console to win0
            Console.WriteLine("Loading...");
            var jp = new JsonParser<Story>(); 
            jp.SetFilename(@"\StoriesConfig.json");
            var stories = jp.Get();
            jp.Dispose();
            //награда за первое посещение в день: счетчик дней и последний день когда заходили - отдельный файлик
            //в нем же про ключи и алмазы
            //варнинг про 10 ключей
            while (true)
            {
                Console.WriteLine("Choose story or press Enter to exit");
                foreach (var storey in stories)
                {
                    Console.Write(storey.Name + " ");
                }
                Console.WriteLine();
                var s = Console.ReadLine();
                if (string.IsNullOrEmpty(s))
                    break;
                
                var story = stories[int.Parse(s)];
                //снять ключ
                var gl = new GameLogic(story);
                var computing = new Thread(gl.ProcessSerie);
                computing.Start();
                computing.Join();
            }
            
            
            
            /*
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            */
        }
    }
}