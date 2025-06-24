namespace Acorn.Records
{
    internal record SearchResult
    {
        public Quote Quote { get; set; }
        public bool IsAltText { get; set; }

        public SearchResult(Quote quote, bool isAltText)
        {
            Quote = quote;
            IsAltText = isAltText;
        }
    }
}
