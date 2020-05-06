using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnceUponATime_1
{
    public class Game
    {
        public Player Player;
        private readonly JsonParser<List<Scene>> _scenesParser;
        private int _logicDelta = 0;
        private int _diamondsDelta = 0;
        private int _intuitionDelta = 0;


        private List<Story> _stories;
        public int StoriesNumber => _stories.Count;
        private int _currentStoryNumber;
        public string StoryName;
        private Story _story { get; set; }
        private List<Scene> _scenes;
        private List<Phrase> _phrases;
        private int _currentPhraseNumber;
        public Phrase CurrentPhrase;
        public string CurrentPerson;
        private bool _cheated;
        private List<Choice> _choices;
        private int _currentChoiceNumber;
        private int _currentSceneNumber;
        public Scene CurrentScene;
        private bool _isEndOfSerie;


        public List<Choice> CurrentChoices => _choices.ToList();

        public Game()
        {
            _scenesParser = new JsonParser<List<Scene>>();
        }

        public int LogicDelta => _logicDelta;
        public int IntuitionDelta => _intuitionDelta;
        public Story Story => _story;
        public event Action<GameStage> StageChanged;

        private void ChangeStage(GameStage stage)
        {
            StageChanged?.Invoke(stage);
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

        public event Action NoSerie;
        public event Action GetGift;
        public event Action NoPlace;
        public event Action StatesUpdated;
        public event Action NoKeys;
        public event Action NoDiamonds;
        public event Action SceneIsLogic;
        public event Action SceneIsIntuitional;
        public event Action YouCheated;
        public event Action NameEntering;
        private void SayNoSerie() => NoSerie?.Invoke();
        private void GetDailyGift() => GetGift?.Invoke();
        private void SayNoPlace() => NoPlace?.Invoke();
        private void UpdateStates() => StatesUpdated?.Invoke();
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

            CurrentScene = _scenes[_currentSceneNumber];
            _phrases = CurrentScene.Dialogues;
            _choices = CurrentScene.Choices;
            _cheated = false;
            StageChanged(GameStage.Game);
            if (_story.CurrentSeason == 0)
                EnterName();
        }
        
        public void EndSerie()
        {
            ChangeStage(GameStage.Finished);
            if (!_story.Hero.TrySetLogicIntuition(_logicDelta, _intuitionDelta))
                Console.WriteLine("WARNING");
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
            _logicDelta += selectedOption.LogicDelta;
            _intuitionDelta += selectedOption.IntuitionalDelta;
            _story.Hero.SetSympathies(selectedOption.RelationshipDelta);
            return true;
            
            // foreach (var nextScene in selectedOption.NextScenes)
            //     PlayScene(nextScene);
        }
        
        public void GetChoices()
        {
            if (_currentChoiceNumber == _choices.Count - 1)
            {
                GetNextScene();
                return;
            }
            CurrentPerson = DecodeName(CurrentScene.Hero);
        }

        public void GetNextPhrase()
        {
            if (_currentPhraseNumber == _phrases.Count - 1)
            {
                if (_cheated)
                    YouCheated?.Invoke();
                GetNextScene();
                return;
            }

            _currentPhraseNumber++;
            CurrentPhrase = _phrases[_currentPhraseNumber];
            CurrentPerson = DecodeName(CurrentPhrase.Person);
            if (CurrentScene.SceneType == SceneType.Love &&
                !CurrentPerson.Equals(_story.Hero.Name) &&
                !CurrentPerson.Equals(_story.Hero.MainLover) &&
                _story.Hero.MaxSympathy >= 10)
                _cheated = true;
        }

        public bool IsChoiceScene => CurrentScene.Choices != null;

        private void GetNextScene()
        {
            if (_currentSceneNumber == _scenes.Count - 1)
            {
                _isEndOfSerie = true;
                return;
            }
            

            _currentSceneNumber++;
            CurrentScene = _scenes[_currentSceneNumber];
            
            while ((CurrentScene.SceneType == SceneType.Logic &&
                    _story.Hero.Intuition + _intuitionDelta > _story.Hero.Logic + _logicDelta) ||
                   (CurrentScene.SceneType == SceneType.Intuitional &&
                    _story.Hero.Logic + _logicDelta >= _story.Hero.Intuition + _intuitionDelta))
            {
                if (_currentSceneNumber == _scenes.Count - 1)
                {
                    _isEndOfSerie = true;
                    return;
                }
            

                _currentSceneNumber++;
                CurrentScene = _scenes[_currentSceneNumber];
            }
            switch (CurrentScene.SceneType)
            {
                case SceneType.Logic:
                    SceneIsLogic?.Invoke();
                    break;
                case SceneType.Intuitional:
                    SceneIsIntuitional?.Invoke();
                    break;
            }
            _phrases = CurrentScene.Dialogues;
            _choices = CurrentScene.Choices;
            _cheated = false;
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
            _story.RollbackSeries();
            Player.AddDiamonds(_diamondsDelta);
            if (_story.CurrentSeason == 0 && _story.CurrentSeries == -1)
                _story.Hero.Name = "MainHero";
            _currentSceneNumber = 0;
            _logicDelta = 0;
            _diamondsDelta = 0;
            _intuitionDelta = 0;
            PlayGame();
        }

        public void ExitFromSerie()
        {
            _story.RollbackSeries();
            Player.AddDiamonds(_diamondsDelta);
            if (_story.CurrentSeason == 0 && _story.CurrentSeries == -1)
                _story.Hero.Name = "MainHero";
            ReturnToMainScreen();
        }
    }
}