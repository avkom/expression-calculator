using System;

namespace Calculator
{
    interface IPattern
    {
        bool TryApply(AutomationStack<INode> stack);
    }
    
    class BinaryOperatorCallPattern : IPattern
    {
        public bool TryApply(AutomationStack<INode> stack)
        {
            if (stack.Count >= 3)
            {
                INode node0 = stack[stack.Count - 3];
                INode node1 = stack[stack.Count - 2];
                INode node2 = stack[stack.Count - 1];
                if (node0 is Expression
                    && node1 is Operator
                    && node2 is Expression)
                {
                    stack.Pop();
                    stack.Pop();
                    stack.Pop();
                    BinaryOperatorCall binaryOperatorCall = new BinaryOperatorCall();
                    binaryOperatorCall.LeftOperand = node0;
                    binaryOperatorCall.Operator = ((Operator) node1).Characters;
                    binaryOperatorCall.RightOperand = node2;
                    Expression expression = new Expression();
                    expression.Value = binaryOperatorCall;
                    stack.Push(expression);
                    return true;
                }
            }
            return false;
        }
    }

    class FunctionCallPattern : IPattern
    {
        public bool TryApply(AutomationStack<INode> stack)
        {
            if (stack.Count >= 3)
            {
                INode node0 = stack[stack.Count - 3];
                INode node1 = stack[stack.Count - 2];
                INode node2 = stack[stack.Count - 1];
                if (node0 is Symbol
                    && node1 is LeftBracket
                    && node2 is RightBracket)
                {
                    stack.Pop();
                    stack.Pop();
                    stack.Pop();
                    FunctionCall functionCall = new FunctionCall();
                    functionCall.Function = ((Symbol)node0).Characters;
                    Expression expression = new Expression();
                    expression.Value = functionCall;
                    stack.Push(expression);
                    return true;
                }
            }
            if (stack.Count >= 4)
            {
                INode node0 = stack[stack.Count - 4];
                INode node1 = stack[stack.Count - 3];
                INode node2 = stack[stack.Count - 2];
                INode node3 = stack[stack.Count - 1];
                if (node0 is Symbol
                    && node1 is LeftBracket
                    && (node2 is Expression)
                    && node3 is RightBracket)
                {
                    stack.Pop();
                    stack.Pop();
                    stack.Pop();
                    stack.Pop();
                    FunctionCall functionCall = new FunctionCall();
                    functionCall.Function = ((Symbol)node0).Characters;
                    functionCall.Arguments = node2;
                    Expression expression = new Expression();
                    expression.Value = functionCall;
                    stack.Push(expression);
                    return true;
                }
            }
            return false;
        }
    }

    class ExpressionPattern : IPattern
    {
        public bool TryApply(AutomationStack<INode> stack)
        {
            if (stack.Count >= 1)
            {
                INode node0 = stack[stack.Count - 1];
                if (node0 is Symbol || node0 is Number || node0 is BinaryOperatorCall || node0 is FunctionCall)
                {
                    stack.Pop();
                    Expression expression = new Expression();
                    expression.Value = node0;
                    stack.Push(expression);
                    return true;
                }
            }
            if (stack.Count >= 3)
            {
                INode node0 = stack[stack.Count - 3];
                INode node1 = stack[stack.Count - 2];
                INode node2 = stack[stack.Count - 1];
                if (node0 is LeftBracket
                    && (node1 is Symbol || node1 is Number || node1 is BinaryOperatorCall || node1 is FunctionCall || node1 is Expression)
                    && node2 is RightBracket)
                {
                    stack.Pop();
                    stack.Pop();
                    stack.Pop();
                    Expression expression;
                    if (node1 is Expression)
                    {
                        expression = (Expression) node1;
                    }
                    else
                    {
                        expression = new Expression();
                        expression.Value = node1;
                    }
                    stack.Push(expression);
                    return true;
                }
            }
            return false;
        }
    }
}
