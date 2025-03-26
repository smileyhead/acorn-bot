using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acorn
{
    internal record HelpArticle
    {
        public string? Command { get; set; }
        public string? HelpText { get; set; }
    }
}
