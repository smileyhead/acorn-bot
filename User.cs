namespace Acorn
{
    internal record User
    {
        public ulong Id;
        public string? Name;
        public int? QuoteCount;

        public User()
        {
            Id = 0;
            Name = null;
            QuoteCount = 0;
        }
    }
}
