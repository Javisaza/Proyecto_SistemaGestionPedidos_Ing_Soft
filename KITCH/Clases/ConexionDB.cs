// Archivo: KITCH/ConexionDB.cs

using Npgsql;
using System; // Agregado para el manejo de excepciones (si se usa ProbarConexion)

namespace KITCH
{
    public class ConexionDB
    {
        // Cadena de conexión proporcionada por el usuario
        private readonly string connectionString =
            "Host=localhost;Port=5432;Username=postgres;Password=123456;Database=kitchdb;";

        /// <summary>
        /// Crea y retorna una nueva conexión Npgsql. La conexión no está abierta.
        /// </summary>
        /// <returns>Objeto NpgsqlConnection.</returns>
        public NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connectionString);
        }

        /// <summary>
        /// Intenta abrir y cerrar una conexión para verificar su estado.
        /// </summary>
        /// <returns>True si la conexión fue exitosa.</returns>
        public bool ProbarConexion()
        {
            try
            {
                using (NpgsqlConnection conn = GetConnection())
                {
                    conn.Open();
                    conn.Close();
                    // Si llega aquí sin excepción, la conexión funciona.
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Aquí podrías loggear el error o mostrar un mensaje específico.
                Console.WriteLine("Error de conexión: " + ex.Message);
                return false;
            }
        }
    }
}