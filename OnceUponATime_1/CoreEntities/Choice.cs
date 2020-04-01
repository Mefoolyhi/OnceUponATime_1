using System.Collections.Generic;

namespace OnceUponATime_1
{
    public class Choice
    {
        public readonly string Text;
        public readonly int LogicDelta;
        public readonly int IntuitionalDelta;
        public readonly int DiamondDelta;
        public readonly Dictionary<string, int> RelationshipDelta;
        public readonly TalkingScene NextScene;

        public Choice(string text, int dLogic, int dInt, int dDiamond, Dictionary<string, int> dRel, TalkingScene nextScene)
        {
            Text = text;
            RelationshipDelta = dRel;
            DiamondDelta = dDiamond;
            LogicDelta = dLogic;
            IntuitionalDelta = dInt;
            NextScene = nextScene;
        }
    }
}