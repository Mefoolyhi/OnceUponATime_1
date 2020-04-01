using System.Collections.Generic;

namespace OnceUponATime_1
{
    public class TalkingScene : IScene
    {
        public List<Phrase> Dialogues { get; }
        public SceneType SceneType { get; }
        public string Background { get; }
    }
}