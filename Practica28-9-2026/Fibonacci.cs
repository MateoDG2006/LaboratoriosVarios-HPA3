using System;
using System.Collections.Generic;
using System.Text;

namespace Practica28_9_2026
{

    internal class Fibonacci
    {
        public int CalcularFibonacci(int n)
        {
            if (n <= 0)
            {
                throw new ArgumentException("El número debe ser mayor que cero.");
            }
            else if (n == 1 || n == 2)
            {
                return 1;
            }
            else
            {
                return CalcularFibonacci(n - 1) + CalcularFibonacci(n - 2);
            }
        }
    }
}
