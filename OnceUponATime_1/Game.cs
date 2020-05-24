using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OnceUponATime_1
{
    public class Game
    {
        public Player Player;
        public int StoriesNumber => _stories.Count;
        public Scene CurrentScene { get; private set; }
        public int LogicDelta { get; private set; }
        public int IntuitionDelta { get; private set; }
        private Story _story { get; set; }
        public string StoryName;
        private Queue<Scene> _storyQueue;
        private IEnumerator<Phrase> _enumerator;
        private readonly JsonParser<List<Scene>> _scenesParser;
        private int _diamondsDelta;
        private List<Story> _stories;
        private int _currentStoryNumber;
        private List<Scene> _scenes;
        private bool _cheated;
        private List<Choice> _choices;
        private int _currentSceneNumber = -1;
        private bool _isEnd = false;
        
        public Game()
        {
            _scenesParser = new JsonParser<List<Scene>>();
            _storyQueue = new Queue<Scene>();
        }

        public Story Story => _story;
        
        public string DecodeName(string name)
        {
            var person = name;
            if (person.Equals("MainHero"))
                person = _story.Hero.Name;
            if (person.Equals("MainLover"))
                person = _story.Hero.MainLover;
            return person;
        }

        public event Action NoSerie;
        public event Action GetGift;
        public event Action NoPlace;
        public event Action StatesUpdated;
        public event Action NoKeys;
        public event Action NoDiamonds;
        public event Action SceneIsLogic;
        public event Action SceneIsIntuitional;
        public event Action YouCheated;
        public event Action<string> RelationshipsUpgraded;
        public event Action<string> RelationshipsDestroyed;
        public event Action<int> DiamondsTaken;
        public event Action<int> LogicIncrease;
        public event Action<int> IntuitionalIncreased;
        public event Action NameEntering;
        public event Action<GameStage> StageChanged;
        private void SayNoSerie() => NoSerie?.Invoke();
        private void GetDailyGift() => GetGift?.Invoke();
        private void SayNoPlace() => NoPlace?.Invoke();
        private void UpdateStates() => StatesUpdated?.Invoke();
        private void SayNoKeys() => NoKeys?.Invoke();
        private void EnterName() => NameEntering?.Invoke();
        private void ChangeStage(GameStage stage) => StageChanged?.Invoke(stage);

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
            _stories = await LoadingInThread();
            _story = _stories[_currentStoryNumber];
            StoryName = _story.Name;
            ChangeStage(GameStage.Main);
            if (!Player.TryUpdateLastVisitAndDaysCountRecords()) return;
            GetDailyGift();
            Player.AddDiamonds(25);
            if (!Player.TrySetKeys(3))
                SayNoPlace();
        }

        public void GetNextStory()
        {
            _currentStoryNumber = (_currentStoryNumber + 1) % _stories.Count;
            _story = _stories[_currentStoryNumber];
            StoryName = _story.Name;
        }

        public void GetPreviousStory()
        {
            _currentStoryNumber = (_currentStoryNumber - 1 + _stories.Count) % _stories.Count;
            _story = _stories[_currentStoryNumber];
            StoryName = _story.Name;
        }

        Task<List<Scene>> LoadSerieInThread()
        {
            var task = new Task<List<Scene>>(
                () =>
                {
                    var filename = _story.GetNextSeries();
                    List<Scene> scenes;
                    if (string.IsNullOrEmpty(filename))
                    {
                        return null;
                    }

                    using (_scenesParser)
                    {
                        _scenesParser.SetFilenameForReading(filename);
                        scenes = _scenesParser.GetObject();
                    }

                    Thread.Sleep(3000);
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
            _scenes = await LoadSerieInThread();
            if (_scenes is null)
            {
                SayNoSerie();
                Player.TrySetKeys(1);
                UpdateStates();
                ChangeStage(GameStage.Main);
                return;
            }
            _cheated = false;
            _currentSceneNumber = -1;
            _storyQueue = new Queue<Scene>();
            _enumerator = null;
            LogicDelta = 0;
            _diamondsDelta = 0;
            IntuitionDelta = 0;
            SetNextScene();
            StageChanged(GameStage.Game);
            if (_story.CurrentSeason == 0)
                EnterName();
        }
        
        public void EndSerie()
        {
            ChangeStage(GameStage.Finished);
            _story.Hero.TrySetLogicIntuition(LogicDelta, IntuitionDelta);
            Player.AddDiamonds(5);
            UpdateStates();
        }

        public void SetName(string name)
        {
            _story.Hero.Name = name;
        }

        public bool UpdateChoiceSuccess(int choiceIndex)
        {
            var selectedOption = _choices[choiceIndex];
            if (!Player.TryRemoveDiamonds(selectedOption.DiamondDelta))
            {
                NoDiamonds?.Invoke();
                return false;
            }
            _diamondsDelta += selectedOption.DiamondDelta;
            if (selectedOption.DiamondDelta != 0)
                DiamondsTaken?.Invoke(selectedOption.DiamondDelta);

            LogicDelta += selectedOption.LogicDelta;
            if (selectedOption.LogicDelta != 0)
                LogicIncrease?.Invoke(selectedOption.LogicDelta);

            IntuitionDelta += selectedOption.IntuitionalDelta;
            if (selectedOption.IntuitionalDelta != 0)
                IntuitionalIncreased?.Invoke(selectedOption.IntuitionalDelta);

            _story.Hero.SetSympathies(selectedOption.RelationshipDelta);
            foreach (var nextScene in selectedOption.NextScenes)
                AddScene(nextScene);
            SetNextScene();
            return true;
        }

        private void AddScene(Scene scene)
        {
            _storyQueue.Enqueue(scene);
        }

        private void SetNextScene()
        {
            _cheated = false;
            if (_storyQueue.Count == 0)
                AddNextScene();
            if (_storyQueue.Count == 0)
            {
                _isEnd = true;
                EndSerie();
                return;
            }
            CurrentScene = _storyQueue.Dequeue();
            if (CurrentScene.Choices != null)
            {
                _choices = CurrentScene.Choices;
                _enumerator = null;
            }
            else
                _enumerator = CurrentScene.Dialogues.GetEnumerator();
        }

        private Phrase GetNextPhrase()
        {
            if (_enumerator == null)
                return null;
            return _enumerator.MoveNext() ? _enumerator.Current : null;
        }
        
        public object GetNext()
        {
            var phrase = GetNextPhrase();
            while (phrase == null)
            {
                if (_enumerator == null)
                    return _choices;
                
                if (_cheated)
                    YouCheated?.Invoke();
                SetNextScene();
                if (_isEnd)
                    return null;
                phrase = GetNextPhrase();
            }
            if (CurrentScene.SceneType == SceneType.Love &&
                !phrase.Person.Equals(_story.Hero.Name) &&
                !phrase.Person.Equals(_story.Hero.MainLover) &&
                _story.Hero.MaxSympathy >= 10)
                _cheated = true;
            return phrase;
        }

        private void AddNextScene()
        {
            Scene currentScene;
            do
            {
                if (_currentSceneNumber == _scenes.Count - 1)
                {
                    return;
                }
                
                _currentSceneNumber++;
                currentScene = _scenes[_currentSceneNumber];

            } while ((currentScene.SceneType == SceneType.Logic &&
                      _story.Hero.Intuition + IntuitionDelta > _story.Hero.Logic + LogicDelta) ||
                     (currentScene.SceneType == SceneType.Intuitional &&
                      _story.Hero.Logic + LogicDelta >= _story.Hero.Intuition + IntuitionDelta));
            switch (currentScene.SceneType)
            {
                case SceneType.Logic:
                    SceneIsLogic?.Invoke();
                    break;
                case SceneType.Intuitional:
                    SceneIsIntuitional?.Invoke();
                    break;
            }
           AddScene(currentScene);
        }

        public void End()
        {
            var storyParser = new JsonParser<List<Story>>();
            storyParser.SetFilenameForWriting(@"\StoriesConfig.json");
            storyParser.SaveToFile(_stories);

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
            InterruptSerie();
            PlayGame();
        }

        public void ExitFromSerie()
        {
            InterruptSerie();
            ReturnToMainScreen();
        }

        private void InterruptSerie()
        {
            _story.RollbackSeries();
            Player.AddDiamonds(_diamondsDelta);
            if (_story.CurrentSeason == 0 && _story.CurrentSeries == -1)
                _story.Hero.Name = "MainHero";
            _currentSceneNumber = -1;
            _storyQueue = new Queue<Scene>();
            _enumerator = null;
            LogicDelta = 0;
            _diamondsDelta = 0;
            IntuitionDelta = 0;
        }
    }
}