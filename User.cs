namespace Acorn
{
    internal record User
    {
        public ulong Id;
        public string? Name;

        public User(ulong id)
        {
            Id = id;
            Name = null;
        }

        public User(ulong id, string? name)
        {
            Id = id;
            Name = name;
        }
    }
}
