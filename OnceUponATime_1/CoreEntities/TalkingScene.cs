using System.Collections.Generic;

namespace OnceUponATime_1
{
    public interface TalkingScene
    {
         List<Phrase> Dialogues { get; }
         SceneType SceneType { get; }
         string Background { get; }
    }
}