namespace Acorn
{
    internal record Quote
    {
        public int Id;
        public string? Body;
        public ulong UserId;
        public string? Username;
        public string? Link;
    }
}
