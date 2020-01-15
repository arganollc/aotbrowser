using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using PygmentSharp.Core.Tokens;

namespace PygmentSharp.Core.Lexing
{
    /// <summary>
    /// When a rule is matched, the state action is applied to actually get the tokens
    /// </summary>
    public abstract class StateAction
    {
        /// <summary>
        /// Executes the action against the lexer state
        /// </summary>
        /// <param name="context">The lexer state</param>
        /// <returns>A list of tokens to emit</returns>
        public abstract IEnumerable<Token> Execute(RegexLexerContext context);
    }

    /// <summary>
    /// Gets tokens from match groups
    /// </summary>
    public class GroupAction : StateAction
    {
        /// <summary>
        /// Gets the action to be applied
        /// </summary>
        public StateChangingAction Action { get; }

        /// <summary>
        /// Gets the list of processors
        /// </summary>
        public IReadOnlyList<GroupProcessor> Processors { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupAction"/> class
        /// </summary>
        /// <param name="processors">The processors to apply to each match group</param>
        public GroupAction(params GroupProcessor[] processors)
        {
            Action = new NoopAction();
            Processors = processors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupAction"/> class
        /// </summary>
        /// <param name="processors">The processors to apply to each match group</param>
        public GroupAction(IReadOnlyList<GroupProcessor> processors)
        {
            Action = new NoopAction();
            Processors = processors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupAction"/> class
        /// </summary>
        /// <param name="action">The action to apply</param>
        /// <param name="processors">The processors to apply to each match group</param>
        public GroupAction(StateChangingAction action, params GroupProcessor[] processors)
            : this(processors)
        {
            Action = action ?? new NoopAction();
        }

        /// <summary>
        /// Executes the action against the lexer state
        /// </summary>
        /// <param name="context">The lexer state</param>
        /// <returns>A list of tokens to emit</returns>
        public override IEnumerable<Token> Execute(RegexLexerContext context)
        {
            if (context.Match.Groups.Count > Processors.Count + 1)
                throw new InvalidOperationException("Regex had more match groups than processors");

            for (int i = 1; i < context.Match.Groups.Count; i++)
            {
                var group = context.Match.Groups[i];
                var tokens = Processors[i-1].GetTokens(context, group.Value);
                foreach (var token in tokens)
                    yield return token;
            }

            Action.Apply(context.StateStack);
        }
    }

    /// <summary>
    /// Processes a state action be calling through to another lexer
    /// </summary>
    public class LexerAction : StateAction
    {
        /// <summary>
        /// Gets the lexer to execute
        /// </summary>
        public Lexer Lexer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LexerAction"/> clas
        /// </summary>
        /// <param name="lexer">the lexer to invoke</param>
        public LexerAction(Lexer lexer)
        {
            Lexer = lexer;
        }

        /// <summary>
        /// Executes the action against the lexer state
        /// </summary>
        /// <param name="context">The lexer state</param>
        /// <returns>A list of tokens to emit</returns>
        public override IEnumerable<Token> Execute(RegexLexerContext context)
        {
            int offset = context.Position;

            var tokens = Lexer.GetTokens(context.Match.Value);
            foreach (var token in tokens)
                yield return token.Offset(offset);

            context.Position += context.Match.Length;
        }
    }

    /// <summary>
    /// Base class for custom actions
    /// </summary>
    public abstract class StateChangingAction : StateAction
    {
        /// <summary>
        /// When overridden in a child class, applies configured changes to the lexer state
        /// </summary>
        /// <param name="stateStack"></param>
        public abstract void Apply(Stack<string> stateStack);

        /// <summary>
        /// Yields a token and applies configured actions against the stack
        /// </summary>
        /// <param name="context">The current lexer context</param>
        /// <returns>A sequence of tokens</returns>
        public override IEnumerable<Token> Execute(RegexLexerContext context)
        {
            if(context.Match.Value != "")
                yield return new Token(context.Position, context.RuleTokenType, context.Match.Value);

            Apply(context.StateStack);
            context.Position += context.Match.Length;
        }
    }

    /// <summary>
    /// When applied, pushes a new state onto the top of the stack
    /// </summary>
    public class PushStateAction : StateChangingAction
    {
        /// <summary>
        /// Gets the new state to push onto the stack
        /// </summary>
        public string DestinationState { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PushStateAction"/> class
        /// </summary>
        /// <param name="destinationState">The next state to push</param>
        public PushStateAction(string destinationState)
        {
            DestinationState = destinationState;
        }

        /// <summary>
        /// When overridden in a child class, applies configured changes to the lexer state
        /// </summary>
        /// <param name="stateStack"></param>
        public override void Apply(Stack<string> stateStack)
        {
            stateStack.Push(DestinationState);
        }
    }

    /// <summary>
    /// When applied, applies multiple other actions to the stack
    /// </summary>
    public class CombinedAction : StateChangingAction
    {
        /// <summary>
        /// Gets the actions that will be applied to the stack
        /// </summary>
        public IReadOnlyCollection<StateChangingAction> Actions { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedAction"/> class
        /// </summary>
        /// <param name="actions">The set of actions to apply to the state</param>
        public CombinedAction(params StateChangingAction[] actions)
        {
            Actions = actions;
        }

        /// <summary>
        /// When overridden in a child class, applies configured changes to the lexer state
        /// </summary>
        /// <param name="stateStack"></param>
        public override void Apply(Stack<string> stateStack)
        {
            foreach (var action in Actions)
            {
                action.Apply(stateStack);
            }
        }
    }

    /// <summary>
    /// When applied, nothing happens
    /// </summary>
    public class NoopAction : StateChangingAction
    {
        /// <summary>
        /// When overridden in a child class, applies configured changes to the lexer state
        /// </summary>
        /// <param name="stateStack"></param>
        public override void Apply(Stack<string> stateStack)
        {
            // this line intentionally left blank
        }
    }

    /// <summary>
    /// When applied, pushes the current state onto the stack a second time
    /// </summary>
    public class PushAgainAction : StateChangingAction
    {
        /// <summary>
        /// When overridden in a child class, applies configured changes to the lexer state
        /// </summary>
        /// <param name="stateStack"></param>
        public override void Apply(Stack<string> stateStack)
        {
            stateStack.Push(stateStack.Peek());
        }
    }

    /// <summary>
    /// When applied, pops the current state off the top of the stack
    /// </summary>
    public class PopAction : StateChangingAction
    {
        /// <summary>
        /// When overridden in a child class, applies configured changes to the lexer state
        /// </summary>
        /// <param name="stateStack"></param>
        public override void Apply(Stack<string> stateStack)
        {
            stateStack.Pop();
        }
    }

}