using System;
using System.Collections.Generic;

namespace Calculator
{
    class Evaluator
    {
        public List<BinaryOperator> BinaryOperators { get; set; }
        public List<Function> Functions { get; set; }
        public Dictionary<string, Variable> Variables { get; set; }
        
        public object Evaluate(INode node)
        {
            Expression expression = node as Expression;
            if (expression != null)
            {
                return Evaluate(expression.Value);
            }
            Symbol symbol = node as Symbol;
            if (symbol != null)
            {
                return EvaluateSymbol(symbol);
            }
            Number number = node as Number;
            if (number != null)
            {
                return EvaluateNumber(number);
            }
            BinaryOperatorCall binaryOperatorCall = node as BinaryOperatorCall;
            if (binaryOperatorCall != null)
            {
                return EvaluateBinaryOperatorCall(binaryOperatorCall);
            }
            FunctionCall functionCall = node as FunctionCall;
            if (functionCall != null)
            {
                return EvaluateFunctionCall(functionCall);
            }
            throw new Exception("Unknown node type: " + node);
        }

        private object EvaluateSymbol(Symbol symbol)
        {
            if (Variables.ContainsKey(symbol.Characters))
            {
                return Variables[symbol.Characters];
            }
            Variable variable = new Variable();
            Variables.Add(symbol.Characters, variable);
            return variable;
        }

        private object EvaluateNumber(Number number)
        {
            return number.Value;
        }

        private object EvaluateBinaryOperatorCall(BinaryOperatorCall binaryOperatorCall)
        {
            object leftOperand = Evaluate(binaryOperatorCall.LeftOperand);
            object rightOperand = Evaluate(binaryOperatorCall.RightOperand);
            BinaryOperator binaryOperator = FindBinaryOperatorByCharacters(binaryOperatorCall.Operator);
            return binaryOperator.Handler(leftOperand, rightOperand);
        }

        private object EvaluateFunctionCall(FunctionCall functionCall)
        {
            Function function = FindFunctionByName(functionCall.Function);
            object[] arguments;
            if (functionCall.Arguments != null)
            {
                object rawArguments = Evaluate(functionCall.Arguments);
                List<object> argumentList = rawArguments as List<object>;
                if (argumentList != null)
                {
                    arguments = new object[argumentList.Count];
                    for (int i = 0; i < arguments.Length; i++)
                    {
                        Variable variable = argumentList[i] as Variable;
                        if (variable != null)
                        {
                            arguments[i] = variable.Value;
                        }
                        else
                        {
                            arguments[i] = argumentList[i];
                        }
                    }
                }
                else
                {
                    Variable variable = rawArguments as Variable;
                    if (variable != null)
                    {
                        arguments = new object[] {variable.Value};
                    }
                    else
                    {
                        arguments = new object[] { rawArguments };
                    }
                }
            }
            else
            {
                arguments = null;
            }
            return function.Handler.DynamicInvoke(arguments);
        }

        private BinaryOperator FindBinaryOperatorByCharacters(string characters)
        {
            foreach (BinaryOperator binaryOperator in BinaryOperators)
            {
                if (binaryOperator.Characters == characters)
                {
                    return binaryOperator;
                }
            }
            throw new Exception("Binary operator is not found: " + characters);
        }

        private Function FindFunctionByName(string name)
        {
            foreach (Function function in Functions)
            {
                if (function.Name == name)
                {
                    return function;
                }
            }
            throw new Exception("Function is not found: " + name);
        }
    }
}
