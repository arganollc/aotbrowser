using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using PygmentSharp.Core.Tokens;

namespace PygmentSharp.Core.Lexing
{
    /// <summary>
    /// Represents a rule for matching syntax. If the regex matches, the tokenTpye is emitted and the action is applied
    /// </summary>
    public class StateRule
    {
        /// <summary>
        /// Gets the regular expression to attempt to match
        /// </summary>
        public Regex Regex { get; }

        /// <summary>
        /// Gets the TokenType that should be emitted if the regex matches
        /// </summary>
        public TokenType TokenType { get; }

        /// <summary>
        /// Gets the action to take when a match occurs
        /// </summary>
        /// <remarks>
        /// Matches might choose to change the current state machine state, or emit more than one token, etc
        /// </remarks>
        public StateAction Action { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateRule"/> structure
        /// </summary>
        /// <remarks>
        /// Internal to force external plugins to use the builder
        /// </remarks>
        /// <param name="regex">The regular expression to attempt to match</param>
        /// <param name="tokenType">The type of token that should be emitted when matched</param>
        /// <param name="action">An action to take when matched</param>
        internal StateRule(Regex regex, TokenType tokenType, StateAction action)
        {
            Regex = regex;
            TokenType = tokenType;
            Action = action;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return $"{Regex} -> {TokenType}";
        }
    }


}