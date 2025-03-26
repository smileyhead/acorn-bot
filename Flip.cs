namespace Acorn
{
    internal class Flip
    {
        public string DoFlip()
        {
            Random random = new Random();
            string answer = "You flip a coin…\n\nIt's ";
            if (random.NextDouble() >= 0.5) { answer += "heads!"; }
            else answer += "tails!";

            return answer;
        }
    }
}
