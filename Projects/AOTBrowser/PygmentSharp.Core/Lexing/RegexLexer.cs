using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

using PygmentSharp.Core.Tokens;

namespace PygmentSharp.Core.Lexing
{
    /// <summary>
    /// Defines the current context of the lexer
    /// </summary>
    public class RegexLexerContext
    {
        /// <summary>
        /// gets or sets the position
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets the current match
        /// </summary>
        public Match Match { get; }

        /// <summary>
        /// Gets the current stack of lexer states
        /// </summary>
        public Stack<string> StateStack { get; }

        /// <summary>
        /// Gets the current token type for the rule
        /// </summary>
        public TokenType RuleTokenType { get; }

        /// <summary>
        /// Initializes a new isntance of the <see cref="RegexLexerContext"/> class
        /// </summary>
        /// <param name="position">the position into the source file</param>
        /// <param name="match">the regular expression match data</param>
        /// <param name="stateStack">The stack of states</param>
        /// <param name="ruleTokenType">The token type the rule specified to emit</param>
        public RegexLexerContext(int position, Match match, Stack<string> stateStack, TokenType ruleTokenType)
        {
            Position = position;
            Match = match;
            StateStack = stateStack;
            RuleTokenType = ruleTokenType;
        }
    }

    /// <summary>
    /// Lexes a file through a regular expression based state machine
    /// </summary>
    public abstract class RegexLexer : Lexer
    {
        /// <summary>
        /// When overridden in a child class, gets all the <see cref="Token"/>s for the given string
        /// </summary>
        /// <param name="text">The string to tokenize</param>
        /// <returns>A sequence of <see cref="Token"/> structs</returns>
        protected override IEnumerable<Token> GetTokensUnprocessed(string text)
        {
            var rules = GetStateRules();
            int pos = 0;
            var stateStack = new Stack<string>(50);
            stateStack.Push("root");
            var currentStateRules = rules[stateStack.Peek()];

            while (true)
            {
                bool found = false;
                foreach (var rule in currentStateRules)
                {
                    var m = rule.Regex.Match(text, pos);
                    if (m.Success)
                    {
                        var context = new RegexLexerContext(pos, m, stateStack, rule.TokenType);
                        Debug.Assert(m.Index == pos, $"Regex \"{rule.Regex}\" should have matched at position {pos} but matched at {m.Index}");

                        var tokens = rule.Action.Execute(context);

                        foreach (var token in tokens)
                            yield return token;

                        pos = context.Position;
                        currentStateRules = rules[stateStack.Peek()];
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    if (pos >= text.Length)
                        break;

                    if (text[pos] == '\n')
                    {
                        stateStack.Clear();
                        stateStack.Push("root");
                        currentStateRules = rules["root"];
                        yield return new Token(pos, TokenTypes.Text, "\n");
                        pos++;
                        continue;
                    }

                    yield return new Token(pos, TokenTypes.Error, text[pos].ToString());
                    pos++;
                }
            }


        }

        /// <summary>
        /// Gets the state transition rules for the lexer. Each time a regex is matched,
        /// the internal state machine can be bumped to a new state which determines what
        /// regexes become valid again
        /// </summary>
        /// <returns></returns>
        protected abstract IDictionary<string, StateRule[]> GetStateRules();
    }
}