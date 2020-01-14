using System;
using System.Collections.Generic;
using System.Linq;

using PygmentSharp.Core.Tokens;

namespace PygmentSharp.Core.Lexing
{
    internal static class CSharpLexerLevel
    {
        public static readonly string None = @"@?[_a-zA-Z]\w*";

        public static readonly string Basic = ("@?[_" + RegexUtil.Combine("Lu", "Ll", "Lt", "Lm", "Nl") + "]" +
                                        "[" + RegexUtil.Combine("Lu", "Ll", "Lt", "Lm", "Nl", "Nd", "Pc",
                                            "Cf", "Mn", "Mc") + "]*");

        public static readonly string Full = ("@?(?:_|[^" +
                                       RegexUtil.AllExcept("Lu", "Ll", "Lt", "Lm", "Lo", "Nl") + "])"
                                       + "[^" + RegexUtil.AllExcept("Lu", "Ll", "Lt", "Lm", "Lo", "Nl",
                                           "Nd", "Pc", "Cf", "Mn", "Mc") + "]*");
    }



    /// <summary>
    /// A lexer for C#
    /// </summary>
    [Lexer("C#", AlternateNames = "csharp,c#,c-sharp,c sharp,c #")]
    [LexerFileExtension("*.cs")]
    public class CSharpLexer : RegexLexer
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
            var cs_ident = CSharpLexerLevel.Full;
            var builder = new StateRuleBuilder();

            rules["root"] = builder.NewRuleSet()
                .ByGroups(@"^([ \t]*(?:" + cs_ident + @"(?:\[\])?\s+)+?)" +  // return type
                                 @"(" + cs_ident +   @")" +                  // method name
                                 @"(\s*)(\()",                               // signature start
                    new LexerGroupProcessor(this),
                    new TokenGroupProcessor(TokenTypes.Name.Function),
                    new TokenGroupProcessor(TokenTypes.Text),
                    new TokenGroupProcessor(TokenTypes.Punctuation))

                .Add(@"^\s*\[.*?\]", TokenTypes.Name.Attribute)
                .Add(@"[^\S\n]+", TokenTypes.Text)
                .Add(@"\\\n", TokenTypes.Text) //line continuation
                .Add(@"//.*?\n", TokenTypes.Comment.Single)
                .Add(@"/[*].*?[*]/", TokenTypes.Comment.Multiline)
                .Add(@"\n", TokenTypes.Text)
                .Add(@"[~!%^&*()+=|\[\]:;,.<>/?-]", TokenTypes.Punctuation)
                .Add(@"[{}]", TokenTypes.Punctuation)
                .Add(@"@""(""""|[^""])*""", TokenTypes.String)
                .Add(@"""(\\\\|\\""|[^""\n])*[""\n]", TokenTypes.String)
                .Add(@"'\\.'|'[^\\]'", TokenTypes.String.Char)
                .Add(@"[0-9](\.[0-9]*)?([eE][+-][0-9]+)?" +
                                 @"[flFLdD]?|0[xX][0-9a-fA-F]+[Ll]?", TokenTypes.Number)
                .Add(@"#[ \t]*(if|endif|else|elif|define|undef|" +
                                 @"line|error|warning|region|endregion|pragma)\b.*?\n", TokenTypes.Comment.Preproc)
                .ByGroups(@"'\b(extern)(\s+)(alias)\b",
                    new TokenGroupProcessor(TokenTypes.Keyword),
                    new TokenGroupProcessor(TokenTypes.Text),
                    new TokenGroupProcessor(TokenTypes.Keyword))
                .Add(@"(abstract|as|async|await|base|break|case|catch|" +
                                @"checked|const|continue|default|delegate|" +
                                @"do|else|enum|event|explicit|extern|false|finally|" +
                                @"fixed|for|foreach|goto|if|implicit|in|interface|" +
                                @"internal|is|lock|new|null|operator|" +
                                @"out|override|params|private|protected|public|readonly|" +
                                @"ref|return|sealed|sizeof|stackalloc|static|" +
                                @"switch|this|throw|true|try|typeof|" +
                                @"unchecked|unsafe|virtual|void|while|" +
                                @"get|set|new|partial|yield|add|remove|value|alias|ascending|" +
                                @"descending|from|group|into|orderby|select|where|" +
                                @"join|equals)\b", TokenTypes.Keyword)
                .ByGroups(@"(global)(::)",
                    new TokenGroupProcessor(TokenTypes.Keyword),
                    new TokenGroupProcessor(TokenTypes.Punctuation))
                .Add(@"(bool|byte|char|decimal|double|dynamic|float|int|long|object|" +
                                 @"sbyte|short|string|uint|ulong|ushort|var)\b\??", TokenTypes.Keyword.Type)
                .ByGroups(@"(class|struct)(\s+)", "class",
                    new TokenGroupProcessor(TokenTypes.Keyword),
                    new TokenGroupProcessor(TokenTypes.Text))
                .ByGroups(@"(namespace|using)(\s+)", "namespace",
                    new TokenGroupProcessor(TokenTypes.Keyword),
                    new TokenGroupProcessor(TokenTypes.Text))
                .Add(cs_ident, TokenTypes.Name)
                .Build();

            rules["class"] = builder.NewRuleSet()
                .Add(cs_ident, TokenTypes.Name.Class, "#pop")
                .Default("#pop")
                .Build();

            rules["namespace"] = builder.NewRuleSet()
                .Add(@"(?=\()", TokenTypes.Text, "#pop") // using resource
                .Add(@"(" + cs_ident + @"|\.)+", TokenTypes.Name.Namespace, "#pop")
                .Build();

            return rules;
        }
    }
}
