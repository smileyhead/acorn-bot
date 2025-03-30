namespace Acorn.Records
{
    public record Outlook
    {
        public string Text;
        public bool IsNeutral;

        public Outlook(string text, bool isNeutral)
        {
            Text = text;
            IsNeutral = isNeutral;
        }
    }
}
