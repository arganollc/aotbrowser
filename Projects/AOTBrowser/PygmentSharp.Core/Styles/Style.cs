using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PygmentSharp.Core.Tokens;

namespace PygmentSharp.Core.Styles
{
    /// <summary>
    /// Represents a mapping of token types to text formatting
    /// </summary>
    public class Style : IEnumerable<KeyValuePair<TokenType, StyleData>>
    {
        private readonly Dictionary<TokenType, StyleData> _styles;

        /// <summary>
        /// Initializes a new instance of the <see cref="Style"/> class
        /// </summary>
        public Style()
        {
            _styles = new Dictionary<TokenType, StyleData>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Style"/> class
        /// </summary>
        /// <param name="styles">A mapping of token types to parseable styledata specifiers</param>
        public Style(IDictionary<TokenType, string> styles)
        {
            _styles = ParseStyles(styles);
        }

        private static Dictionary<TokenType, StyleData> ParseStyles(IDictionary<TokenType, string> styles)
        {
            foreach (var ttype in TokenTypeMap.Instance.Keys)
            {
                if (!styles.ContainsKey(ttype))
                    styles[ttype] = "";
            }

            var output = new Dictionary<TokenType, StyleData>();
            foreach (var style in styles)
            {
                foreach (var ttype in style.Key.Split())
                {
                    if (output.ContainsKey(ttype))
                        continue;

                    var styledefs = styles[ttype] ?? "";

                    var parentStyle = ttype.Parent == null ? null : output[ttype.Parent];

                    if (parentStyle == null)
                        parentStyle = new StyleData();
                    else if (style.Value.Contains("noinherit") && ttype != TokenTypes.Token)
                        parentStyle = output[TokenTypes.Token]; //inherit from Token

                    output[ttype] = StyleData.Parse(styledefs, parentStyle);
                }
            }

            return output;
        }

        /// <summary>
        /// Gets or sets the background color
        /// </summary>
        public string BackgroundColor { get; set; } = "#ffffff";

        /// <summary>
        /// Gets or sets the highlight color
        /// </summary>
        public string HighlightColor { get; set; } = "#ffffcc";

        /// <summary>
        /// Gets the style for a provided TokenType
        /// </summary>
        /// <param name="ttype">The type of token</param>
        /// <returns></returns>
        public StyleData StyleForToken(TokenType ttype)
        {
            return this[ttype];
        }

        /// <summary>
        /// Gets the style for a token type
        /// </summary>
        /// <param name="ttype">The type of token</param>
        public StyleData this[TokenType ttype] => _styles.ContainsKey(ttype) ? _styles[ttype] : null;

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<KeyValuePair<TokenType, StyleData>> GetEnumerator()
        {
            return _styles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _styles).GetEnumerator();
        }
    }
}
