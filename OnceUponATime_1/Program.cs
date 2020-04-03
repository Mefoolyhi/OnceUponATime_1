using System;
using System.Collections.Generic;
using System.Threading;

namespace OnceUponATime_1
{
    public delegate void NotifyParent(string msg);
    static class Program
    {
        public static Player Player;

        private static void End(List<Story> stories)
        {
            var jp = new JsonParser<Story>();
            jp.SetTs("StoriesConfig.json", stories);
            
            var pp = new JsonParser<Player>();
            pp.SetT("GameConfig.json", Player);
        }
        
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
            var story = stories[0];
            var playerParser = new JsonParser<Player>();
            playerParser.SetFilename(@"\GameConfig.json");
            Player = playerParser.GetOneT();
            if (Player.CheckIfFirstVisit())
            {
                Console.WriteLine("Спасибо, что снова с нами");
                Console.WriteLine("Подарок: 3 ключа и 25 алмазов");
                Player.SetDiamonds(25);
                if (!Player.SetKeys(3))
                    Console.WriteLine("Слишком много ключей. Мы оставили 10");
            }
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
                    {
                        End(stories);
                        break;
                    }
                    story = stories[int.Parse(s)];
                }

                if (!Player.GetKey())
                {
                    Console.WriteLine("We don't have any keys!");
                    continue;
                }
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
            End(stories);
            /*
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            */
        }
    }
}