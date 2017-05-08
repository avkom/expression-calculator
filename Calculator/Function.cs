using System;

namespace Calculator
{
    public class Function
    {
        public string Name { get; set; }
        public Delegate Handler { get; set; }

        public Function()
        {
        }

        public Function(string name, Delegate handler)
        {
            Name = name;
            Handler = handler;
        }
    }
}
