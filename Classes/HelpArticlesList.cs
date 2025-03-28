using Acorn.Records;
using System.Text.Json;

namespace Acorn.Classes
{
    internal class HelpArticlesList
    {
        private List<HelpArticle> Articles;

        public HelpArticlesList(string helpArticlesPath)
        {
            Articles = new List<HelpArticle>();

            using (FileStream readHelpArticles = File.OpenRead(helpArticlesPath))
            {
                Articles =
                    JsonSerializer.Deserialize<List<HelpArticle>>(readHelpArticles);
            }
        }

        public string GetHelp(string command)
        {
            string answer = "";

            for (int i = 0; i < Articles.Count; i++)
            {
                if (command == Articles[i].Command) { answer = Articles[i].HelpText; break; }

                if (i == Articles.Count - 1) { answer = "Command not found."; break; }
            }

            return answer;
        }
    }
}
