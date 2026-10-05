using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Practica28_9_2026
{
    internal class ConsultasPreparadas
    {
        public void Ejecutar()
        {
            Console.WriteLine($"=== CONSULTAS PREPARADAS ({ConexionBD.NombreBaseDatos} / SQL Server) ===");
            Console.WriteLine();

            Console.Write("Cuantos productos de prueba desea insertar? (Enter = 5): ");
            string? entrada = Console.ReadLine();
            if (!int.TryParse(entrada, out int totalRegistros) || totalRegistros <= 0)
                totalRegistros = 5;

            // Inicializacion y conexion.
            SqlConnection conn = ConexionBD.ObtenerConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                // Apertura fisica de la base de datos.
                conn.Open();

                // La consulta SQL y el metodo Prepare(): se define UNA sola vez.
                cmd.CommandText = "INSERT INTO productos (nombre, precio, cantidad, imagen) VALUES (@nombre, @precio, @cantidad, @imagen)";

                // Definicion inicial de parametros con tipo y tamano (requerido por Prepare()).
                cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 100);
                SqlParameter pPrecio = cmd.Parameters.Add("@precio", SqlDbType.Decimal);
                pPrecio.Precision = 10;   // decimal(10,2)
                pPrecio.Scale = 2;
                cmd.Parameters.Add("@cantidad", SqlDbType.Int);
                cmd.Parameters.Add("@imagen", SqlDbType.VarBinary, -1);   // varbinary(max)
                cmd.Prepare();

                Console.WriteLine();
                Console.WriteLine($"Consulta preparada: {cmd.CommandText}");
                Console.WriteLine("Ejecutando inserciones (cmd.ExecuteNonQuery)...");
                Console.WriteLine();

                // El ciclo REUTILIZA la sentencia preparada; solo cambia el Value.
                for (int i = 1; i <= totalRegistros; i++)
                {
                    cmd.Parameters["@nombre"].Value = $"Producto {i}";
                    cmd.Parameters["@precio"].Value = 10.50m * i;
                    cmd.Parameters["@cantidad"].Value = i;
                    cmd.Parameters["@imagen"].Value = System.Text.Encoding.UTF8.GetBytes("sin_imagen");

                    cmd.ExecuteNonQuery();
                    Console.WriteLine($"  Insertado -> nombre: Producto {i}, precio: {10.50m * i}, cantidad: {i}, imagen: (binario)");
                }

                Console.WriteLine();
                Console.WriteLine($"{totalRegistros} producto(s) insertado(s) reutilizando la misma sentencia preparada.");
            }
            catch (SqlException ex)
            {
                // Manejo de errores especifico de SQL Server.
                Console.WriteLine($"Error {ex.Number} de SQL Server: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
