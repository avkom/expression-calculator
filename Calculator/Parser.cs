using System;
using System.Collections.Generic;

namespace Calculator
{
    public class Parser
    {
        private readonly List<IPattern> patterns = new List<IPattern>();
        private readonly AutomationStack<INode> stack = new AutomationStack<INode>();
        private List<INode> lexemes;
        private int position;

        public List<BinaryOperator> BinaryOperators { get; set; }

        public Parser()
        {
            patterns.Add(new BinaryOperatorCallPattern());
            patterns.Add(new FunctionCallPattern());
            patterns.Add(new ExpressionPattern());
        }

        public INode Parse(List<INode> lexemes)
        {
            this.lexemes = lexemes;
            if (this.lexemes.Count == 0)
            {
                throw new Exception("Lexeme list is empty.");
            }
            this.lexemes.Insert(0, new Bound());
            this.lexemes.Add(new Bound());
            position = 0;
            stack.Clear();
            stack.Push(Current());
            MoveNext();
            while (!IsEndOfLexemeList())
            {
                INode topTerminalInStack = FindTopTerminalInStack();
                PrecedenceRelation precedenceRelation = GetPrecedenceRelation(topTerminalInStack, Current());
                if (precedenceRelation == PrecedenceRelation.None)
                {
                    throw new Exception("Terminal relation is missings.");
                }
                if (precedenceRelation == PrecedenceRelation.Less || precedenceRelation == PrecedenceRelation.Equal)
                {
                    stack.Push(Current());
                    MoveNext();
                }
                else if (precedenceRelation == PrecedenceRelation.More)
                {
                    RecognizePattern();
                }
                else if (precedenceRelation == PrecedenceRelation.Finish)
                {
                    break;
                }
            }
            return stack[1];
        }

        private INode Current()
        {
            return lexemes[position];
        }

        private bool MoveNext()
        {
            position++;
            return position < lexemes.Count;
        }

        private bool IsEndOfLexemeList()
        {
            return position >= lexemes.Count;
        }

        private INode FindTopTerminalInStack()
        {
            for (int topTerminalIndexInStack = stack.Count - 1; topTerminalIndexInStack >= 0; topTerminalIndexInStack--)
            {
                INode lexeme = stack[topTerminalIndexInStack];
                if (lexeme is Symbol
                    || lexeme is Number
                    || lexeme is LeftBracket
                    || lexeme is RightBracket
                    || lexeme is Bound
                    || lexeme is Operator)
                {
                    return lexeme;
                }
            }
            throw new Exception("Terminal is not found in stack.");
        }

