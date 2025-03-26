using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Acorn_old;

namespace Acorn
{
    internal class HelpArticlesList
    {
        private List<HelpArticle> articles;

        public HelpArticlesList(string helpArticlesPath)
        {
            articles = new List<HelpArticle>();

            using (FileStream readHelpArticles = File.OpenRead(helpArticlesPath))
            {
                articles =
                    JsonSerializer.Deserialize<List<HelpArticle>>(readHelpArticles);
            }
        }

        public string GetHelp (string command)
        {
            string answer = "";

            for (int i = 0; i < articles.Count; i++)
            {
                if (command == articles[i].Command) { answer = articles[i].HelpText; break; }

                if (i == articles.Count - 1) { answer = "Command not found."; break; }
            }

            return answer;
        }
    }
}
