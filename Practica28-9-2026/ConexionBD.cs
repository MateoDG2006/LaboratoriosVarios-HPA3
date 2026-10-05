using Microsoft.Data.SqlClient;

namespace Practica28_9_2026
{
    internal static class ConexionBD
    {
        // Servidor/instancia de SQL Server. Ajusta si tu instancia es distinta (p. ej. "Mateo\\SQLEXPRESS").
        public const string Servidor = "Mateo";

        // Base de datos propia de esta practica (independiente de productosdb).
        // Se crea con BaseDatos/01_CrearBaseDatos.sql y se reinicia con BaseDatos/02_ReiniciarBaseDatos.sql.
        public const string NombreBaseDatos = "practica28_productosdb";

        // Cadena de conexion construida con SqlConnectionStringBuilder (evita errores de formato).
        public static string CadenaConexion => new SqlConnectionStringBuilder
        {
            DataSource = Servidor,
            InitialCatalog = NombreBaseDatos,
            IntegratedSecurity = true,
            Encrypt = true,
            TrustServerCertificate = true
        }.ConnectionString;

        // Devuelve una nueva conexion lista para abrir con conn.Open().
        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(CadenaConexion);
        }
    }
}
