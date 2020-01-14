using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using PygmentSharp.Core.Tokens;
using PygmentSharp.Core.Utils;

namespace PygmentSharp.Core.Lexing
{
    /// <summary>
    /// Assists in creating <see cref="StateRule"/> state machine steps
    /// </summary>
    /// <remarks>
    /// State rules support a couple special rule names:
    /// <list type="table">
    ///     <listheader><term>Special State</term><description>Description</description></listheader>
    ///     <item><term><c>#pop</c></term><description>Pops the current state off the stack and returns the lexer to the previous state</description></item>
    ///     <item><term><c>#push</c></term><description>Pushes the current state onto the lexer stack again</description></item>
    /// </list>
    /// </remarks>
    public class StateRuleBuilder
    {
        /// <summary>
        /// Gets or sets the default regular expression options that subsequent
        /// rules should use
        /// </summary>
        public RegexOptions DefaultRegexOptions { get; set; } = RegexOptions.None;

        /// <summary>
        /// Creates a builder for a new set of rules
        /// </summary>
        /// <returns>A rules set builder</returns>
        public FluentStateRuleBuilder NewRuleSet()
        {
            return new FluentStateRuleBuilder(DefaultRegexOptions);
        }

        /// <summary>
        /// Builder object for state rules
        /// </summary>
        public class FluentStateRuleBuilder
        {
            private const int ListDefaultCapacity = 16;

            private readonly RegexOptions _defaultRegexOptions;
            private readonly List<StateRule> _rules = new List<StateRule>(ListDefaultCapacity);

            /// <summary>
            /// Converts the builder into the finalized list of rules
            /// </summary>
            /// <returns></returns>
            public StateRule[] Build() => _rules.ToArray();

            internal FluentStateRuleBuilder(RegexOptions defaultRegexOptions)
            {
                _defaultRegexOptions = defaultRegexOptions;
            }

            /// <summary>
            /// Adds a new rule to the list
            /// </summary>
            /// <param name="regex">The regular expression to match</param>
            /// <param name="tokenType">The type of token to emit</param>
            /// <param name="nextState">The next state to transition the machine to. See the docs for <see cref="StateRuleBuilder"/> for details</param>
            /// <returns></returns>
            public FluentStateRuleBuilder Add(string regex, TokenType tokenType, string nextState)
            {
                Argument.EnsureNotNull(regex, nameof(regex));
                Argument.EnsureNotNull(tokenType, nameof(tokenType));
                Argument.EnsureNotNull(nextState, nameof(nextState));

                _rules.Add(new StateRule(CreateRegex(regex), tokenType, Parse(nextState)));
                return this;
            }

            /// <summary>
            /// Adds a new rule to the list that will push multiple new states onto the state machine stack
            /// </summary>
            /// <param name="regex">regular expression to match</param>
            /// <param name="tokenType">type of token to emit</param>
            /// <param name="nextStates">a sequence of states to evaluate onto the state machine stack. See the docs for <see cref="StateRuleBuilder"/> for details</param>
            /// <returns></returns>
            public FluentStateRuleBuilder Add(string regex, TokenType tokenType, params string[] nextStates)
            {
                Argument.EnsureNotNull(regex, nameof(regex));
                Argument.EnsureNotNull(tokenType, nameof(tokenType));

                _rules.Add(new StateRule(CreateRegex(regex), tokenType,
                    new CombinedAction(nextStates.Select(Parse).ToArray())));
                return this;
            }

            /// <summary>
            /// Adds a new rule to the list that will emit a token but make no changes to the lexer state
            /// </summary>
            /// <param name="regex">The regular expression to match</param>
            /// <param name="tokenType">The type of token to emit if matched</param>
            /// <returns></returns>
            public FluentStateRuleBuilder Add(string regex, TokenType tokenType)
            {
                Argument.EnsureNotNull(regex, nameof(regex));
                Argument.EnsureNotNull(tokenType, nameof(tokenType));

                _rules.Add(new StateRule(CreateRegex(regex), tokenType, new NoopAction()));
                return this;
            }

            /// <summary>
            /// Adds a rule to the list that pushes a set of states to the lexer stack if no previous rule had matched
            /// </summary>
            /// <param name="states">The states to push onto the lexer stack</param>
            /// <returns></returns>
            public FluentStateRuleBuilder Default(params string[] states)
            {
                _rules.Add(new StateRule(CreateRegex(""), TokenTypes.Token,
                    new CombinedAction(states.Select(Parse).ToArray())));
                return this;
            }

            /// <summary>
            /// Copies existing rules into the rule set
            /// </summary>
            /// <param name="existing">The list of existing rules to copy</param>
            /// <returns></returns>
            public FluentStateRuleBuilder Include(IEnumerable<StateRule> existing)
            {
                Argument.EnsureNotNull(existing, nameof(existing));

                _rules.AddRange(existing);
                return this;
            }

