using System;
using System.Collections.Generic;

namespace OnceUponATime_1
{
    public class Scene :ITalkingScene, IChoiceScene
    {
        public List<Phrase> Dialogues { get; }
        public SceneType SceneType { get; }
        public string Background { get; }
        public string Hero { get; }
        public List<Choice> Choices { get; }

        public Scene(List<Phrase> dialogues, string sceneType, string background, string hero, List<Choice> choices)
        {
            Dialogues = dialogues;
            SceneType = Enum.TryParse(sceneType, out SceneType st) ? st : SceneType.None;
            Background = background;
            Hero = hero;
            Choices = choices;
        }
    }
}