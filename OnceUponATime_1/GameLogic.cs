using System;
using System.Collections.Generic;
using System.Linq;

namespace OnceUponATime_1
{
    public class GameLogic
    {
        public event Action<string> Stop;    
        public static string StoryName;
        private readonly Story _story;
        private readonly JsonParser<List<Scene>> _scenesParser;
        private int _logicDelta;
        private int _diamondsDelta;
        private int _intuitionDelta;


        public GameLogic(Story story)
        {
            _story = story;
            StoryName = story.Name;
            _scenesParser = new JsonParser<List<Scene>>();
        }

        private void Menu()
        {
            Console.WriteLine("e - exit, c - continue, r - replay");
            var ans = Console.ReadLine();
            if (ans == null || (!ans.Equals("e") && !ans.Equals("r"))) return;
            _story.RollbackSeries();
            Program.Player.AddDiamonds(_diamondsDelta);
            if (_story.CurrentSeason == 0 && _story.CurrentSeries == -1)
                _story.Hero.Name = "MainHero";
            Stop(ans.Equals("r") ? "restart" : "kill me");
        }

        private string DecodeName(string name)
        {
            var person = name;
            if (person.Equals("MainHero"))
                person = _story.Hero.Name;
            if (person.Equals("MainLover"))
                person = _story.Hero.MainLover;
            return person;
        }
        
        private void PlayScene(ITalkingScene scene)
        {
            var cheated = false;
            switch (scene.SceneType)
            {
                case SceneType.Logic:
                    Console.WriteLine("Path of Logic");
                    break;
                case SceneType.Intuitional:
                    Console.WriteLine("Path of Intuition");
                    break;
            }

            foreach (var phrase in scene.Dialogues)
            {
                if (string.IsNullOrEmpty(phrase.Person))
                    Console.WriteLine(phrase.Text);
                else
                {
                    var person = DecodeName(phrase.Person);
                    Console.WriteLine($@"{person}: ""{phrase.Text}""");
                    if (scene.SceneType == SceneType.Love && 
                        !person.Equals(_story.Hero.Name) &&
                        !person.Equals(_story.Hero.MainLover) &&
                        _story.Hero.MaxSympathy >= 10)
                        cheated = true;
                }
                var ans = Console.ReadLine();
                while (ans == null)
                    ans = Console.ReadLine();
                
                if (ans.Equals("m"))
                    Menu();
            }
            if (!cheated) return;
            Console.WriteLine("Вы изменили своей второй половинке!");
            _story.Hero.SetSympathies(new Dictionary<string, int>{{"MainLover", -2}});
        }

        private void EndSerie()
        {
            Console.WriteLine($@"Logic: {_story.Hero.Logic} + {_logicDelta}");
            Console.WriteLine($@"Intuition: {_story.Hero.Intuition} + {_intuitionDelta}");
            Console.WriteLine($@"Diamonds: {Program.Player.Diamonds} + {5}");
            if (!_story.Hero.TrySetLogicIntuition(_logicDelta, _intuitionDelta))
                Console.WriteLine("WARNING");
            Program.Player.AddDiamonds(5);
            Stop("ended");

        }
        
        public void ProcessSerie()
        {
            if (_story.CurrentSeason == -1 ||
                (_story.CurrentSeason == 0 && _story.CurrentSeries == -1))
            {
                Console.WriteLine("Введите имя и нажмите enter");
                _story.Hero.Name = Console.ReadLine();
            }
            var filename = _story.GetNextSeries();
            if (string.IsNullOrEmpty(filename))
            {
                Console.WriteLine("Oooops We don't have series");
                Program.Player.TrySetKeys(1);
                Console.WriteLine($"Ключей {Program.Player.Keys}");
                return;
            }

            List<Scene> scenesList;
            using (_scenesParser)
            {
                _scenesParser.SetFilenameForReading(filename);
                scenesList = _scenesParser.GetObject();
            }
            foreach (var scene in scenesList)
            {
                if (scene.SceneType == SceneType.None)
                {
                    IChoiceScene currentScene = scene;
                    var person = DecodeName(currentScene.Hero);
                    Console.WriteLine($"Алмазов {Program.Player.Diamonds}");
                    Console.WriteLine($"{person} : ");
                    foreach (var optionSqueeze in currentScene.Choices.Select(choice => choice.DiamondDelta == 0 ? choice.Text : 
                        string.Join(" ", choice.Text, 
                            Math.Abs(choice.DiamondDelta).ToString())))
                    {
                        Console.WriteLine(optionSqueeze);
                    }
                    
                    
                    var ans = Console.ReadLine();
                    while (ans == null)
                        ans = Console.ReadLine();
                    if (ans.Equals("m"))
                    {
                        Menu();
                        ans = Console.ReadLine();
                    }

                    var selectedOption = currentScene.Choices[int.Parse(ans)];
                    while (!Program.Player.TryRemoveDiamonds(selectedOption.DiamondDelta))
                    {
                        Console.WriteLine("We don't have enough diamonds! Rechoose");
                        ans = Console.ReadLine();
                        while (ans == null)
                            ans = Console.ReadLine();
                        if (ans.Equals("m"))
                        {
                            Menu();
                            ans = Console.ReadLine();
                        }
                        selectedOption = currentScene.Choices[int.Parse(ans)];
                    }

                    _diamondsDelta += selectedOption.DiamondDelta;
                    Console.WriteLine($"Алмазов {Program.Player.Diamonds}");
                    _logicDelta += selectedOption.LogicDelta;
                    _intuitionDelta += selectedOption.IntuitionalDelta;
                    _story.Hero.SetSympathies(selectedOption.RelationshipDelta);
                    foreach (var nextScene in selectedOption.NextScenes)
                        PlayScene(nextScene);
                }
                else
                {
                    ITalkingScene currentScene = scene;
                    if ((currentScene.SceneType == SceneType.Logic && _story.Hero.Intuition + _intuitionDelta > _story.Hero.Logic + _logicDelta) ||
                        (currentScene.SceneType == SceneType.Intuitional && _story.Hero.Logic + _logicDelta >= _story.Hero.Intuition + _intuitionDelta))
                        continue;
                    PlayScene(scene);
                }
            }
            EndSerie();
        }
    }
}