            /// <summary>
            /// Adds a rule who's regular expression matches a set of groups, and applies processing to each
            /// of those match group values
            /// </summary>
            /// <remarks>
            /// For example, a regular expression might match a 3 groups for attribute, =, and attribute value and
            /// use processors to emit a token for each
            /// </remarks>
            /// <param name="regex">The regular expression to match. Should have a sequence of groups that correspond to a processor</param>
            /// <param name="processors">The actions to apply to each matched group</param>
            /// <returns></returns>
            public FluentStateRuleBuilder ByGroups(string regex, params GroupProcessor[] processors)
            {
                Argument.EnsureNotNull(regex, nameof(regex));
                Argument.EnsureNotNull(processors, nameof(processors));

                _rules.Add(new StateRule(CreateRegex(regex), TokenTypes.Token,
                    new GroupAction(processors)));
                return this;
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="regex"></param>
            /// <param name="tokens"></param>
            /// <returns></returns>
            public FluentStateRuleBuilder ByGroups(string regex, params TokenType[] tokens)
            {
                Argument.EnsureNotNull(regex, nameof(regex));
                var processors = tokens.Select(t => new TokenGroupProcessor(t)).ToList();
                _rules.Add(new StateRule(CreateRegex(regex), TokenTypes.Token, new GroupAction(processors)));
                return this;
            }

            /// <summary>
            /// Adds a rule who's regular expression matches a set of groups, and applies processing to each
            /// of those match group values. A new state is then taken inside of the lexer
            /// </summary>
            /// <remarks>
            /// For example, a regular expression might match a 3 groups for attribute, =, and attribute value and
            /// use processors to emit a token for each
            /// </remarks>
            /// <param name="regex">The regular expression to match. Should have a sequence of groups that correspond to a processor</param>
            /// <param name="newState">The new state to apply to the lexer stack</param>
            /// <param name="processors">The actions to apply to each matched group</param>
            /// <returns></returns>
            public FluentStateRuleBuilder ByGroups(string regex, string newState, params GroupProcessor[] processors)
            {
                Argument.EnsureNotNull(regex, nameof(regex));
                Argument.EnsureNotNull(newState, nameof(newState));
                Argument.EnsureNotNull(processors, nameof(processors));

                _rules.Add(new StateRule(CreateRegex(regex), TokenTypes.Token,
                    new GroupAction(Parse(newState), processors)));
                return this;
            }

            /// <summary>
            /// Adds a rule who's regular expression matches a set of groups, and applies processing to each
            /// of those match group values. A set of new states are then pushed onto the lexer stack
            /// </summary>
            /// <remarks>
            /// For example, a regular expression might match a 3 groups for attribute, =, and attribute value and
            /// use processors to emit a token for each
            /// </remarks>
            /// <param name="regex">The regular expression to match. Should have a sequence of groups that correspond to a processor</param>
            /// <param name="newStates">The set of new state to apply to the lexer stack</param>
            /// <param name="processors">The actions to apply to each matched group</param>
            /// <returns></returns>
            public FluentStateRuleBuilder ByGroups(string regex, string[] newStates, params GroupProcessor[] processors)
            {
                Argument.EnsureNotNull(regex, nameof(regex));
                Argument.EnsureNotNull(newStates, nameof(newStates));
                Argument.EnsureNotNull(processors, nameof(processors));

                var actions = newStates.Select(Parse).ToArray();
                _rules.Add(new StateRule(CreateRegex(regex), TokenTypes.Token,
                    new GroupAction(new CombinedAction(actions), processors)));
                return this;
            }

            /// <summary>
            /// Adds a rule that processes a match by executing another <see cref="Lexer"/>
            /// </summary>
            /// <remarks>
            /// This is useful for lexing a language that embeds other languages. For example, HTML embeds CSS and Javsacript
            /// languages inside style and script tags
            /// </remarks>
            /// <typeparam name="T">The type of lexer to invoke</typeparam>
            /// <param name="regex">The regular expression to match and process</param>
            /// <returns></returns>
            public FluentStateRuleBuilder Using<T>(string regex) where T : Lexer, new()
            {
                Argument.EnsureNotNull(regex, nameof(regex));

                var lexer = new T();
                _rules.Add(new StateRule(CreateRegex(regex), TokenTypes.Token, new LexerAction(lexer)));
                return this;
            }

            private StateChangingAction Parse(string name)
            {
                if (name == "#push")
                    return new PushAgainAction();
                if (name == "#pop")
                    return new PopAction();

                return new PushStateAction(name);
            }

            private Regex CreateRegex(string regex)
            {
                // wrap in non capturing group and apply the \G prefix, which means
                // to start looking at a provided index
                return new Regex(@"\G(?:" + regex + ")", _defaultRegexOptions);
            }
        }
    }
}