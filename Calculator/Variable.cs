using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calculator
{
    public class Variable
    {
        public object Value { get; set; }

        public override string ToString()
        {
            if (Value != null)
            {
                return Value.ToString();
            }
            return "Variable value: null";
        }
    }
}
