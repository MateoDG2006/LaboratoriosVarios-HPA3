using System;
using Microsoft.Data.SqlClient;

namespace Practica28_9_2026
{
    internal class InyeccionSQL
    {
        public void Ejecutar()
        {
            Console.WriteLine($"=== INYECCION SQL (OWASP A05) - {ConexionBD.NombreBaseDatos} / SQL Server ===");
            Console.WriteLine();
            Console.WriteLine("Busque un producto por nombre. Pruebe primero un nombre real");
            Console.WriteLine("y luego un payload de inyeccion, por ejemplo:");
            Console.WriteLine("   ' OR '1'='1");
            Console.WriteLine();

            Console.Write("Nombre del producto a buscar: ");
            string nombre = Console.ReadLine() ?? "";

            Console.WriteLine();
            Console.WriteLine("----------------------------------------");
            Console.WriteLine(" A) FORMA VULNERABLE (concatenacion)");
            Console.WriteLine("----------------------------------------");
            BuscarVulnerable(nombre);

            Console.WriteLine();
            Console.WriteLine("----------------------------------------");
            Console.WriteLine(" B) FORMA SEGURA (parametrizada)");
            Console.WriteLine("----------------------------------------");
            BuscarSeguro(nombre);
        }

        // Concatena directamente la entrada del usuario en la consulta -> VULNERABLE.
        private void BuscarVulnerable(string nombre)
        {
            string query = "SELECT id, nombre, precio, cantidad FROM productos WHERE nombre = '" + nombre + "'";
            Console.WriteLine("Consulta generada:");
            Console.WriteLine("  " + query);
            Console.WriteLine();

            SqlConnection conn = ConexionBD.ObtenerConexion();
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                MostrarResultados(cmd);
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error {ex.Number} de SQL Server: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        // Usa parametros -> la entrada se trata como dato literal (SEGURO).
        private void BuscarSeguro(string nombre)
        {
            string query = "SELECT id, nombre, precio, cantidad FROM productos WHERE nombre = @nombre";
            Console.WriteLine("Consulta parametrizada:");
            Console.WriteLine("  " + query);
            Console.WriteLine($"  @nombre = \"{nombre}\"   (valor literal)");
            Console.WriteLine();

            SqlConnection conn = ConexionBD.ObtenerConexion();
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                MostrarResultados(cmd);
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error {ex.Number} de SQL Server: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        // Ejecuta el comando y muestra las filas devueltas.
        private void MostrarResultados(SqlCommand cmd)
        {
            int filas = 0;
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    filas++;
                    Console.WriteLine($"     - [{reader["id"]}] {reader["nombre"]} | precio: {reader["precio"]} | cantidad: {reader["cantidad"]}");
                }
            }

            Console.WriteLine($"  Filas devueltas: {filas}");
            if (filas > 1)
                Console.WriteLine("  [!] Se devolvieron TODOS los registros: la inyeccion tuvo exito.");
            else if (filas == 0)
                Console.WriteLine("  Sin coincidencias (el payload se trato como texto literal o no existe el producto).");
        }
    }
}
