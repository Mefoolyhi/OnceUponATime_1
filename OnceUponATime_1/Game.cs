using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OnceUponATime_1
{
    public class Game
    {
        private GameStage stage = GameStage.Loading;
        public Player Player;
        public event Action<string> Stop;
        private readonly JsonParser<List<Scene>> _scenesParser;
        private int _logicDelta;
        private int _diamondsDelta;
        private int _intuitionDelta;
        public bool Replayed = false;


        private List<Story> stories;
        private int currentStoryNumber = 0;
        public string StoryName;
        private Story story { get; set; }
        public List<Scene> Scenes;
        public List<Phrase> Phrases;
        private int currentSceneNumber = 0;
        private int currentPhraseNumber = 0;
        public Scene CurrentScene;
        public Phrase CurrentPhrase;
        public string CurrentPerson;
        public bool isEndOfSerie = false;

        public Game()
        {
            _scenesParser = new JsonParser<List<Scene>>();
        }

        public int StoriesCount => stories.Count;
        public int LogicDelta => _logicDelta;
        public int DiamondsDelta => _diamondsDelta;
        public int IntuitionDelta => _intuitionDelta;
        public Story Story => story;
        public GameStage Stage => stage;
        public event Action<GameStage> StageChanged;
        private void ChangeStage(GameStage stage)
        {
            this.stage = stage;
            StageChanged?.Invoke(stage);
        }

        public event Action NoSerie;
        public event Action GetGift;
        public event Action NoPlace;
        public event Action StatesUpdeted;
        public event Action NoKeys;
        public event Action NameEntering;
        private void SayNoSerie() => NoSerie?.Invoke();
        private void GetDailyGift() => GetGift?.Invoke();
        private void SayNoPlace() => NoPlace?.Invoke();
        private void UpdateStates() => StatesUpdeted?.Invoke();
        private void SayNoKeys() => NoKeys?.Invoke();
        private void EnterName() => NameEntering?.Invoke();

        Task<List<Story>> LoadingInThread()
        {
            var task = new Task<List<Story>>(
                () =>
                {
                    var storyParser = new JsonParser<List<Story>>();
                    storyParser.SetFilenameForReading(@"\StoriesConfig.json");
                    var stories = storyParser.GetObject();
                    storyParser.Dispose();
                    var playerParser = new JsonParser<Player>();
                    playerParser.SetFilenameForReading(@"\GameConfig.json");
                    Player = playerParser.GetObject();
                    playerParser.Dispose();
                    Thread.Sleep(3000);
                    return stories;
                });
            task.Start();
            return task;
        }

        public async void Loading()
        {
            ChangeStage(GameStage.Loading);
            stories = await LoadingInThread();
            story = stories[currentStoryNumber];
            StoryName = story.Name;
            ChangeStage(GameStage.Main);
            if (Player.TryUpdateLastVisitAndDaysCountRecords())
            {
                GetDailyGift();
                Player.AddDiamonds(25);
                if (!Player.TrySetKeys(3))
                    SayNoPlace();
            }
        }

        public void GetNextStory()
        {
            currentStoryNumber = (currentStoryNumber + 1) % stories.Count;
            story = stories[currentStoryNumber];
            StoryName = story.Name;
        }

        public void GetPreviousStory()
        {
            currentStoryNumber = (currentStoryNumber - 1 + stories.Count) % stories.Count;
            story = stories[currentStoryNumber];
            StoryName = story.Name;
        }

        Task<List<Scene>> LoadSerieInThread()
        {
            var task = new Task<List<Scene>>(
                () =>
                {
                    var filename = story.GetNextSeries();
                    var scenes = new List<Scene>();
                    if (string.IsNullOrEmpty(filename))
                    {
                        return null;
                    }
                    else
                    {
                        using (_scenesParser)
                        {
                            _scenesParser.SetFilenameForReading(filename);
                            scenes = _scenesParser.GetObject();
                        };
                        Thread.Sleep(3000);
                    }
                    return scenes;
                });
            task.Start();
            return task;
        }

        public async void PlayGame()
        {
            if (!Player.TryDecreaseKeys())
            {
                SayNoKeys();
                return;
            }
            UpdateStates();
            StageChanged(GameStage.Loading);
            Scenes = await LoadSerieInThread();
            if (Scenes is null)
            {
                SayNoSerie();
                Player.TrySetKeys(1);
                UpdateStates();
                ChangeStage(GameStage.Main);
                return;
            }
            CurrentScene = Scenes[currentSceneNumber];
            Phrases = CurrentScene.Dialogues;
            CurrentPhrase = Phrases[currentPhraseNumber];
            CurrentPerson = DecodeName(CurrentPhrase.Person);
            StageChanged(GameStage.Game);
            if ((story.CurrentSeason == 0 ||
                (story.CurrentSeason == 0 && story.CurrentSeries == 0)))
            {
                EnterName();
            }
        }

        public void SetName(string name)
        {
            story.Hero.Name = name;
        }

        public void GetNextPhrase()
        {
            if (currentPhraseNumber == Phrases.Count -1)
            {
                GetNextScene();
                return;
            }
            currentPhraseNumber++;
            CurrentPhrase = Phrases[currentPhraseNumber];
            CurrentPerson = DecodeName(CurrentPhrase.Person);
        }

        public void GetNextScene()
        {
            if (currentSceneNumber == Scenes.Count - 1)
            {
                isEndOfSerie = true;
                return;
            }
            currentSceneNumber++;
            CurrentScene = Scenes[currentSceneNumber];
            currentPhraseNumber = 0;
            Phrases = CurrentScene.Dialogues;
            CurrentPhrase = Phrases[currentPhraseNumber];
            CurrentPerson = DecodeName(CurrentPhrase.Person);
        }

        public void End()
        {
            var storyParser = new JsonParser<List<Story>>();
            storyParser.SetFilenameForWriting(@"\StoriesConfig.json");
            storyParser.SaveToFile(stories);

            var playerParser = new JsonParser<Player>();
            playerParser.SetFilenameForWriting(@"\GameConfig.json");
            playerParser.SaveToFile(Player);
        }

        public void ReturnToMainScreen()
        {
            ChangeStage(GameStage.Main);
        }

        public void RestartSerie()
        {
            story.RollbackSeries();
            Player.AddDiamonds(_diamondsDelta);
            if (story.CurrentSeason == 0 && story.CurrentSeries == -1)
                story.Hero.Name = "MainHero";
            PlayGame();
        }

        public void ExitFromSerie()
        {
            story.RollbackSeries();
            Player.AddDiamonds(_diamondsDelta);
            if (story.CurrentSeason == 0 && story.CurrentSeries == -1)
                story.Hero.Name = "MainHero";
            ReturnToMainScreen();
        }

        private void Menu()
        {
            Console.WriteLine("e - exit, c - continue, r - replay");
            var ans = Console.ReadLine();
            if (ans == null || (!ans.Equals("e") && !ans.Equals("r"))) return;
            story.RollbackSeries();
            Program.Player.AddDiamonds(_diamondsDelta);
            if (story.CurrentSeason == 0 && story.CurrentSeries == -1)
                story.Hero.Name = "MainHero";
            Stop(ans.Equals("r") ? "restart" : "kill me");
        }

        private string DecodeName(string name)
        {
            var person = name;
            if (person.Equals("MainHero"))
                person = story.Hero.Name;
            if (person.Equals("MainLover"))
                person = story.Hero.MainLover;
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
                        !person.Equals(story.Hero.Name) &&
                        !person.Equals(story.Hero.MainLover) &&
                        story.Hero.MaxSympathy >= 10)
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
            story.Hero.SetSympathies(new Dictionary<string, int> { { "MainLover", -2 } });
        }

        public void EndSerie()
        {
            ChangeStage(GameStage.Finished);
            if (!story.Hero.TrySetLogicIntuition(_logicDelta, _intuitionDelta))
                Console.WriteLine("WARNING");
            Player.AddDiamonds(5);
            UpdateStates();
        }

        public void ProcessSerie()
        {
            if (story.CurrentSeason == -1 ||
                (story.CurrentSeason == 0 && story.CurrentSeries == -1))
            {
                Console.WriteLine("Введите имя и нажмите enter");
                story.Hero.Name = Console.ReadLine();
            }
            var filename = story.GetNextSeries();
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
                    story.Hero.SetSympathies(selectedOption.RelationshipDelta);
                    foreach (var nextScene in selectedOption.NextScenes)
                        PlayScene(nextScene);
                }
                else
                {
                    ITalkingScene currentScene = scene;
                    if ((currentScene.SceneType == SceneType.Logic && story.Hero.Intuition + _intuitionDelta > story.Hero.Logic + _logicDelta) ||
                        (currentScene.SceneType == SceneType.Intuitional && story.Hero.Logic + _logicDelta >= story.Hero.Intuition + _intuitionDelta))
                        continue;
                    PlayScene(scene);
                }
            }
            EndSerie();
        }
    }
}