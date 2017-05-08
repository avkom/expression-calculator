using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calculator
{
    class AutomationStack<T> : List<T>
    {
        public void Push(T item)
        {
            Add(item);
        }

        public T Peek()
        {
            return this[Count - 1];
        }

        public T Pop()
        {
            T item = this[Count - 1];
            RemoveAt(Count - 1);
            return item;
        }
    }
}
