using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    static class Program
    {
        public static Player Player = new Player(0, 0);

        private static void EndConsoleVersion(List<Story> stories)
        {
            var storyParser = new JsonParser<List<Story>>();
            storyParser.SetFilenameForWriting(@"\StoriesConfig.json");
            storyParser.SaveToFile(stories);

            var playerParser = new JsonParser<Player>();
            playerParser.SetFilenameForWriting(@"\GameConfig.json");
            playerParser.SaveToFile(Player);
        }

        private static void PlayConsole()
        {
            Console.WriteLine("Loading...");
            var storyParser = new JsonParser<List<Story>>();
            storyParser.SetFilenameForReading(@"\StoriesConfig.json");
            var stories = storyParser.GetObject();
            storyParser.Dispose();
            var replayed = false;
            var story = stories[0];
            var playerParser = new JsonParser<Player>();
            playerParser.SetFilenameForReading(@"\GameConfig.json");
            Player = playerParser.GetObject();
            playerParser.Dispose();
            if (Player.TryUpdateLastVisitAndDaysCountRecords())
            {
                Console.WriteLine("Спасибо, что снова с нами");
                Console.WriteLine("Подарок: 3 ключа и 25 алмазов");
                Player.AddDiamonds(25);
                Console.WriteLine($"Теперь алмазов {Player.Diamonds}");
                if (!Player.TrySetKeys(3))
                    Console.WriteLine("Слишком много ключей. Мы оставили 10");
                Console.WriteLine($"И ключей {Player.Keys}");
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
                    var ans = Console.ReadLine();
                    if (string.IsNullOrEmpty(ans))
                    {
                        EndConsoleVersion(stories);
                        break;
                    }
                    story = stories[int.Parse(ans)];
                }

                if (!Player.TryDecreaseKeys())
                {
                    Console.WriteLine("We don't have any keys!");
                    continue;
                }
                Console.WriteLine($"И ключей {Player.Keys}");
                var gl = new GameLogic(story);
                var computing = new Thread(gl.ProcessSerie);
                gl.Stop += msg =>
                {
                    replayed = msg.Equals("restart");
                    computing.Abort();
                };
                computing.Start();
                computing.Join();
            }
            EndConsoleVersion(stories);
        }

        [STAThread]
        public static void Main()
        {
            //properties - app time - console to win
            //без этого формочки не заработают
            
            //чтобы поиграть в консольную версию
            //верни все в консольное приложение
            //заккоментируй формочки
            //раскомментируй это
            //PlayConsole();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MyForm());
        }
    }
}