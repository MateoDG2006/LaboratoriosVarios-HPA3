using System;
using System.Collections.Generic;

namespace Practica28_9_2026
{
    // Ejemplo 4: Diccionarios y Listas.
    // Basado en los ejemplos de la Ing. Irina Fong (.Keys, foreach, List, string.Join).
    internal class DiccionariosListas
    {
        public void Ejecutar()
        {
            Console.WriteLine("=== DICCIONARIOS Y LISTAS ===");
            Console.WriteLine();

            BuscarPrecio();

            Console.WriteLine();
            GenerarConsultaDinamica();
        }

        // Busqueda de un valor por su clave con TryGetValue.
        private void BuscarPrecio()
        {
            var preciosProductos = new Dictionary<string, decimal>
            {
                { "Manzanas", 1.5m },
                { "Bananas", 0.8m },
                { "Naranjas", 1.2m },
                { "Peras", 1.3m }
            };

            // .Keys + string.Join para listar las claves disponibles.
            Console.WriteLine("Productos disponibles: " + string.Join(", ", preciosProductos.Keys));
            Console.Write("Ingrese el nombre del producto para obtener su precio: ");
            string producto = Console.ReadLine() ?? "";

            if (preciosProductos.TryGetValue(producto, out decimal precio))
                Console.WriteLine($"El precio de {producto} es: {precio:C}");
            else
                Console.WriteLine($"El producto {producto} no se encuentra en el diccionario.");
        }

        // Uso de .Keys, foreach, List<string> y string.Join para armar SQL dinamico.
        private void GenerarConsultaDinamica()
        {
            Console.WriteLine("--- Generacion dinamica de SQL a partir de un diccionario ---");

            Dictionary<string, object> datosInventario = new Dictionary<string, object>
            {
                { "Nombre", "Laptop HP Envy" },
                { "Precio", 850.99m },
                { "Cantidad", 15 }
            };

            // Clausula SET: columna = @columna
            var setParts = new List<string>();
            foreach (var key in datosInventario.Keys)
            {
                setParts.Add($"{key} = @{key}");
            }
            string setClause = string.Join(", ", setParts);
            Console.WriteLine($"Clausula SET generada: {setClause}");

            // INSERT: columnas y marcadores de posicion.
            var columns = string.Join(", ", datosInventario.Keys);
            var placeholders = "@" + string.Join(", @", datosInventario.Keys);
            string sql = $"INSERT INTO productos ({columns}) VALUES ({placeholders})";
            Console.WriteLine($"La cadena SQL es: {sql}");
        }
    }
}
