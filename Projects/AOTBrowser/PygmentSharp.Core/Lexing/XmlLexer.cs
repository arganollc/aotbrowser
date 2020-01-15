using System;
using System.Collections.Generic;
using System.Linq;

using PygmentSharp.Core.Tokens;

namespace PygmentSharp.Core.Lexing
{
    /// <summary>
    /// A lexer for XML
    /// </summary>
    [Lexer("XML", AlternateNames = "xml")]
    [LexerFileExtension("*.xml")]
    [LexerFileExtension("*.xsl")]
    [LexerFileExtension("*.rss")]
    [LexerFileExtension("*.xslt")]
    [LexerFileExtension("*.xsd")]
    [LexerFileExtension("*.wsdl")]
    [LexerFileExtension("*.wsf")]
    public class XmlLexer : RegexLexer
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
            var builder = new StateRuleBuilder();

            rules["root"] = builder.NewRuleSet()
                .Add(@"[^<&]+", TokenTypes.Text)
                .Add(@"&\S*?;", TokenTypes.Name.Entity)
                .Add(@"\<\!\[CDATA\[.*?\]\]\>", TokenTypes.Comment.Preproc)
                .Add(@"<!--", TokenTypes.Comment, "comment")
                .Add(@"<\?.*?\?>", TokenTypes.Comment.Preproc)
                .Add(@"<![^>]*>", TokenTypes.Comment.Preproc)
                .Add(@"<\s*[\w:.-]+", TokenTypes.Name.Tag, "tag")
                .Add(@"<\s*/\s*[\w:.-]+\s*>'", TokenTypes.Name.Tag)
                .Build();

            rules["comment"] = builder.NewRuleSet()
                .Add(@"[^-]+", TokenTypes.Text)
                .Add(@"-->", TokenTypes.Comment, "#pop")
                .Add(@"-", TokenTypes.Comment)
                .Build();

            rules["tag"] = builder.NewRuleSet()
                .Add(@"\s+", TokenTypes.Text)
                .Add(@"[\w.:-]+\s*=", TokenTypes.Name.Attribute, "attr")
                .Add(@"/?\s*>", TokenTypes.Name.Tag, "#pop")
                .Build();

            rules["attr"] = builder.NewRuleSet()
                .Add(@"\s+", TokenTypes.Text)
                .Add(@""".*?""", TokenTypes.String, "#pop")
                .Add(@".*?'", TokenTypes.String, "#pop")
                .Add(@"[^\s>]+", TokenTypes.String, "#pop")
                .Build();

            return rules;
        }
    }
}
