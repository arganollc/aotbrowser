using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using PygmentSharp.Core.Extensions;
using PygmentSharp.Core.Styles;
using PygmentSharp.Core.Tokens;

namespace PygmentSharp.Core.Formatting
{
    internal class ClassToStyle
    {
        public string StyleRules { get; set; }
        public TokenType TokenType { get; set; }
        public int Depth => TokenType.Depth;
    }

    internal struct WrapResult
    {
        public bool IsSourceLine { get; }
        public string FormattedLine { get; }

        public WrapResult(bool isSourceLine, string formattedLine)
        {
            IsSourceLine = isSourceLine;
            FormattedLine = formattedLine;
        }
    }

    /// <summary>
    /// An output formatter that writes a token stream as HTML
    /// </summary>
    [Formatter("Html", AlternateNames = "web")]
    [FormatterFileExtension("*.html")]
    [FormatterFileExtension("*.htm")]
    public class HtmlFormatter : Formatter
    {
        private const string CSSFILE_TEMPLATE =
@"td.linenos {{ background-color: #f0f0f0; padding-right: 10px; }}
span.lineno {{ background-color: #f0f0f0; padding: 0 5px 0 5px; }}
pre {{ line-height: 125%; }}
{1}";

        private const string DOC_HEADER =
@"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">
<html>
<head>
  <title>{0}</title>
  <meta http-equiv=""content-type"" content=""text/html; charset=""utf-8"">
  <style type=""text/css"">
" + CSSFILE_TEMPLATE + @"
  </style>
</head>
<body>
<h2>{0}</h2>
";

        private const string DOC_FOOTER =
@"
</body>
</html>
";
        /// <summary>
        /// Gets the options for the formatter
        /// </summary>
        public HtmlFormatterOptions Options { get; }

        private Dictionary<string, ClassToStyle> _cssToStyleMap;
        private TokenTypeMap _tokenToClassMap;
        private readonly Style _style;


        /// <summary>
        /// Initializes a new isntance of the <see cref="HtmlFormatter"/> class
        /// </summary>
        public HtmlFormatter() : this(new HtmlFormatterOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlFormatter"/> class with given <paramref name="options"/>
        /// </summary>
        /// <param name="options">Configuration details for the <see cref="HtmlFormatter"/></param>
        public HtmlFormatter(HtmlFormatterOptions options)
        {
            Options = options;
            _style = Options.Style ?? new DefaultStyle();
            CreateStylesheet();
        }

        private string GetTokenTypeClass(TokenType ttype)
        {
            var foundName = TokenTypeMap.Instance[ttype];
            if (foundName != null)
                return foundName;

            var autoName = new StringBuilder();
            while (foundName == null)
            {
                Debug.Assert(ttype != null, "ttype != null");
                autoName.Insert(0, "-" + ttype.Parent?.Name);
                ttype = ttype.Parent;
                foundName = TokenTypeMap.Instance[ttype];
            }

            return foundName + autoName;
        }

        private string GetCssClass(TokenType ttype)
        {
            var ttypeclass = GetTokenTypeClass(ttype);
            if (ttypeclass != null)
                return Options.ClassPrefix + ttypeclass;
            return "";
        }

        private string GetCssClasses(TokenType ttype)
        {
            var cls = new StringBuilder();
            cls.Append(GetCssClass(ttype));

            while (!TokenTypeMap.Instance.Contains(ttype))
            {
                ttype = ttype.Parent;
                cls.Insert(0, GetCssClass(ttype) + " ");
            }

            return cls.ToString();
        }

        private void CreateStylesheet()
        {
            //variable names kept for parity with python
            var t2c = new Dictionary<TokenType, string>()
            {
                {TokenTypes.Token, ""}
            };
            var c2s = new Dictionary<string, ClassToStyle>();

            var style = new StringBuilder();

            foreach (var kvp in _style)
            {
                var ttype = kvp.Key;
                var ndef = kvp.Value;

                var name = GetCssClass(ttype);
                style.Clear();

                if (!string.IsNullOrEmpty(ndef.Color))
                    style.AppendFormat("color: #{0}; ", ndef.Color);
                if (ndef.Bold)
                    style.Append("font-weight: bold; ");
                if (ndef.Italic)
                    style.Append("font-style: italic; ");
                if (ndef.Underline)
                    style.Append("text-decoration: underline; ");
                if (!string.IsNullOrEmpty(ndef.BackgroundColor))
                    style.AppendFormat("background-color: #{0}; ", ndef.BackgroundColor);
                if (!string.IsNullOrEmpty(ndef.BorderColor))
                    style.AppendFormat("border-color: #{0}; ", ndef.BorderColor);
                if (style.Length > 0)
                {
                    style.Length = style.Length - 2; //delete trailing "; "
                    t2c[ttype] = name;
                    c2s[name] = new ClassToStyle {StyleRules = style.ToString(), TokenType = ttype};
                }
            }

            _tokenToClassMap = new TokenTypeMap(t2c);
            _cssToStyleMap = c2s;

        }

        internal string GetStyleDefaults(string arg = null)
        {
            string prefix = arg;
            if (arg == null)
            {
                prefix = string.IsNullOrEmpty(Options.CssClass) ? "" : ("." + Options.CssClass);
            }

            return GetStyleDefaults(new [] { prefix }, arg != null);
        }

        internal string GetStyleDefaults(string[] prefixes)
        {
            return GetStyleDefaults(prefixes, true);
        }

        private string GetStyleDefaults(string[] prefixes, bool usesPrefixes)
        {
            Func<string, string> prefix = cls =>
            {
                if (!string.IsNullOrEmpty(cls))
                    cls = "." + cls;

                return string.Join(", ", prefixes.Select(arg => (!string.IsNullOrEmpty(arg) ? arg + " " : "") + cls));
            };

            var lines = _cssToStyleMap
                .Where(kvp => kvp.Key != null && kvp.Value != null)
                .Select(kvp => Tuple.Create(kvp.Value.Depth, kvp.Value.TokenType, kvp.Key, kvp.Value.StyleRules))
                .OrderBy(t => t.Item1)
                .Select(s => $"{prefix(s.Item3)} {{ {s.Item4} }} /* {s.Item2} */")
                .ToList();

            if (usesPrefixes && !Options.NoBackground && _style.BackgroundColor != null)
            {
                var textStyle = "";
                if (_tokenToClassMap.Contains(TokenTypes.Text))
                {
                    textStyle = " " + _cssToStyleMap[_tokenToClassMap[TokenTypes.Text]].StyleRules;
                }

                lines.Insert(0, $"{prefix("")} {{ background: {_style.BackgroundColor}; {textStyle} }}");
            }

            if (_style.HighlightColor != null)
            {
                lines.Insert(0, $"{prefix("")}.hll {{ background-color: {_style.HighlightColor} }}");
            }

            return string.Join("\n", lines);
        }


        private IEnumerable<WrapResult> WrapFull(IEnumerable<WrapResult> inner)
        {
            /* stuff for external css file elided for now */

            yield return new WrapResult(false, string.Format(DOC_HEADER, Options.Title, GetStyleDefaults("body")));

            foreach (var line in inner)
                yield return line;

            yield return new WrapResult(false, DOC_FOOTER);
        }

        private IEnumerable<WrapResult> WrapTableLineNos(IEnumerable<WrapResult> inner)
        {
            var innerHtml = new StringBuilder(1024);
            int lncount = 0;
            foreach (var i in inner)
            {
                innerHtml.Append(i.FormattedLine);
                if (i.IsSourceLine)
                    lncount++;
            }

            var fl = Options.LineNumberStart;
            var mw = (lncount + fl - 1).ToString().Length;
            var sp = Options.LineNumberSpecial;
            var st = Options.LineNumberStep;
            var la = Options.LineAnchors;
            var aln = Options.AnchorLineNumbers;
            var nocls = Options.NoClasses;
            string ls;

            if (sp > 0)
            {
                var lines = new List<string>();
                for (int i = fl; i < fl + lncount; i++)
                {
                    if (i%st == 0)
                    {
                        if (i%sp == 0)
                        {
                            if (aln)
                                lines.Add($"<a href=\"#{la}-{i}\" class=\"special\">{i.ToString().PadLeft(mw, ' ')}</a>");
                            else
                                lines.Add($"<span class=\"special\">{i.ToString().PadLeft(mw, ' ')}</span>");
                        }
                        else
                        {
                            if (aln)
                                lines.Add($"<a href=\"#{la}-{i}\">{i.ToString().PadLeft(mw, ' ')}</a>");
                            else
                                lines.Add(i.ToString().PadLeft(mw, ' '));
                        }
                    }
                    else
                    {
                        lines.Add("");
                    }
                }

                ls = string.Join("\n", lines);
            }
            else
            {

                var lines = new List<string>();
                for (int i = fl; i < fl + lncount; i++)
                {
                    if (i%st == 0)
                    {
                        if (aln)
                            lines.Add($"<a href=\"#{la}-{i}\">{i.ToString().PadLeft(mw, ' ')}</a>");
                        else
                            lines.Add(i.ToString().PadLeft(mw, ' '));
                    }
                    else
                        lines.Add("");
                }

                ls = string.Join("\n", lines);
            }

            if (nocls)
            {
                yield return new WrapResult(false, $"<table class=\"{Options.CssClass}table\">" +
                                                   "<tr><td><div class=\"linenodiv\"" +
                                                   "style=\"background-color: #f0f0f0; padding-right: 10px\">" +
                                                   "<pre style=\"line-height: 125%\">" +
                                                   ls + "</pre></div></td><td class=\"code\">");
            }
            else
            {
                yield return new WrapResult(false, $"<table class=\"{Options.CssClass}table\">" +
                                               "<tr><td class=\"linenos\"><div class=\"linenodiv\"><pre>" +
                                               ls + "</pre></div></td><td class=\"code\">");
            }

            yield return new WrapResult(false, innerHtml.ToString());
            yield return new WrapResult(false, "</td></tr></table>");

        }

        private IEnumerable<WrapResult> WrapLineAnchors(IEnumerable<WrapResult> inner)
        {
            var s = Options.LineAnchors;
            var i = Options.LineNumberStart - 1;
            foreach (var item in inner)
            {
                if (item.IsSourceLine)
                {
                    i++;
                    yield return new WrapResult(true, $"<a name=\"{s}-{i}\"></a>{item.FormattedLine}");
                }
                else
                    yield return item;
            }
        }

        private IEnumerable<WrapResult> WrapDiv(IEnumerable<WrapResult> inner)
        {
            var style = new StringBuilder();
            if (Options.NoClasses && !Options.NoBackground && _style.BackgroundColor != null)
                style.AppendFormat("background: {0}", _style.BackgroundColor).Append("; ");

            if (Options.CssStyles != null)
                style.Append(Options.CssStyles).Append("; ");

            yield return new WrapResult(false, "<div " +
                                               (Options.CssClass == null ? "" : $"class=\"{Options.CssClass}\" ") +
                                               (style.Length == 0 ? "" : $"style=\"{style}\"") +
                                               ">");
            foreach (var i in inner)
                yield return i;
            yield return new WrapResult(false, "</div>\n");

        }

        private IEnumerable<WrapResult> WrapLineSpans(IEnumerable<WrapResult> inner)
        {
            var s = Options.LineAnchors;
            var i = Options.LineNumberStart - 1;
            foreach (var item in inner)
            {
                if (item.IsSourceLine)
                {
                    i++;
                    yield return new WrapResult(true, $"<span id=\"{s}-{i}>{item.FormattedLine}</span>");
                }
                else
                    yield return item;
            }
        }

        private IEnumerable<WrapResult> WrapPre(IEnumerable<WrapResult> inner)
        {
            var styles = new StringBuilder();
            if (Options.PreStyles != null)
                styles.Append(Options.PreStyles).Append("; ");
            if (Options.NoClasses)
                styles.Append("line-height: 125%").Append("; ");

            if (Options.Filename != null)
                yield return new WrapResult(false, $"<span class=\"filename\">{Options.Filename}</span>");

            yield return new WrapResult(false, "<pre " +
                (styles.Length > 0 ? $"style=\"{styles}\" " : "") +
                ">");

            foreach (var res in inner)
                yield return res;

            yield return new WrapResult(false, "</pre>");

        }

        private IEnumerable<WrapResult> FormatLines(IEnumerable<Token> tokenSource)
        {
            var nocls = Options.NoClasses;
            var lsep = Options.LineSeparator;
            Func<TokenType, string> getcls = ttype => _tokenToClassMap[ttype];
            var c2s = _cssToStyleMap;
            var lspan = "";
            var line = new StringBuilder();

            foreach (var token in tokenSource)
            {
                var ttype = token.Type;
                var value = token.Value;

                string cspan;
                if (nocls)
                {
                    var cclass = getcls(ttype);
                    while (cclass == null)
                    {
                        ttype = ttype.Parent;
                        cclass = getcls(ttype);
                    }
                    cspan = !string.IsNullOrEmpty(cclass) ? $"<span style=\"{c2s[cclass].StyleRules}\">" : "";                   
                }
                else
                {
                    var cls = GetCssClasses(ttype);
                    cspan = cls != null ? $"<span class=\"{cls}\">" : "";
                }

                var parts = EscapeHtml(value).Split('\n');

                foreach (var part in parts.Take(parts.Length - 1))
                {
                    if (line.Length > 0)
                    {
                        if (lspan != cspan)
                        {
                            line.Append(lspan.PythonAnd("</span>"));
                            line.Append(cspan);
                            line.Append(part);
                            line.Append(cspan.PythonAnd("</span>"));
                            line.Append(lsep);
                        }
                        else
                        {
                            line.Append(part)
                                .Append(lspan.PythonAnd("</span>"))
                                .Append(lsep);
                        }
                        yield return new WrapResult(true, line.ToString());
                        line.Clear();
                    }
                    else if (part != null)
                    {
                        yield return new WrapResult(true,
                            "" + cspan + part + (cspan.PythonAnd("</span>")) + lsep);
                    }
                    else
                    {
                        yield return new WrapResult(true, lsep);
                    }
                }

                // last line
                if (line.Length > 0 && parts.Last() != null)
                {
                    if (lspan != cspan)
                    {
                        line.Append(lspan.PythonAnd("</span>"))
                            .Append(cspan)
                            .Append(parts.Last());
                        lspan = cspan;
                    }
                    else
                    {
                        line.Append(parts.Last());
                    }
                }
                else if (parts.Last() != null)
                {
                    line.Clear();
                    line.Append(cspan).Append(parts.Last());
                    lspan = cspan;
                }
            }

            if (line.Length > 0)
            {
                line.Append(lspan.PythonAnd("</span>"))
                    .Append(lsep);
                yield return new WrapResult(true, line.ToString());
            }
        }

        private IEnumerable<WrapResult> HighlighLines(IEnumerable<WrapResult> inner)
        {
            var hls = Options.HighlightLines.ToArray();

            int i = 0;
            foreach (var inn in inner)
            {
                var t = inn.IsSourceLine;
                var value = inn.FormattedLine;

                if (!t)
                {
                    yield return inn;
                }

                if (hls.Contains(i + 1))
                {
                    if (Options.NoClasses)
                    {
                        if (_style.HighlightColor != null)
                        {
                            string style = $" style=\"background-color: {_style.HighlightColor}";
                            yield return new WrapResult(true, $"<span{style}>{value}</span>");
                        }
                        else
                            yield return new WrapResult(true, $"<span class=\"hll\">{value}</span>");
                    }
                }
                else
                    yield return inn;

                i++;
            }
        }

        private IEnumerable<WrapResult> Wrap(IEnumerable<WrapResult> source, TextWriter writer)
        {
            return WrapDiv(WrapPre(source));
        }

        /// <summary>
        /// When overriden in a child class, formats tokens into text stream
        /// </summary>
        /// <remarks>The name was borrowed from python, because it would further process the results to an encoding. That's not needed in C#</remarks>
        /// <param name="tokenSource">The input stream of Tokens</param>
        /// <param name="writer">The output stream to write text</param>
        protected override void FormatUnencoded(IEnumerable<Token> tokenSource, TextWriter writer)
        {
            var source = FormatLines(tokenSource);

            if (Options.HighlightLines.Any())
                source = HighlighLines(source);

            if (!Options.NoWrap)
            {
                if (Options.LineNumbers == LineNumberStyle.Inline)
                    source = WrapInlineLineNumbers(source);
                if (!string.IsNullOrEmpty(Options.LineAnchors))
                    source = WrapLineAnchors(source);
                if (!string.IsNullOrEmpty(Options.LineSpans))
                    source = WrapLineSpans(source);
                source = Wrap(source, writer);

                if (Options.LineNumbers == LineNumberStyle.Table)
                    source = WrapTableLineNos(source);
                if (Options.Full)
                    source = WrapFull(source);
            }

            foreach (var line in source)
                writer.Write(line.FormattedLine);
        }

        // ReSharper disable once UnusedParameter.Local
        private IEnumerable<WrapResult> WrapInlineLineNumbers(IEnumerable<WrapResult> source)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Escapes HTML to HTML Entities
        /// </summary>
        /// <param name="input">A snippet of html to escape</param>
        /// <returns>An escaped string of html</returns>
        internal static string EscapeHtml(string input)
        {
            return WebUtility.HtmlEncode(input);
        }

        /// <summary>
        /// Escapes HTML to a stream
        /// </summary>
        /// <param name="input">A snippet of html to escape</param>
        /// <param name="output">The stream to write the escaped text</param>
        internal static void EscapeHtml(string input, TextWriter output)
        {
            WebUtility.HtmlEncode(input, output);
        }
    }
}
