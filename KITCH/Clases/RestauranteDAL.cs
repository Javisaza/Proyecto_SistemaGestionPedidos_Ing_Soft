using Npgsql;
using System;
using System.Data;
using System.Text; // Necesario para StringBuilder

namespace KITCH.CapaDatos
{
    public class RestauranteDAL
    {
        // Asumiendo que esta clase existe y funciona correctamente
        private ConexionDB _conexionDB = new ConexionDB();

        /// <summary>
        /// Registra un nuevo restaurante en la base de datos y devuelve su ID.
        /// </summary>
        /// <param name="nombre">Nombre del restaurante.</param>
        /// <param name="direccion">Dirección del restaurante (opcional).</param>
        /// <param name="telefono">Teléfono del restaurante (opcional).</param>
        /// <param name="idAdminPrincipal">ID del usuario que lo crea (Administrador Principal).</param>
        /// <returns>El ID del restaurante recién creado, o 0 si falla.</returns>
        public int CrearRestaurante(string nombre, int idAdminPrincipal, int nummesas)
        {
            // Columnas según tu DDL: nombre_restaurante, direccion, telefono, id_admin_usuario, fecha_registro
            string sql = @"
                INSERT INTO Restaurantes (nombre_restaurante, id_admin_usuario, fecha_registro, numeromesas)
                VALUES (@nombre_restaurante, @idAdmin, CURRENT_TIMESTAMP, @numeromesas)
                RETURNING id_restaurante;"; // Usamos RETURNING para obtener el ID

            using (NpgsqlConnection conn = _conexionDB.GetConnection())
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre_restaurante", nombre);
                    // Manejar valores nulos o vacíos para campos opcionales (DBNull si están vacíos)
                    cmd.Parameters.AddWithValue("@numeromesas", nummesas); // Valor por defecto, se actualizará luego
                    cmd.Parameters.AddWithValue("@idAdmin", idAdminPrincipal);

                    // Ejecutar el comando y obtener el ID devuelto
                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null && int.TryParse(resultado.ToString(), out int idRestaurante))
                    {
                        return idRestaurante; // Éxito
                    }
                    return 0; // Fallo al crear
                }
            }
        }

        /// <summary>
        /// Crea un número específico de mesas iniciales para un restaurante.
        /// Se debe llamar después de CrearRestaurante.
        /// </summary>
        /// <param name="idRestaurante">ID del restaurante recién creado.</param>
        /// <param name="numeroMesas">Cantidad de mesas a crear.</param>
        /// <param name="capacidadMesa">Capacidad por defecto para cada mesa.</param>
        /// <returns>True si la creación de mesas fue exitosa.</returns>
        public bool CrearMesasIniciales(int idRestaurante, int numeroMesas)
        {
            if (numeroMesas <= 0) return true; // No hay mesas para crear

            // Columnas según tu DDL: id_restaurante, numero_mesa, capacidad, estado_mesa
            // Usamos un solo comando con múltiples inserts para eficiencia.
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine("INSERT INTO Mesas (id_restaurante, numero_mesa, estado_mesa) VALUES ");

            for (int i = 1; i <= numeroMesas; i++)
            {
                // Generamos un número de mesa simple, por ejemplo 'Mesa 1'
                sqlBuilder.Append($"(@idRestaurante, @numMesa{i}, 'Libre')");
                if (i < numeroMesas)
                {
                    sqlBuilder.Append(", ");
                }
            }
            sqlBuilder.Append(";");

            using (NpgsqlConnection conn = _conexionDB.GetConnection())
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(sqlBuilder.ToString(), conn))
                {
                    cmd.Parameters.AddWithValue("@idRestaurante", idRestaurante);

                    // Añadir parámetros dinámicos para el nombre de la mesa
                    for (int i = 1; i <= numeroMesas; i++)
                    {
                        cmd.Parameters.AddWithValue($"@numMesa{i}", $"Mesa {i}");
                    }

                    int filasAfectadas = cmd.ExecuteNonQuery();
                    // Verificar si se crearon tantas mesas como se pidieron
                    return filasAfectadas == numeroMesas;
                }
            }
        }

        // The issue is that '_mesaDAL' is referenced but not defined in the provided code. 
        // To fix this, we need to define '_mesaDAL' and ensure it has the method 'ObtenerMesasPorRestaurante' implemented.
        // Below is the fix assuming '_mesaDAL' is an instance of a class that handles operations related to 'Mesas'.

        // Add this field to the RestauranteDAL class
        private MesaDAL _mesaDAL = new MesaDAL();

        // Ensure the MesaDAL class has the method 'ObtenerMesasPorRestaurante' implemented.
        // Here is a possible implementation for the MesaDAL class:

        public class MesaDAL
        {
            private ConexionDB _conexionDB = new ConexionDB();

            /// <summary>
            /// Obtiene las mesas de un restaurante específico.
            /// </summary>
            /// <param name="idRestaurante">ID del restaurante.</param>
            /// <returns>DataTable con las mesas del restaurante.</returns>
            public DataTable ObtenerMesasPorRestaurante(int idRestaurante)
            {
                string sql = @"SELECT id_mesa, numero_mesa, estado_mesa 
                               FROM Mesas 
                               WHERE id_restaurante = @idRestaurante
                               ORDER BY numero_mesa;";

                using (NpgsqlConnection conn = _conexionDB.GetConnection())
                {
                    conn.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@idRestaurante", idRestaurante);

                        NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        // Aquí iría el método para unir un usuario a un restaurante (Join)
    }
}