using System;

namespace Practica28_9_2026
{
    internal class MetodosSobrecargados
    {
        // Punto de entrada del ejemplo (invocado desde el menu).
        public void Ejecutar()
        {
            Console.WriteLine("=== SOBRECARGA DE METODOS ===");
            ProbarMetodosSobreCargados();

            Console.WriteLine();
            Console.Write("Ingrese un numero entero para elevar al cuadrado: ");
            if (int.TryParse(Console.ReadLine(), out int valor))
                Console.WriteLine($"El cuadrado de {valor} es {Cuadrado(valor)}");
            else
                Console.WriteLine("Entrada no valida.");

            Console.WriteLine();
            Console.WriteLine("=== RECURSIVIDAD: FACTORIAL ===");
            Console.WriteLine("Calculo del factorial del 0 al 10:");
            for (long contador = 0; contador <= 10; contador++)
                Console.WriteLine($"{contador}! = {Factorial(contador)}");

            Console.WriteLine();
            Console.WriteLine("=== RECURSIVIDAD: FIBONACCI ===");
            Fibonacci fibonacci = new Fibonacci();
            Console.Write("Ingrese el numero de iteraciones Fibonacci: ");
            if (int.TryParse(Console.ReadLine(), out int n) && n > 0)
                Console.WriteLine($"La iteracion {n} de Fibonacci es: {fibonacci.CalcularFibonacci(n)}");
            else
                Console.WriteLine("Debe ingresar un numero entero mayor que cero.");
        }

        // Prueba los metodos Cuadrado sobrecargados.
        public void ProbarMetodosSobreCargados()
        {
            Console.WriteLine("El cuadrado del integer 7 es {0}", Cuadrado(7));
            Console.WriteLine("El cuadrado del double 7.5 es {0}", Cuadrado(7.5));
        }

        // Sobrecarga: mismo nombre, distinta firma (int).
        public int Cuadrado(int valorInt)
        {
            Console.WriteLine("Se llamo a Cuadrado con argumento int: {0}", valorInt);
            return valorInt * valorInt;
        }

        // Sobrecarga: mismo nombre, distinta firma (double).
        public double Cuadrado(double valorDouble)
        {
            Console.WriteLine("Se llamo a Cuadrado con argumento double: {0}", valorDouble);
            return valorDouble * valorDouble;
        }

        // Declaracion recursiva del metodo Factorial.
        public long Factorial(long numero)
        {
            if (numero <= 1)          // caso base
                return 1;
            return numero * Factorial(numero - 1); // paso de recursividad
        }
    }
}
