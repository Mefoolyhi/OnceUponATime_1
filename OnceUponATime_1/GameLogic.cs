using System;
using System.Collections.Generic;
using System.Linq;

namespace OnceUponATime_1
{
    public class GameLogic
    {
        public event NotifyParent Stop;    
        public static string StoryName;
        private readonly Story _story;
        private readonly JsonParser<Scene> _jp;
        private int _logicDelta;
        private int _intuitionDelta;
        private int _diamondDelta;


        public GameLogic(Story story)
        {
            _story = story;
            StoryName = story.Name;
            _jp = new JsonParser<Scene>();
        }

        private void Menu()
        {
            Console.WriteLine("e - exit, c - continue, r - replay");
            var ans = Console.ReadLine();
            if (ans == null || (!ans.Equals("e") && !ans.Equals("r"))) return;
            _story.RollbackSeries();
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
        
        private void PlayScene(TalkingScene s)
        {
            var cheated = false;
            switch (s.SceneType)
            {
                case SceneType.Logic:
                    Console.WriteLine("Path of Logic");
                    break;
                case SceneType.Intuitional:
                    Console.WriteLine("Path of Intuition");
                    break;
            }

            foreach (var phrase in s.Dialogues)
            {
                if (string.IsNullOrEmpty(phrase.Person))
                    Console.WriteLine(phrase.Text);
                else
                {
                    var person = DecodeName(phrase.Person);
                    Console.WriteLine($@"{person}: ""{phrase.Text}""");
                    if (s.SceneType == SceneType.Love && 
                        !person.Equals(_story.Hero.Name) &&
                        !person.Equals(_story.Hero.MainLover) &&
                        _story.Hero.MaxSympathy >= 10)
                        cheated = true;
                }
                var ans = Console.ReadLine();
                if (ans != null && ans.Equals("m"))
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
            Console.WriteLine($@"Diamonds: {_diamondDelta}"); //алмазы написать
            _story.Hero.SetLogicIntuition(_logicDelta, _intuitionDelta);
            //накинуть алмазов за прохождение
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
                return;
            }
            _jp.SetFilename(filename);
            var scenesList = _jp.Get();
            _jp.Dispose();
            foreach (var scene in scenesList)
            {
                if (scene.SceneType == SceneType.None)
                {
                    ChoiceScene s = scene;
                    var person = DecodeName(s.Hero);
                    Console.WriteLine($"{person} : ");
                    foreach (var d in s.Choices.Select(choice => choice.DiamondDelta == 0 ? choice.Text : 
                        string.Join(" ", choice.Text, 
                            Math.Abs(choice.DiamondDelta).ToString())))
                    {
                        Console.WriteLine(d);
                    }
                    
                    
                    var ans = Console.ReadLine();
                    if (ans != null && ans.Equals("m"))
                    {
                        Menu();
                        ans = Console.ReadLine();
                    }

                    var c = s.Choices[int.Parse(ans)];
                    _diamondDelta += c.DiamondDelta;
                    //снять алмазы
                    _logicDelta += c.LogicDelta;
                    _intuitionDelta += c.IntuitionalDelta;
                    _story.Hero.SetSympathies(c.RelationshipDelta);
                    PlayScene(c.NextScene);
                }
                else
                {
                    TalkingScene s = scene;
                    if ((s.SceneType == SceneType.Logic && _story.Hero.Intuition + _intuitionDelta > _story.Hero.Logic + _logicDelta) ||
                        (s.SceneType == SceneType.Intuitional && _story.Hero.Logic + _logicDelta >= _story.Hero.Intuition + _intuitionDelta))
                        continue;
                    PlayScene(scene);
                }
            }
            EndSerie();
        }
    }
}