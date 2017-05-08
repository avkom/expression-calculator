using System;
using System.IO;
using Calculator;
using System.Windows.Forms;

namespace CalculatorTests
{
    class Program
    {
        readonly Calculator.Calculator calculator = new Calculator.Calculator();

        [STAThreadAttribute]
        static void Main()
        {
            Program program = new Program();
            program.Run();
        }

        private void Run()
        {
            AddBinaryOperators();
            AddFunctions();
            while (true)
            {
                string expression = Console.ReadLine();
                object result = null;
                try
                {
                    result = calculator.Evaluate(expression);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
                Console.WriteLine(result);
            }
        }

        private void AddBinaryOperators()
        {
            calculator.BinaryOperators.Add(new BinaryOperator("=", 1, Associativity.RightToLeft, BinaryOperator.CreateAssingnmentHandler<object>((a, b) => b)));
            calculator.BinaryOperators.Add(new BinaryOperator("+", 2, Associativity.LeftToRight, BinaryOperator.CreateHandler<decimal>((a, b) => a + b)));
            calculator.BinaryOperators.Add(new BinaryOperator("*", 3, Associativity.LeftToRight, BinaryOperator.CreateHandler<decimal>((a, b) => a * b)));
            calculator.BinaryOperators.Add(new BinaryOperator("^", 4, Associativity.RightToLeft, BinaryOperator.CreateHandler<decimal>((x, y) => (decimal) Math.Pow((double) x, (double) y))));
        }

        private void AddFunctions()
        {
            calculator.Functions.Add(new Function("Hello", new Action(Hello)));
            calculator.Functions.Add(new Function("Sin", new Func<decimal, decimal>(Sin)));
            calculator.Functions.Add(new Function("Power", new Func<decimal, decimal, decimal>(Power)));
            calculator.Functions.Add(new Function("RunScript", new Action(RunScript)));
            calculator.Functions.Add(new Function("Exit", new Action(Exit)));
        }

        private void Hello()
        {
            Console.WriteLine("Hello, World!");
        }

        private decimal Sin(decimal x)
        {
            return (decimal) Math.Sin((double) x);
        }

        private decimal Power(decimal x, decimal y)
        {
            return (decimal) Math.Pow((double) x, (double) y);
        }

        private void RunScript()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string filename = dialog.FileName;
            using (StreamReader streamReader = new StreamReader(filename))
            {
                while (!streamReader.EndOfStream)
                {
                    string expression = streamReader.ReadLine();
                    object result = null;
                    try
                    {
                        result = calculator.Evaluate(expression);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message);
                    }
                    Console.WriteLine(result);
                }
            }
        }

        private void Exit()
        {
            Environment.Exit(0);
        }    
    }
}
