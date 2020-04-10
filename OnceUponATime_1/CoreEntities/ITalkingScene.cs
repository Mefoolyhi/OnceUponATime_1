using System.Collections.Generic;

namespace OnceUponATime_1
{
    public interface ITalkingScene
    {
         List<Phrase> Dialogues { get; }
         SceneType SceneType { get; }
    }
}