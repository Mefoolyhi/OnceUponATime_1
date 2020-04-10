using System.Collections.Generic;

namespace OnceUponATime_1
{
    public interface IChoiceScene
    {
        string Hero { get; }
        List<Choice> Choices { get; }
        
    }
}