using System;
using Npgsql;
using KITCH.CapaDatos;
using System.Data;

namespace KITCH.CapaDatos
{
    public class UsuarioDAL
    {
        // Instancia de la clase de conexión para obtener la conexión
        private ConexionDB _conexionDB = new ConexionDB();

        // ----------------------------------------------------------------------------------
        // --- MÉTODO: PRUEBA DE CONEXIÓN ---
        // ----------------------------------------------------------------------------------
        public string ProbarConexion()
        {
            using (NpgsqlConnection conn = _conexionDB.GetConnection())
            {
                try
                {
                    conn.Open();
                    return "Éxito: Conexión a la Base de Datos PostgreSQL exitosa.";
                }
                catch (NpgsqlException pgEx)
                {
                    return "Error de Conexión a la Base de Datos:\n" + pgEx.Message;
                }
                catch (Exception ex)
                {
                    return "Error de Conexión Genérico:\n" + ex.Message;
                }
            }
        }

        // ----------------------------------------------------------------------------------
        // --- MÉTODO: REGISTRAR USUARIO ---
        // ----------------------------------------------------------------------------------
        public bool RegistrarUsuario(string email, string contrasenaHash, string nombreUsuario, string nombre, string apellido, string rol, string telefono, string numeroIdentificacion)
        {
            // Asumo que tu tabla Usuarios tiene la columna 'id_restaurante' con un valor por defecto o se gestiona en el registro.
            // Si la columna id_restaurante es obligatoria, debe incluirse aquí.
            string sql = @"
                INSERT INTO Usuarios (email, contraseña, nombre_de_usuario, nombre, apellido, rol, numero_telefono, numero_identificacion) 
                VALUES (@email, @contrasena, @nombreUsuario, @nombre, @apellido, @rol, @telefono, @numeroIdentificacion)";

            using (NpgsqlConnection conn = _conexionDB.GetConnection())
            {
                conn.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@contrasena", contrasenaHash);
                    cmd.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@apellido", apellido);
                    cmd.Parameters.AddWithValue("@rol", rol);
                    cmd.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(telefono) ? (object)DBNull.Value : telefono);
                    cmd.Parameters.AddWithValue("@numeroIdentificacion", numeroIdentificacion);

                    int filasAfectadas = cmd.ExecuteNonQuery();
                    return filasAfectadas > 0;
                }
            }
        }

        // ----------------------------------------------------------------------------------
        // --- MÉTODO CORREGIDO: AUTENTICAR USUARIO (Incluye id_restaurante) ---
        // ----------------------------------------------------------------------------------
        public DataTable AutenticarUsuario(string nombreUsuario, string contrasena)
        {
            // 🔑 CORRECCIÓN: Agregamos 'id_restaurante' al SELECT para que la capa de negocio pueda leerlo
            string sql = @"
    SELECT id_usuario, email, nombre, apellido, rol, id_restaurante 
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
                        cmd.Parameters.AddWithValue("@contrasena", contrasena); // Asumiendo texto plano o hash enviado desde BLL

                        NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        return dt;
                    }
                }
                catch (Exception ex)
                {
                    // Propagar el error para que el BLL lo maneje.
                    throw new Exception("Error al intentar autenticar al usuario: " + ex.Message);
                }
            }
        }
    }
}