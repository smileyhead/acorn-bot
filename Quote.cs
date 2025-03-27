namespace Acorn
{
    internal record Quote
    {
        public int Id { get; set; }
        public string? Body { get; set; }
        public ulong UserId { get; set; }
        public string? Username { get; set; }
        public string? Link { get; set; }
    }
}
