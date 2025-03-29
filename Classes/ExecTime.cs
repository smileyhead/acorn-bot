using System.Diagnostics;
using System.Globalization;

namespace Acorn.Classes
{
    public class ExecTime
    {
        private Stopwatch Timer;
        private string ActionShortForm;
        private string ActionLongForm;

        public ExecTime(string actionShortForm, string actionLongForm)
        {
            Timer = new Stopwatch();
            ActionShortForm = actionShortForm;
            ActionLongForm = actionLongForm;
            Timer.Start();
            Console.WriteLine($"{DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("hu-HU"))}: {ActionLongForm}.");
        }

        public void Stop()
        {
            Timer.Stop();
            Console.WriteLine($"  {ActionShortForm} finished in {Timer.ElapsedMilliseconds}ms.");
            if (Timer.ElapsedMilliseconds > 3000) { Program.PrintDebugMessage($"{ActionShortForm} finished in {Timer.ElapsedMilliseconds} ms."); }
        }
    }
}
