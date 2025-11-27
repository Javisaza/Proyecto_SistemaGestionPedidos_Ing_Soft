// KITCH.CapaDatos/SesionDAL.cs

using Npgsql;
using System;

namespace KITCH.CapaDatos
{
    public class SesionDAL
    {
        private ConexionDB _conexionDB = new ConexionDB();

        /// <summary>
        /// Registra el inicio de una sesión de trabajo y devuelve el ID de la fila insertada.
        /// </summary>
        /// <param name="idUsuario">ID del usuario (camarero).</param>
        /// <param name="idRestaurante">ID del restaurante seleccionado.</param>
        /// <returns>El id_sesiones_trabajo si es exitoso, o 0 si falla.</returns>
        public int IniciarSesionTrabajo(int idUsuario, int idRestaurante)
        {
            string sql = @"
                INSERT INTO public.sesiones_trabajo 
                    (id_usuario, id_restaurante, hora_inicio)
                VALUES 
                    (@id_usuario, @id_restaurante, CURRENT_TIMESTAMP)
                RETURNING id_sesiones_trabajo;"; // CLAVE: obtener el ID generado

            try
            {
                using (NpgsqlConnection conn = _conexionDB.GetConnection())
                {
                    conn.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                        cmd.Parameters.AddWithValue("@id_restaurante", idRestaurante);

                        object resultado = cmd.ExecuteScalar();

                        if (resultado != null && int.TryParse(resultado.ToString(), out int idSesion))
                        {
                            return idSesion;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine($"Error en DAL al iniciar sesión: {ex.Message}");
            }
            return 0; // Fallo
        }

        /// <summary>
        /// Registra la hora_fin de una sesión de trabajo.
        /// </summary>
        public bool CerrarSesionTrabajo(int idSesionTrabajo)
        {
            string sql = @"
                UPDATE public.sesiones_trabajo 
                SET hora_fin = CURRENT_TIMESTAMP
                WHERE id_sesiones_trabajo = @id_sesion;";

            try
            {
                using (NpgsqlConnection conn = _conexionDB.GetConnection())
                {
                    conn.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_sesion", idSesionTrabajo);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DAL al cerrar sesión: {ex.Message}");
                return false;
            }
        }
    }
}