        private PrecedenceRelation GetPrecedenceRelation(INode firstOperator, INode secondOperator)
        {
            PrecedenceRelation precedenceRelation = PrecedenceRelation.None;
            if (firstOperator is Symbol || firstOperator is Number)
            {
                if (secondOperator is Symbol || secondOperator is Number)
                {
                    precedenceRelation = PrecedenceRelation.None;
                }
                else if (secondOperator is LeftBracket)
                {
                    precedenceRelation = PrecedenceRelation.Equal;
                }
                else if (secondOperator is RightBracket)
                {
                    precedenceRelation = PrecedenceRelation.More;
                }
                else if (secondOperator is Bound)
                {
                    precedenceRelation = PrecedenceRelation.More;
                }
                else if (secondOperator is Operator)
                {
                    precedenceRelation = PrecedenceRelation.More;
                }
                else
                {
                    throw new Exception(string.Format("Unknown second operator: {0}", secondOperator));
                }
            }
            else if (firstOperator is LeftBracket)
            {
                if (secondOperator is Symbol || secondOperator is Number)
                {
                    precedenceRelation = PrecedenceRelation.Less;
                }
                else if (secondOperator is LeftBracket)
                {
                    precedenceRelation = PrecedenceRelation.Less;
                }
                else if (secondOperator is RightBracket)
                {
                    precedenceRelation = PrecedenceRelation.Equal;
                }
                else if (secondOperator is Bound)
                {
                    precedenceRelation = PrecedenceRelation.None;
                }
                else if (secondOperator is Operator)
                {
                    precedenceRelation = PrecedenceRelation.Less;
                }
                else
                {
                    throw new Exception(string.Format("Unknown second operator: {0}", secondOperator));
                }
            }
            else if (firstOperator is RightBracket)
            {
                if (secondOperator is Symbol || secondOperator is Number)
                {
                    precedenceRelation = PrecedenceRelation.None;
                }
                else if (secondOperator is LeftBracket)
                {
                    precedenceRelation = PrecedenceRelation.None;
                }
                else if (secondOperator is RightBracket)
                {
                    precedenceRelation = PrecedenceRelation.More;
                }
                else if (secondOperator is Bound)
                {
                    precedenceRelation = PrecedenceRelation.More;
                }
                else if (secondOperator is Operator)
                {
                    precedenceRelation = PrecedenceRelation.More;
                }
                else
                {
                    throw new Exception(string.Format("Unknown second operator: {0}", secondOperator));
                }
            }
            else if (firstOperator is Bound)
            {
                if (secondOperator is Symbol || secondOperator is Number)
                {
                    precedenceRelation = PrecedenceRelation.Less;
                }
                else if (secondOperator is LeftBracket)
                {
                    precedenceRelation = PrecedenceRelation.Less;
                }
                else if (secondOperator is RightBracket)
                {
                    precedenceRelation = PrecedenceRelation.None;
                }
                else if (secondOperator is Bound)
                {
                    precedenceRelation = PrecedenceRelation.Finish;
                }
                else if (secondOperator is Operator)
                {
                    precedenceRelation = PrecedenceRelation.Less;
                }
                else
                {
                    throw new Exception(string.Format("Unknown second operator: {0}", secondOperator));
                }
            }
            else if (firstOperator is Operator)
            {
                if (secondOperator is Symbol || secondOperator is Number)
                {
                    precedenceRelation = PrecedenceRelation.Less;
                }
                else if (secondOperator is LeftBracket)
                {
                    precedenceRelation = PrecedenceRelation.Less;
                }
                else if (secondOperator is RightBracket)
                {
                    precedenceRelation = PrecedenceRelation.More;
                }
                else if (secondOperator is Bound)
                {
                    precedenceRelation = PrecedenceRelation.More;
                }
                else if (secondOperator is Operator)
                {
                    BinaryOperator firstBinaryOperator = FindBinaryOperatorByNode(firstOperator);
                    BinaryOperator secondBinaryOperator = FindBinaryOperatorByNode(secondOperator);
                    if (firstBinaryOperator.Precedence > secondBinaryOperator.Precedence)
                    {
                        precedenceRelation = PrecedenceRelation.More;
                    }
                    else if (firstBinaryOperator.Precedence < secondBinaryOperator.Precedence)
                    {
                        precedenceRelation = PrecedenceRelation.Less;
                    }
                    else
                    {
                        if (firstBinaryOperator.Associativity != secondBinaryOperator.Associativity)
                        {
                            throw new Exception(string.Format("If operators {0} and {1} have the same precedence they must have the same associativity.", firstBinaryOperator, secondBinaryOperator));
                        }
                        if (firstBinaryOperator.Associativity == Associativity.LeftToRight)
                        {
                            precedenceRelation = PrecedenceRelation.More;
                        }
                        else
                        {
                            precedenceRelation = PrecedenceRelation.Less;
                        }
                    }
                }
                else
                {
                    throw new Exception(string.Format("Unknown second operator: {0}", secondOperator));
                }
            }
            else
            {
                throw new Exception(string.Format("Unknown first operator: {0}", firstOperator));
            }
            return precedenceRelation;
        }

        private BinaryOperator FindBinaryOperatorByNode(INode lexeme)
        {
            return BinaryOperators.Find(binaryOperator => binaryOperator.Characters == ((Operator)lexeme).Characters);
        }

        private void RecognizePattern()
        {
            foreach (IPattern pattern in patterns)
            {
                bool successful = pattern.TryApply(stack);
                if (successful)
                {
                    return;
                }
            }
            throw new Exception("Pattern is not found.");
        }

        private enum PrecedenceRelation
        {
            None,
            Less,
            Equal,
            More,
            Finish
        }
    }
}
