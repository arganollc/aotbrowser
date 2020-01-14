using System;
using System.Collections.Generic;
using System.Linq;

using PygmentSharp.Core.Tokens;

namespace PygmentSharp.Core.Styles
{
    internal class TokenTypeMap
    {
        private readonly IDictionary<TokenType, string> _map;

        public TokenTypeMap() : this(new Dictionary<TokenType, string>())
        {
        }

        public TokenTypeMap(IDictionary<TokenType, string> map)
        {
            _map = map;
        }

        public string this[TokenType type] => _map.ContainsKey(type) ? _map[type] : null;

        public bool Contains(TokenType type) => _map.ContainsKey(type);

        static TokenTypeMap()
        {
            Instance = new TokenTypeMap();
            Instance._map[TokenTypes.Token] = "";
            Instance._map[TokenTypes.Text] = "";
            Instance._map[TokenTypes.Whitespace] = "w";
            Instance._map[TokenTypes.Escape] = "esc";
            Instance._map[TokenTypes.Error] = "err";
            Instance._map[TokenTypes.Other] = "x";
            Instance._map[TokenTypes.Keyword] = "k";
            Instance._map[TokenTypes.Keyword.Constant] = "kc";
            Instance._map[TokenTypes.Keyword.Declaration] = "kd";
            Instance._map[TokenTypes.Keyword.Namespace] = "kn";
            Instance._map[TokenTypes.Keyword.Pseudo] = "kp";
            Instance._map[TokenTypes.Keyword.Reserved] = "kr";
            Instance._map[TokenTypes.Keyword.Type] = "kt";
            Instance._map[TokenTypes.Name] = "n";
            Instance._map[TokenTypes.Name.Attribute] = "na";
            Instance._map[TokenTypes.Name.Builtin] = "nb";
            Instance._map[TokenTypes.Name.Builtin.Pseudo] = "bp";
            Instance._map[TokenTypes.Name.Class] = "nc";
            Instance._map[TokenTypes.Name.Constant] = "no";
            Instance._map[TokenTypes.Name.Decorator] = "nd";
            Instance._map[TokenTypes.Name.Entity] = "ni";
            Instance._map[TokenTypes.Name.Exception] = "ne";
            Instance._map[TokenTypes.Name.Function] = "nf";
            Instance._map[TokenTypes.Name.Property] = "py";
            Instance._map[TokenTypes.Name.Label] = "nl";
            Instance._map[TokenTypes.Name.Namespace] = "nn";
            Instance._map[TokenTypes.Name.Other] = "nx";
            Instance._map[TokenTypes.Name.Tag] = "nt";
            Instance._map[TokenTypes.Name.Variable] = "nv";
            Instance._map[TokenTypes.Name.Variable.Class] = "vc";
            Instance._map[TokenTypes.Name.Variable.Global] = "vg";
            Instance._map[TokenTypes.Name.Variable.Instance] = "vi";
            Instance._map[TokenTypes.Literal] = "l";
            Instance._map[TokenTypes.Literal.Date] = "ld";
            Instance._map[TokenTypes.String] = "s";
            Instance._map[TokenTypes.String.Backtick] = "sb";
            Instance._map[TokenTypes.String.Char] = "sc";
            Instance._map[TokenTypes.String.Doc] = "sd";
            Instance._map[TokenTypes.String.Double] = "s2";
            Instance._map[TokenTypes.String.Escape] = "se";
            Instance._map[TokenTypes.String.Heredoc] = "sh";
            Instance._map[TokenTypes.String.Interpol] = "si";
            Instance._map[TokenTypes.String.Other] = "sx";
            Instance._map[TokenTypes.String.Regex] = "sr";
            Instance._map[TokenTypes.String.Single] = "s1";
            Instance._map[TokenTypes.String.Symbol] = "ss";
            Instance._map[TokenTypes.Number] = "m";
            Instance._map[TokenTypes.Number.Bin] = "mb";
            Instance._map[TokenTypes.Number.Float] = "mf";
            Instance._map[TokenTypes.Number.Hex] = "mh";
            Instance._map[TokenTypes.Number.Integer] = "mi";
            Instance._map[TokenTypes.Number.Integer.Long] = "il";
            Instance._map[TokenTypes.Number.Oct] = "mo";
            Instance._map[TokenTypes.Operator] = "o";
            Instance._map[TokenTypes.Operator.Word] = "ow";
            Instance._map[TokenTypes.Punctuation] = "p";
            Instance._map[TokenTypes.Comment] = "c";
            Instance._map[TokenTypes.Comment.Hashbang] = "ch";
            Instance._map[TokenTypes.Comment.Multiline] = "cm";
            Instance._map[TokenTypes.Comment.Preproc] = "cp";
            Instance._map[TokenTypes.Comment.PreprocFile] = "cpf";
            Instance._map[TokenTypes.Comment.Single] = "c1";
            Instance._map[TokenTypes.Comment.Special] = "cs";
            Instance._map[TokenTypes.Generic] = "g";
            Instance._map[TokenTypes.Generic.Deleted] = "gd";
            Instance._map[TokenTypes.Generic.Emph] = "ge";
            Instance._map[TokenTypes.Generic.Error] = "gr";
            Instance._map[TokenTypes.Generic.Heading] = "gh";
            Instance._map[TokenTypes.Generic.Inserted] = "gi";
            Instance._map[TokenTypes.Generic.Output] = "go";
            Instance._map[TokenTypes.Generic.Prompt] = "gp";
            Instance._map[TokenTypes.Generic.Strong] = "gs";
            Instance._map[TokenTypes.Generic.Subheading] = "gu";
            Instance._map[TokenTypes.Generic.Traceback] = "gt";

        }

        public static TokenTypeMap Instance { get; }
        public IEnumerable<TokenType> Keys => _map.Keys;
    }
}