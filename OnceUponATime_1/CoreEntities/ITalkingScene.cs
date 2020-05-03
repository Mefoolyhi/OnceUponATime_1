using System.Collections.Generic;

namespace OnceUponATime_1
{
    public interface ITalkingScene : IScene
    {
         List<Phrase> Dialogues { get; }
         SceneType SceneType { get; }
         
    }
}