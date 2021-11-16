using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Specialized;
using System.IO;

namespace Arbela.Dynamics.AX.Xpp.Support
{
    public class HtmlFormatHelper
    {
        public static string Format(string _unformattedString)
        {
            PygmentSharp.Core.Formatting.HtmlFormatterOptions htmlOptions = new PygmentSharp.Core.Formatting.HtmlFormatterOptions();
            htmlOptions.Style = new PygmentSharp.Core.Styles.DefaultStyle();
            htmlOptions.ClassPrefix = "ARBAOT";
            htmlOptions.NoClasses = true;
            htmlOptions.LineNumbers = PygmentSharp.Core.Formatting.LineNumberStyle.Table;
            return PygmentSharp.Core.Pygmentize.Content(_unformattedString)
                .WithLexer(new XPlusPlusLexer())
                .WithFormatter(new PygmentSharp.Core.Formatting.HtmlFormatter(htmlOptions))
                .AsString();

        }
    }
}
