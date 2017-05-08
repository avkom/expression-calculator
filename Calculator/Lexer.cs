using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator
{
    class Lexer
    {
        private string expression;
        private int position;
        readonly List<INode> lexemes = new List<INode>();

        private const string operatorCharacters = "`~!#$%^&*-+=\\|[{]};:\'\",<.>/?№";
        
        public List<INode> Parse(string expression)
        {
            lexemes.Clear();
            this.expression = expression;
            position = 0;
            while (!IsEndOfExpression())
            {
                SkipWhiteSpaces();
                if (IsEndOfExpression())
                {
                    break;
                }
                if (Current() == '(')
                {
                    ReadLeftBracket();
                    continue;
                }
                if (Current() == ')')
                {
                    ReadRightBracket();
                    continue;
                }
                if (char.IsLetter(Current()) || Current() == '_')
                {
                    ReadSymbol();
                    continue;
                }
                if (char.IsDigit(Current()))
                {
                    ReadNumber();
                    continue;
                }
                if (IsOperator())
                {
                    ReadOperator();
                    continue;
                }
                throw new Exception("Unknown character " + Current() + " at " + position);
            }
            return lexemes;
        } 

        private char Current()
        {
            return expression[position];
        }

        private bool MoveNext()
        {
            position++;
            return position < expression.Length;
        }

        private void SkipWhiteSpaces()
        {
            while (position < expression.Length && char.IsWhiteSpace(Current()))
            {
                position++;
            }
        }

        private bool IsEndOfExpression()
        {
            return position >= expression.Length;
        }

        private bool IsOperator()
        {
            foreach (char operatorCharacter in operatorCharacters)
            {
                if (Current() == operatorCharacter)
                {
                    return true;
                }
            }
            return false;
        }

        private void ReadLeftBracket()
        {
            LeftBracket leftBracket = new LeftBracket();
            lexemes.Add(leftBracket);
            MoveNext();
        }

        private void ReadRightBracket()
        {
            RightBracket rightBracket = new RightBracket();
            lexemes.Add(rightBracket);
            MoveNext();
        }

        private void ReadSymbol()
        {
            StringBuilder symbolCharacters = new StringBuilder();
            symbolCharacters.Append(Current());
            while (MoveNext())
            {
                if (char.IsLetter(Current()) || char.IsDigit(Current()) || Current() == '_')
                {
                    symbolCharacters.Append(Current());
                }
                else
                {
                    break;
                }
            }
            Symbol symbol = new Symbol();
            symbol.Characters = symbolCharacters.ToString();
            lexemes.Add(symbol);
        }

        private void ReadNumber()
        {
            decimal integralPart = ReadNumberIntegralPart();
            decimal fractionalPart = 0;
            if (!IsEndOfExpression())
            {
                if (Current() == '.')
                {
                    MoveNext();
                    if (!IsEndOfExpression())
                    {
                        if (char.IsDigit(Current()))
                        {
                            fractionalPart = ReadNumberFractionalPart();
                        }
                        else
                        {
                            throw new Exception(Current() + " got, digit expected at " + position);
                        }
                    }
                    else
                    {
                        throw new Exception("End of line got, digit expected at " + position);
                    }
                }
            }
            Number number = new Number();
            number.Value = integralPart + fractionalPart;
            lexemes.Add(number);
        }

        private decimal ReadNumberIntegralPart()
        {
            decimal result = (decimal)char.GetNumericValue(Current());
            while (MoveNext())
            {
                if (char.IsDigit(Current()))
                {
                    result *= 10;
                    result += (decimal)char.GetNumericValue(Current());
                }
                else
                {
                    break;
                }
            }
            return result;
        }

        private decimal ReadNumberFractionalPart()
        {
            decimal result = 0;
            decimal ratio = new decimal(0.1);
            while (!IsEndOfExpression())
            {
                if (char.IsDigit(Current()))
                {
                    result += (decimal) char.GetNumericValue(Current())*ratio;
                    ratio /= 10;
                    MoveNext();
                }
                else
                {
                    break;
                }
            }
            return result;
        }

        private void ReadOperator()
        {
            StringBuilder operatorCharacters = new StringBuilder();
            operatorCharacters.Append(Current());
            while (MoveNext())
            {
                if (IsOperator())
                {
                    operatorCharacters.Append(Current());
                }
                else
                {
                    break;
                }
            }
            Operator @operator = new Operator();
            @operator.Characters = operatorCharacters.ToString();
            lexemes.Add(@operator);
        }
    }
}
