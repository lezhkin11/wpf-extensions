namespace Demo.Classes
{
    public class SimpleItem
    {
        public SimpleItem(int number, string codeText)
        {
            Number = number;
            CodeText = codeText;
        }

        public int Number { get; }

        public string CodeText { get; }
    }
}