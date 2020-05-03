using System.Collections.Generic;
using Newtonsoft.Json;

namespace OnceUponATime_1
{
    public class Choice
    {
        [JsonProperty("Text")]
        public readonly string Text;
        
        [JsonProperty("LogicDelta")]
        public readonly int LogicDelta;
        
        [JsonProperty("IntuitionalDelta")]
        public readonly int IntuitionalDelta;
        
        [JsonProperty("DiamondDelta")]
        public readonly int DiamondDelta;
        
        [JsonProperty("RelationShipDelta")]
        public readonly Dictionary<string, int> RelationshipDelta;
        
        [JsonProperty("NextScenes")]
        public readonly List<Scene> NextScenes;

        public Choice(string text, int dLogic, int dInt, int dDiamond, Dictionary<string, int> dRel, List<Scene> nextScene)
        {
            Text = text;
            RelationshipDelta = dRel;
            DiamondDelta = dDiamond;
            LogicDelta = dLogic;
            IntuitionalDelta = dInt;
            NextScenes = nextScene;
        }
    }
}