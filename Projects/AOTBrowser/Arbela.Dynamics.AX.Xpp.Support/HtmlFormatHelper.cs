using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Arbela.Dynamics.AX.Xpp.Support
{
    public class HtmlFormatHelper
    {
        public static string Format(string _unformattedString)
        {
            if (string.IsNullOrEmpty(_unformattedString))
            {
                return string.Empty;
            }

            try
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
            catch (Exception)
            {
                return "<pre>" + System.Net.WebUtility.HtmlEncode(_unformattedString) + "</pre>";
            }
        }
    }
}
