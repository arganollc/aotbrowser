using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using PygmentSharp.Core.Tokens;

namespace PygmentSharp.Core.Lexing
{
    /// <summary>
    /// A lexer for javascript
    /// </summary>
    [Lexer("Javascript", AlternateNames = "javascript,ecmascript,js,json")]
    [LexerFileExtension("*.js")]
    [LexerFileExtension("*.json")]
    public class JavascriptLexer : RegexLexer
    {
        /// <summary>
        /// Gets the state transition rules for the lexer. Each time a regex is matched,
        /// the internal state machine can be bumped to a new state which determines what
        /// regexes become valid again
        /// </summary>
        /// <returns></returns>
        protected override IDictionary<string, StateRule[]> GetStateRules()
        {
            var rules = new Dictionary<string, StateRule[]>();

            string JS_IDENT_START = "(?:[$_" + RegexUtil.Combine("Lu", "Ll", "Lt", "Lm", "Lo", "Nl") + "]|\\\\u[a-fA-F0-9]{4})";
            string JS_IDENT_PART = "(?:[$" + RegexUtil.Combine("Lu", "Ll", "Lt", "Lm", "Lo", "Nl", "Mn", "Mc", "Nd", "Pc") + "\u200c\u200d]|\\\\u[a-fA-F0-9]{4})";
            string JS_IDENT = JS_IDENT_START + "(?:" + JS_IDENT_PART + ")*";

            var builder = new StateRuleBuilder();
            builder.DefaultRegexOptions = RegexOptions.Multiline;

            rules["commentsandwhitespace"] = builder.NewRuleSet()
                .Add(@"\s+", TokenTypes.Text)
                .Add(@"<!--", TokenTypes.Comment)
                .Add(@"//.*?\n", TokenTypes.Comment.Single)
                .Add(@"/\*.*?\*/", TokenTypes.Comment.Multiline)
                .Build();

            rules["slashstartsregex"] = builder.NewRuleSet()
                .Include(rules["commentsandwhitespace"])
                .Add(@"/(\\.|[^[/\\\n]|\[(\\.|[^\]\\\n])*])+/" + @"([gim]+\b|\B)", TokenTypes.String.Regex, "#pop")
                .Add(@"(?=/)", TokenTypes.Text, "#pop", "badregex")
                .Default("#pop")
                .Build();

            rules["badregex"] = builder.NewRuleSet()
                .Add(@"\n", TokenTypes.Text, "#pop")
                .Build();

            rules["root"] = builder.NewRuleSet()
                .Add(@"\A#! ?/.*?\n", TokenTypes.Comment.Hashbang)
                .Add(@"^(?=\s|/|<!--)", TokenTypes.Text, "slashstartsregex")
                .Include(rules["commentsandwhitespace"])
                .Add(@"\+\+|--|~|&&|\?|:|\|\||\\(?=\n)|(<<|>>>?|=>|==?|!=?|[-<>+*%&|^/])=?", TokenTypes.Operator, "slashstartsregex")
                .Add(@"\.\.\.", TokenTypes.Punctuation)
                .Add(@"[{(\[;,]", TokenTypes.Punctuation, "slashstartsregex")
                .Add(@"[})\].]", TokenTypes.Punctuation)
                .Add(@"(for|in|while|do|break|return|continue|switch|case|default|if|else|throw|try|catch|finally|new|delete|typeof|instanceof|void|yield|this|of)\b", TokenTypes.Keyword, "slashstartsregex")
                .Add(@"(var|let|with|function)\b", TokenTypes.Keyword.Declaration, "slashstartsregex")
                .Add(@"(abstract|boolean|byte|char|class|const|debugger|double|enum|export|extends|final|float|goto|implements|import|int|interface|long|native|package|private|protected|public|short|static|super|synchronized|throws|transient|volatile)\b", TokenTypes.Keyword.Reserved)
                .Add(@"(true|false|null|NaN|Infinity|undefined)\b", TokenTypes.Keyword.Constant)
                .Add(@"(Array|Boolean|Date|Error|Function|Math|netscape|Number|Object|Packages|RegExp|String|Promise|Proxy|sun|decodeURI|decodeURIComponent|encodeURI|encodeURIComponent|Error|eval|isFinite|isNaN|isSafeInteger|parseFloat|parseInt|document|this|window)\b", TokenTypes.Name.Builtin)
                .Add(JS_IDENT, TokenTypes.Name.Other)
                .Add(@"[0-9][0-9]*\.[0-9]+([eE][0-9]+)?[fd]?", TokenTypes.Number.Float)
                .Add(@"0b[01]+", TokenTypes.Number.Bin)
                .Add(@"0o[0-7]+", TokenTypes.Number.Oct)
                .Add(@"0x[0-9a-fA-F]+", TokenTypes.Number.Hex)
                .Add(@"[0-9]+'", TokenTypes.Number.Integer)
                .Add(@"""(\\\\|\\""|[^""])*""", TokenTypes.String.Double)
                .Add(@"'(\\\\|\\'|[^'])*'", TokenTypes.String.Single)
                .Add(@"`", TokenTypes.String.Backtick, "interp")
                .Build();

            rules["interp"] = builder.NewRuleSet()
                .Add(@"`", TokenTypes.String.Backtick, "#pop")
                .Add(@"\\\\", TokenTypes.String.Backtick)
                .Add(@"\\`", TokenTypes.String.Backtick)
                .Add(@"\${", TokenTypes.String.Interpol, "interp-inside")
                .Add(@"\$", TokenTypes.String.Backtick)
                .Add(@"[^`\\$]+'", TokenTypes.String.Backtick)
                .Build();

            rules["interp-inside"] = builder.NewRuleSet()
                .Add(@"}", TokenTypes.String.Interpol, "#pop")
                .Include(rules["root"])
                .Build();

            return rules;
        }
    }
}