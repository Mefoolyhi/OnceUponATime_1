using System.Collections.Generic;

namespace OnceUponATime_1
{
    public interface ChoiceScene
    {
        string Background { get; }
        string Hero { get; }
        List<Choice> Choices { get; }
        
    }
}