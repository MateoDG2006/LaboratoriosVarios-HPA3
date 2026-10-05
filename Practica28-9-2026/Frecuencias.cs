using System;
using System.Collections.Generic;
using System.Text;

namespace Practica28_9_2026
{
    internal class Frecuencias
    {
        public void Ejecutar()
        {
            int[] frecuencias = new int[6];
            Console.WriteLine("=== FRECUENCIAS ===");
            Console.WriteLine();
            for(int i = 0; i < 6000; i++)
            {
                // Genera un número aleatorio entre 1 y 6.
                int tirada = new Random().Next(1, 7);

                // Incrementa la frecuencia correspondiente.
                frecuencias[tirada - 1]++; 
            }
            // Mostrar los resultados.
            Console.WriteLine("Cara \t Frecuencia");
            Console.WriteLine("1\t{0}\n2\t{1}\n3\t{2}\n4\t{3}\n5\t{4}\n6\t{5}",
           frecuencias[0], frecuencias[1], frecuencias[2], frecuencias[3], frecuencias[4], frecuencias[5]);
        }
    }
}
