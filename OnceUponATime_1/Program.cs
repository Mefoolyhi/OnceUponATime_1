using System;
using System.Collections.Generic;
using System.Threading;

namespace OnceUponATime_1
{
    public delegate void NotifyParent(string msg);
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
            var replayed = false;
            var story = new Story("Test", new List<int> {1}, new MainHero("MainHero"));
            //награда за первое посещение в день: счетчик дней и последний день когда заходили - отдельный файлик
            //в нем же про ключи и алмазы
            //варнинг про 10 ключей
            while (true)
            {
                if (!replayed)
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
                    story = stories[int.Parse(s)];
                }

                //снять ключ
                var gl = new GameLogic(story);
                var computing = new Thread(gl.ProcessSerie);
                gl.Stop += (msg) =>
                {
                    if (msg.Equals("restart"))
                        replayed = true;
                    if (msg.Equals("ended"))
                        replayed = false;
                    computing.Abort();
                };
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