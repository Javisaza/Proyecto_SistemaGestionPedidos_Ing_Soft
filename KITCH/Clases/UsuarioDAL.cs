using System;
using Npgsql;
using KITCH.CapaDatos; // Asumiendo que ConexionDB está aquí
using System.Data;

namespace KITCH.CapaDatos
{
    public class UsuarioDAL
    {
        // Instancia de la clase de conexión para obtener la conexión
        private ConexionDB _conexionDB = new ConexionDB();

        // ----------------------------------------------------------------------------------
        // --- MÉTODO AGREGADO: PRUEBA DE CONEXIÓN ---
        // ----------------------------------------------------------------------------------
        /// <summary>
        /// Intenta abrir y cerrar la conexión para verificar su estado con PostgreSQL.
        /// Este método es llamado por el BLL para la prueba de inicio del sistema.
        /// </summary>
        public string ProbarConexion()
        {
            // Obtener la conexión usando el objeto de la Capa de Datos
            using (NpgsqlConnection conn = _conexionDB.GetConnection())
            {
                try
                {
                    conn.Open();
                    // Si llega hasta aquí, la conexión fue exitosa. La sentencia 'using' asegura el cierre.
                    return "Éxito: Conexión a la Base de Datos PostgreSQL exitosa.";
                }
                catch (NpgsqlException pgEx)
                {
                    // Captura errores específicos de Npgsql (problemas de cadena, host, credenciales)
                    return "Error de Conexión a la Base de Datos:\n" + pgEx.Message;
                }
                catch (Exception ex)
                {
                    // Captura otros errores genéricos
                    return "Error de Conexión Genérico:\n" + ex.Message;
                }
            }
        }

        // ----------------------------------------------------------------------------------
        // --- MÉTODO EXISTENTE: REGISTRAR USUARIO (Mantiene el flujo de errores al BLL) ---
        // ----------------------------------------------------------------------------------

        /// <summary>
        /// Inserta un nuevo registro de usuario en la tabla Usuarios.
        /// </summary>
        public bool RegistrarUsuario(string email, string contrasenaHash, string nombreUsuario, string nombre, string apellido, string rol, string telefono, string numeroIdentificacion)
        {
            string sql = @"
                INSERT INTO Usuarios (email, contraseña, nombre_de_usuario, nombre, apellido, rol, numero_telefono, numero_identificacion) 
                VALUES (@email, @contrasena, @nombreUsuario, @nombre, @apellido, @rol, @telefono, @numeroIdentificacion)";

            // Uso de 'using' para asegurar el manejo de recursos (cierre de conexión)
            using (NpgsqlConnection conn = _conexionDB.GetConnection())
            {
                // El try/catch se maneja en el BLL para traducir los errores de la DB.
                // Si falla aquí (ej. conexión), el BLL lo capturará como una Exception general.
                conn.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    // Asignación de parámetros (¡Crucial para prevenir Inyección SQL!)
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@contrasena", contrasenaHash);
                    cmd.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@apellido", apellido);
                    cmd.Parameters.AddWithValue("@rol", rol);

                    // Manejo de valores nulos para columnas opcionales (ej. telefono)
                    cmd.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(telefono) ? (object)DBNull.Value : telefono);

                    cmd.Parameters.AddWithValue("@numeroIdentificacion", numeroIdentificacion);

                    int filasAfectadas = cmd.ExecuteNonQuery();
                    return filasAfectadas > 0;
                }
            }
        }
        public DataTable AutenticarUsuario(string nombreUsuario, string contrasena)
        {
            // Buscamos un usuario donde nombre_de_usuario Y contraseña coincidan.
            // Si usáramos hashing, la consulta solo buscaría por nombre_de_usuario,
            // y el BLL haría la verificación del hash. Pero simplificaremos.
            string sql = @"
        SELECT id_usuario, email, nombre, apellido, rol 
        FROM Usuarios 
        WHERE nombre_de_usuario = @nombreUsuario AND contraseña = @contrasena";

            using (NpgsqlConnection conn = _conexionDB.GetConnection())
            {
                try
                {
                    conn.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
                        // Si estás usando texto plano: se compara directamente en la DB.
                        // Si estuvieras usando HASHING, la contraseña YA DEBE llegar hasheada aquí.
                        cmd.Parameters.AddWithValue("@contrasena", contrasena);

                        NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Si dt tiene una fila, el usuario existe y las credenciales son correctas.
                        return dt;
                    }
                }
                catch (Exception ex)
                {
                    // Propagar el error para que el BLL lo maneje y traduzca.
                    throw new Exception("Error al intentar autenticar al usuario: " + ex.Message);
                }
            }
        }
    }
}