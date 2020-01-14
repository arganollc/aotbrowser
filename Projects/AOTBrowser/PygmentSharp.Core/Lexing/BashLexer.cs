using System;
using System.Collections.Generic;
using System.Linq;

using PygmentSharp.Core.Tokens;

namespace PygmentSharp.Core.Lexing
{
    /// <summary>
    /// Lexer for shell scripts
    /// </summary>
    [Lexer("Bash", AlternateNames = "bash,sh,ksh,shell")]
    [LexerFileExtension("*.sh")]
    [LexerFileExtension("*.ksh")]
    [LexerFileExtension("*.bash")]
    [LexerFileExtension("*.ebuild")]
    [LexerFileExtension("*.eclass")]
    [LexerFileExtension("*.exheres-0")]
    [LexerFileExtension("*.exlib")]
    [LexerFileExtension(".bashrc")]
    [LexerFileExtension("bashrc")]
    [LexerFileExtension(".bash_*")]
    [LexerFileExtension("bash_*")]
    [LexerFileExtension(".profile")]
    [LexerFileExtension("PKGBUILD")]
    public class BashLexer : RegexLexer
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

            rules["basic"] = builder.NewRuleSet()
                .ByGroups(@"\b(if|fi|else|while|do|done|for|then|return|function|case|elect|continue|until|esac|elif)(\s*)\b",
                    TokenTypes.Keyword, TokenTypes.Text)
                .Add(@"\b(alias|bg|bind|break|builtin|caller|cd|command|compgen|complete|declare|dirs|disown|echo|enable|eval|exec|exit|export|false|fc|fg|getopts|hash|help|history|jobs|kill|let|local|logout|popd|printf|pushd|pwd|read|readonly|set|shift|shopt|source|suspend|test|time|times|trap|true|type|typeset|ulimit|umask|unalias|unset|wait)(?=[\s)`])",
                    TokenTypes.Name.Builtin)
                .Add(@"\A#!.+\n", TokenTypes.Comment.Hashbang)
                .Add(@"#.*\n", TokenTypes.Comment.Single)
                .Add(@"\\[\w\W]", TokenTypes.String.Escape)
                .ByGroups(@"(\b\w+)(\s*)(=)", TokenTypes.Name.Variable, TokenTypes.Text, TokenTypes.Operator)
                .Add(@"[\[\]{}()=]", TokenTypes.Operator)
                .Add(@"<<<", TokenTypes.Operator)
                .Add(@"<<-?\s*(\'?)\\?(\w+)[\w\W]+?\2", TokenTypes.String)
                .Add(@"&&|\|\|", TokenTypes.Operator)
                .Build();

            rules["data"] = builder.NewRuleSet()
                .Add(@"(?s)\$?""(\\\\|\\[0-7]+|\\.|[^""\\$])*""", TokenTypes.String.Double)
                .Add(@"""", TokenTypes.String.Double, "string")
                .Add(@"(?s)\$'(\\\\|\\[0-7]+|\\.|[^'\\])*'", TokenTypes.String.Single)
                .Add(@"(?s)'.*?'", TokenTypes.String.Single)
                .Add(@";", TokenTypes.Punctuation)
                .Add(@"&", TokenTypes.Punctuation)
                .Add(@"\|", TokenTypes.Punctuation)
                .Add(@"\s+", TokenTypes.Text)
                .Add(@"\d+(?= |\Z)", TokenTypes.Number)
                .Add(@"[^=\s\[\]{}()$""\'`\\<&|;]+", TokenTypes.Text)
                .Add(@"<", TokenTypes.Text)
                .Build();

            rules["interp"] = builder.NewRuleSet()
                .Add(@"\$\(\(", TokenTypes.Keyword, "math")
                .Add(@"\$\(", TokenTypes.Keyword, "paren")
                .Add(@"\$\{#?", TokenTypes.String.Interpol, "curly")
                .Add(@"\$[a-zA-Z_][a-zA-Z0-9_]*", TokenTypes.Name.Variable)
                .Add(@"\$(?:\d+|[#$?!_*@-])", TokenTypes.Name.Variable)
                .Add(@"\$", TokenTypes.Text)
                .Build();

            rules["root"] = builder.NewRuleSet()
                .Include(rules["basic"])
                .Add(@"`", TokenTypes.String.Backtick, "backticks")
                .Include(rules["data"])
                .Include(rules["interp"])
                .Build();

            rules["string"] = builder.NewRuleSet()
                .Add(@"""", TokenTypes.String.Double, "#pop")
                .Add(@"(?s)(\\\\|\\[0-7]+|\\.|[^""\\$])+", TokenTypes.String.Double)
                .Include(rules["interp"])
                .Build();

            rules["curly"] = builder.NewRuleSet()
                .Add(@"\}", TokenTypes.String.Interpol, "#pop")
                .Add(@":-", TokenTypes.Keyword)
                .Add(@"\w+", TokenTypes.Name.Variable)
                .Add(@"[^}:""\'`$\\]+", TokenTypes.Punctuation)
                .Add(@":", TokenTypes.Punctuation)
                .Include(rules["root"])
                .Build();

            rules["paren"] = builder.NewRuleSet()
                .Add(@"\)", TokenTypes.Keyword, "#pop")
                .Include(rules["root"])
                .Build();

            rules["math"] = builder.NewRuleSet()
                .Add(@"\)\)", TokenTypes.Keyword, "#pop")
                .Add(@"[-+*/%^|&]|\*\*|\|\|", TokenTypes.Operator)
                .Add(@"\d+#\d+", TokenTypes.Number)
                .Add(@"\d+#(?! )", TokenTypes.Number)
                .Add(@"\d+", TokenTypes.Number)
                .Include(rules["root"])
                .Build();

            rules["backticks"] = builder.NewRuleSet()
                .Add(@"`", TokenTypes.String.Backtick, "#pop")
                .Include(rules["root"])
                .Build();

            return rules;
        }
    }
}
