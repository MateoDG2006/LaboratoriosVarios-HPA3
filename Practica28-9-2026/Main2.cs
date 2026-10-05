using System;

namespace Practica28_9_2026
{
    internal class Main2
    {
        static void Main(string[] args)
        {
            bool salir = false;

            while (!salir)
            {
                Console.WriteLine();
                Console.WriteLine("========================================");
                Console.WriteLine("   PRACTICA 28-09-2026 - MENU PRINCIPAL");
                Console.WriteLine("========================================");
                Console.WriteLine(" 1. Metodos Sobrecargados y Recursividad");
                Console.WriteLine(" 2. Consultas Preparadas");
                Console.WriteLine(" 3. Inyeccion SQL");
                Console.WriteLine(" 4. Diccionarios y Listas");
                Console.WriteLine(" 5. Frecuencias");
                Console.WriteLine(" 0. Salir");
                Console.WriteLine("========================================");
                Console.Write("Seleccione una opcion: ");

                string? opcion = Console.ReadLine();
                Console.WriteLine();

                switch (opcion)
                {
                    case "1":
                        new MetodosSobrecargados().Ejecutar();
                        break;
                    case "2":
                        new ConsultasPreparadas().Ejecutar();
                        break;
                    case "3":
                        new InyeccionSQL().Ejecutar();
                        break;
                    case "4":
                        new DiccionariosListas().Ejecutar();
                        break;
                    case "5":
                        new Frecuencias().Ejecutar();
                        break;
                    case "0":
                        salir = true;
                        Console.WriteLine("Saliendo del programa...");
                        break;
                    default:
                        Console.WriteLine("Opcion no valida. Intente de nuevo.");
                        break;
                }

                if (!salir)
                {
                    Console.WriteLine();
                    Console.WriteLine("Presione ENTER para volver al menu...");
                    Console.ReadLine();
                }
            }
        }
    }
}
