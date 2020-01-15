using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using PygmentSharp.Core.Tokens;

namespace PygmentSharp.Core.Lexing
{
    /// <summary>
    /// A lexer for HTML
    /// </summary>
    [Lexer("HTML", AlternateNames = "html")]
    [LexerFileExtension("*.html")]
    [LexerFileExtension("*.htm")]
    [LexerFileExtension("*.xhtml")]
    public class HtmlLexer : RegexLexer
    {
        /// <summary>
        /// Gets the state transition rules for the lexer. Each time a regex is matched,
        /// the internal state machine can be bumped to a new state which determines what
        /// regexes become valid again
        /// </summary>
        /// <returns></returns>
        protected override IDictionary<string, StateRule[]> GetStateRules()
        {
            var builder = new StateRuleBuilder();
            builder.DefaultRegexOptions = RegexOptions.IgnoreCase;

            var rules = new Dictionary<string, StateRule[]>();

            rules["root"] = builder.NewRuleSet()
                .Add(@"[^<&]+", TokenTypes.Text)
                .Add(@"&\S*?;", TokenTypes.Name.Entity)
                .Add(@"\<\!\[CDATA\[.*?\]\]\>", TokenTypes.Comment.Preproc)
                .Add(@"<!--", TokenTypes.Comment, "comment")
                .Add(@"<\?.*?\?>", TokenTypes.Comment.Preproc)
                .Add(@"<![^>]*>", TokenTypes.Comment.Preproc)
                .ByGroups(@"(<)(\s*)(script)(\s*)", new[] { "script-content", "tag" },
                    new TokenGroupProcessor(TokenTypes.Punctuation),
                    new TokenGroupProcessor(TokenTypes.Text),
                    new TokenGroupProcessor(TokenTypes.Name.Tag),
                    new TokenGroupProcessor(TokenTypes.Text))
                .ByGroups(@"(<)(\s*)(style)(\s*)", new[] { "style-content", "tag" },
                    new TokenGroupProcessor(TokenTypes.Punctuation),
                    new TokenGroupProcessor(TokenTypes.Text),
                    new TokenGroupProcessor(TokenTypes.Name.Tag),
                    new TokenGroupProcessor(TokenTypes.Text))
                .ByGroups(@"(<)(\s*)([\w:.-]+)", "tag",
                    new TokenGroupProcessor(TokenTypes.Punctuation),
                    new TokenGroupProcessor(TokenTypes.Text),
                    new TokenGroupProcessor(TokenTypes.Name.Tag))
                .ByGroups(@"(<)(\s*)(/)(\s*)([\w:.-]+)(\s*)(>)",
                    new TokenGroupProcessor(TokenTypes.Punctuation),
                    new TokenGroupProcessor(TokenTypes.Text),
                    new TokenGroupProcessor(TokenTypes.Punctuation),
                    new TokenGroupProcessor(TokenTypes.Text),
                    new TokenGroupProcessor(TokenTypes.Name.Tag),
                    new TokenGroupProcessor(TokenTypes.Text),
                    new TokenGroupProcessor(TokenTypes.Punctuation))
                .Build();

            rules["comment"] = builder.NewRuleSet()
                .Add(@"[^-]+", TokenTypes.Comment, "#pop")
                .Add(@"-->", TokenTypes.Comment)
                .Add(@"-", TokenTypes.Comment)
                .Build();

            rules["tag"] = builder.NewRuleSet()
                .Add(@"\s+", TokenTypes.Text)
                .ByGroups(@"([\w:-]+\s*)(=)(\s*)", "attr",
                    new TokenGroupProcessor(TokenTypes.Name.Attribute),
                    new TokenGroupProcessor(TokenTypes.Operator),
                    new TokenGroupProcessor(TokenTypes.Text))
                .Add(@"[\w:-]+", TokenTypes.Name.Attribute)
                .ByGroups(@"(/?)(\s*)(>)", "#pop",
                    new TokenGroupProcessor(TokenTypes.Punctuation),
                    new TokenGroupProcessor(TokenTypes.Text),
                    new TokenGroupProcessor(TokenTypes.Punctuation))
                .Build();

            rules["script-content"] = builder.NewRuleSet()
                .ByGroups(@"(<)(\s*)(/)(\s*)(script)(\s*)(>)", "#pop",
                    new TokenGroupProcessor(TokenTypes.Punctuation),
                    new TokenGroupProcessor(TokenTypes.Text),
                    new TokenGroupProcessor(TokenTypes.Punctuation),
                    new TokenGroupProcessor(TokenTypes.Text),
                    new TokenGroupProcessor(TokenTypes.Name.Tag),
                    new TokenGroupProcessor(TokenTypes.Text),
                    new TokenGroupProcessor(TokenTypes.Punctuation))
                .Using<JavascriptLexer>(@".+?(?=<\s*/\s*script\s*>)")
                .Build();


            rules["style-content"] = builder.NewRuleSet()
                .ByGroups(@"(<)(\s*)(/)(\s*)(style)(\s*)(>)", "#pop",
                    new TokenGroupProcessor(TokenTypes.Punctuation),
                    new TokenGroupProcessor(TokenTypes.Text),
                    new TokenGroupProcessor(TokenTypes.Punctuation),
                    new TokenGroupProcessor(TokenTypes.Text),
                    new TokenGroupProcessor(TokenTypes.Name.Tag),
                    new TokenGroupProcessor(TokenTypes.Text),
                    new TokenGroupProcessor(TokenTypes.Punctuation))
                .Using<CssLexer>(@".+?(?=<\s*/\s*style\s*>)")
                .Build();

            rules["attr"] = builder.NewRuleSet()
                .Add(@""".*?""", TokenTypes.String, "#pop")
                .Add(@"'.*?'", TokenTypes.String, "#pop")
                .Add(@"[^\s>]+", TokenTypes.String, "#pop")
                .Build();

            return rules;
        }
    }
}
