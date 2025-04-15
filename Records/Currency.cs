namespace Acorn.Records
{
    public record Currency
    {
        public string Code;
        public string Name;
        public double Value;

        public Currency(string code, string name, double value = 0.0)
        {
            Code = code;
            Name = name;
            Value = value;
        }

        public void SetRate(double rate) { Value = rate; }
    }
}
