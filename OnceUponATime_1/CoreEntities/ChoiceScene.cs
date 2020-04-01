using System.Collections.Generic;

namespace OnceUponATime_1
{
    public class ChoiceScene : IScene
    {
        public string Background { get; }
        public string Hero { get; }
        public List<Choice> Choices { get; }
        
        
    }
}