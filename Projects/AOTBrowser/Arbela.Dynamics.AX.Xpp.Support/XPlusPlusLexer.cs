namespace Arbela.Dynamics.AX.Xpp.Support
{
    using System.Collections.Generic;

    using PygmentSharp.Core;
    using PygmentSharp.Core.Lexing;
    using PygmentSharp.Core.Tokens;

    internal static class XPlusPlusLexerLevel
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
    /// A lexer for X++
    /// </summary>
    [Lexer("X++", AlternateNames = "xplusplus,x++,x-plusplus")]
    [LexerFileExtension("*.xpp")]
    public class XPlusPlusLexer : RegexLexer
    {
        // X++ intrinsic (compile-time) functions
        private const string IntrinsicFunctions =
            @"(classStr|tableStr|formStr|menuItemDisplayStr|menuItemActionStr|menuItemOutputStr|" +
            @"extendedTypeStr|enumStr|queryStr|resourceStr|ssrsReportStr|" +
            @"fieldStr|fieldPName|tableFieldGroupStr|tableMethodStr|classMethodStr|" +
            @"staticMethodStr|tableStaticMethodStr|delegateStr|" +
            @"tableNum|fieldNum|enumNum|configurationKeyStr|configurationKeyNum|" +
            @"licenseCodeStr|licenseCodeNum|identifierStr|literalStr|" +
            @"funcName|prmIsDefault|methodStr|typeOf|" +
            @"classNum|formControlStr|webMenuItemStr|webActionItemStr|" +
            @"maxDate|maxInt|minInt|dateNull|systemDateGet|" +
            @"evalBuf|runBuf|curExt|curUserId|" +
            @"classIdGet|dimOf|typeId|indexStr|indexNum)";

        // X++ keywords (flow control, OOP, data access, transactions, modifiers)
        // Derived from Microsoft.Dynamics.Framework.Tools.LanguageService.Parser.Tokens enum
        private const string Keywords =
            @"(abstract|as|base|break|breakpoint|case|catch|" +
            @"const|continue|default|" +
            @"do|else|enum|event|eventhandler|extends|false|final|finally|" +
            @"for|foreach|generateonly|goto|hint|if|implements|in|index|interface|" +
            @"internal|is|new|null|" +
            @"out|override|print|private|protected|public|readonly|" +
            @"ref|retry|return|reverse|set|static|" +
            @"switch|super|this|throw|true|try|unchecked|" +
            @"virtual|while|" +
            // X++ data access
            @"select|where|from|join|exists|notexists|outer|" +
            @"firstonly|firstonly1|firstonly10|firstonly100|firstonly1000|firstfast|" +
            @"forupdate|nofetch|forceselectorder|forceliterals|" +
            @"forceplaceholders|forcenestedloop|" +
            @"optimisticlock|pessimisticlock|repeatableread|" +
            @"validtimestate|crosscompany|" +
            @"order|by|asc|desc|like|" +
            @"count|sum|avg|minof|maxof|group|" +
            @"delete_from|insert_recordset|update_recordset|" +
            // X++ transactions
            @"ttsbegin|ttscommit|ttsabort|flush|changecompany|" +
            // X++ modifiers
            @"display|edit|server|client|" +
            @"next|byref|" +
            // X++ operators as keywords
            @"mod|div)";

        // X++ type keywords
        private const string TypeKeywords =
            @"(int|int64|str|real|date|utcdatetime|guid|" +
            @"anytype|boolean|container|void|" +
            @"tableId|fieldId|classId|identifiername|" +
            @"string|object|var)";

        /// <summary>
        /// Gets the state transition rules for the lexer. Each time a regex is matched,
        /// the internal state machine can be bumped to a new state which determines what
        /// regexes become valid again
        /// </summary>
        /// <returns></returns>
        protected override IDictionary<string, StateRule[]> GetStateRules()
        {
            var rules = new Dictionary<string, StateRule[]>();
            var cs_ident = XPlusPlusLexerLevel.Full;
            var builder = new StateRuleBuilder();

            rules["root"] = builder.NewRuleSet()
                // Method signatures: return type + method name + (
                .ByGroups(@"^([ \t]*(?:" + cs_ident + @"(?:\[\])?\s+)+?)" +  // return type
                                 @"(" + cs_ident +   @")" +                  // method name
                                 @"(\s*)(\()",                               // signature start
                    new LexerGroupProcessor(this),
                    new TokenGroupProcessor(TokenTypes.Name.Function),
                    new TokenGroupProcessor(TokenTypes.Text),
                    new TokenGroupProcessor(TokenTypes.Punctuation))

                // Attributes: [SysEntryPointAttribute, ...]
                .Add(@"^\s*\[.*?\]", TokenTypes.Name.Attribute)
                .Add(@"[^\S\n]+", TokenTypes.Text)
                .Add(@"\\\n", TokenTypes.Text) // line continuation

                // XML doc comments (/// ...) — must come before single-line comment rule
                .Add(@"///.*?\n", TokenTypes.String.Doc)
                // Comments
                .Add(@"//.*?\n", TokenTypes.Comment.Single)
                .Add(@"/[*].*?[*]/", TokenTypes.Comment.Multiline)

                .Add(@"\n", TokenTypes.Text)

                // Operators (multi-char first, then single-char)
                .Add(@"(==|!=|>=|<=|<<|>>|\+\+|--|&&|\|\||[+\-*/%=!<>&|^~?])", TokenTypes.Operator)
                // Structural punctuation
                .Add(@"[{}()\[\]:;,.]", TokenTypes.Punctuation)

                // String literals
                .Add(@"""(\\\\|\\""|[^""\n])*[""\n]", TokenTypes.String)
                .Add(@"'(\\\\|\\'|[^'])*'", TokenTypes.String)

                // Numeric literals
                .Add(@"[0-9](\.[0-9]*)?([eE][+-][0-9]+)?" +
                                 @"[flFLdD]?|0[xX][0-9a-fA-F]+[Ll]?", TokenTypes.Number)

                // Preprocessor directives
                .Add(@"#[ \t]*(if|endif|else|elif|define|undef|" +
                                 @"line|error|warning|region|endregion|pragma|localmacro|endmacro|" +
                                 @"globaldefine|linenumber|defjob)\b.*?\n", TokenTypes.Comment.Preproc)
                // Macro usage: #MacroName
                .Add(@"#[a-zA-Z_]\w*", TokenTypes.Comment.Preproc)

                // Label literals: @SYS12345, @Module:LabelId
                .Add(@"@[A-Za-z][A-Za-z0-9]*:[A-Za-z][A-Za-z0-9_]*", TokenTypes.Name.Label)
                .Add(@"@[A-Z]{2,}[0-9]+", TokenTypes.Name.Label)

                // Intrinsic functions: classStr(...), tableStr(...), etc.
                .ByGroups(IntrinsicFunctions + @"(\s*\()",
                    new TokenGroupProcessor(TokenTypes.Name.Builtin),
                    new TokenGroupProcessor(TokenTypes.Punctuation))

                // Static method access: ClassName::
                .ByGroups(@"([A-Z][a-zA-Z0-9_]*)(::)",
                    new TokenGroupProcessor(TokenTypes.Name.Class),
                    new TokenGroupProcessor(TokenTypes.Punctuation))

                // global:: special case
                .ByGroups(@"(global)(::)",
                    new TokenGroupProcessor(TokenTypes.Keyword),
                    new TokenGroupProcessor(TokenTypes.Punctuation))

                // X++ keywords
                .Add(Keywords + @"\b", TokenTypes.Keyword)

                // X++ type keywords
                .Add(TypeKeywords + @"\b\??", TokenTypes.Keyword.Type)

                // class/struct declaration — push to 'class' state to highlight the name
                .ByGroups(@"(class|struct)(\s+)", "class",
                    new TokenGroupProcessor(TokenTypes.Keyword),
                    new TokenGroupProcessor(TokenTypes.Text))
                // namespace/using declaration
                .ByGroups(@"(namespace|using)(\s+)", "namespace",
                    new TokenGroupProcessor(TokenTypes.Keyword),
                    new TokenGroupProcessor(TokenTypes.Text))

                // Generic identifiers (must be last)
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
