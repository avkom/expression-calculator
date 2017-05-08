namespace Calculator
{
    public interface INode
    {
    }


    // Terminal nodes
    // They are created by Lexer

    public class Symbol : INode
    {
        public string Characters { get; set; }

        public override string ToString()
        {
            return "Symbol: " + Characters;
        }
    }

    public class Number : INode
    {
        public decimal Value { get; set; }

        public override string ToString()
        {
            return "Number: " + Value;
        }
    }

    public class LeftBracket : INode
    {
        public override string ToString()
        {
            return "LeftBracket";
        }
    }

    public class RightBracket : INode
    {
        public override string ToString()
        {
            return "RightBracket";
        }
    }

    public class Operator : INode
    {
        public string Characters { get; set; }

        public override string ToString()
        {
            return "Operator: " + Characters;
        }
    }

    // Bound is terminal node, but it is created by Parser
    public class Bound : INode
    {
        public override string ToString()
        {
            return "Bound";
        }
    }


    // Non-terminal nodes
    // They are created by Parser

    public class Expression : INode
    {
        public INode Value { get; set; }

        public override string ToString()
        {
            return "Expression: " + Value;
        }
    }
    
    public class BinaryOperatorCall : INode
    {
        public INode LeftOperand { get; set; }

        public INode RightOperand { get; set; }

        public string Operator { get; set; }

        public override string ToString()
        {
            return "BinaryOperatorCall: " + Operator;
        }
    }

    public class FunctionCall : INode
    {
        public INode Arguments { get; set; }

        public string Function { get; set; }

        public override string ToString()
        {
            return "FunctionCall: " + Function;
        }
    }
}
