using System;
using System.Collections.Generic;

namespace Calculator
{
    public class Calculator
    {
        private readonly Lexer lexer = new Lexer();
        private readonly Parser parser = new Parser();
        private readonly Evaluator evaluator = new Evaluator();

        public List<BinaryOperator> BinaryOperators { get; private set; }
        public List<Function> Functions { get; private set; }
        public Dictionary<string, Variable> Variables { get; private set; }

        public Calculator()
        {
            BinaryOperators = new List<BinaryOperator>();
            Functions = new List<Function>();
            Variables = new Dictionary<string, Variable>();
            BinaryOperators.Add(new BinaryOperator(",", 0, Associativity.LeftToRight, CommaOperator));
            parser.BinaryOperators = BinaryOperators;
            evaluator.BinaryOperators = BinaryOperators;
            evaluator.Functions = Functions;
            evaluator.Variables = Variables;
        }

        public object Evaluate(string expression)
        {
            List<INode> lexemes = lexer.Parse(expression);
            INode expressionTree = parser.Parse(lexemes);
            object result = evaluator.Evaluate(expressionTree);
            return result;
        }

        private static object CommaOperator(object a, object b)
        {
            List<object> list = a as List<object>;
            if (list == null)
            {
                list = new List<object>();
                list.Add(a);
            }
            list.Add(b);
            return list;
        }
    }
}
