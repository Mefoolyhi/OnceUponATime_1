namespace OnceUponATime_1
{
    public class Phrase
    {
        public string Person { get; }
        public string Text { get; }

        public Phrase(string person, string text)
        {
            Text = text;
            Person = person;
        }
    }
}