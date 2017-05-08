using System;

namespace Calculator
{
    public class BinaryOperator
    {
        public string Characters { get; set; }
        public int Precedence { get; set; }
        public Associativity Associativity { get; set; }
        public Func<object, object, object> Handler { get; set; }

        public BinaryOperator()
        {
        }
        
        public BinaryOperator(string characters, int precedence, Associativity associativity,
                              Func<object, object, object> handler)
        {
            Characters = characters;
            Precedence = precedence;
            Associativity = associativity;
            Handler = handler;
        }

        public static Func<object, object, object> CreateHandler<T>(Func<T, T, T> handler)
        {
            return (a, b) =>
                {
                    Variable aAsVariable = a as Variable;
                    if (aAsVariable != null)
                    {
                        a = aAsVariable.Value;
                    }
                    Variable bAsVariable = b as Variable;
                    if (bAsVariable != null)
                    {
                        b = bAsVariable.Value;
                    }
                    return handler((T)a, (T)b);
                };
        }

        public static Func<object, object, object> CreateAssingnmentHandler<T>(Func<T, T, T> handler)
        {
            return (a, b) =>
            {
                Variable aAsVariable = a as Variable;
                if (aAsVariable == null)
                {
                    throw new Exception(a + " is not lvalue.");
                }
                Variable bAsVariable = b as Variable;
                if (bAsVariable != null)
                {
                    b = bAsVariable.Value;
                }
                aAsVariable.Value = handler((T)a, (T)b);
                return aAsVariable.Value;
            };
        }
    }

    public enum Associativity
    {
        LeftToRight,
        RightToLeft
    }
}
