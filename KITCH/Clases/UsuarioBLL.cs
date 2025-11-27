using System;
using KITCH.CapaDatos;
using Npgsql;
using System.Security.Cryptography; // Para Hashing (SHA256)
using System.Text; // Para codificación de bytes
using System.Data;
using KITCH.Modelos;

namespace KITCH.CapaNegocio
{
    public class UsuarioBLL
    {
        private UsuarioDAL _usuarioDAL = new UsuarioDAL();

        // ----------------------------------------------------------------------------------
        // --- MÉTODO AGREGADO: PRUEBA DE CONEXIÓN ---
        // ----------------------------------------------------------------------------------
        /// <summary>
        /// Prueba la conexión a la base de datos delegando la tarea al DAL.
        /// </summary>
        public string ProbarConexionDB()
        {
            // Llama al método del DAL para ejecutar la prueba de conexión real.
            // Se asume que UsuarioDAL.ProbarConexion() devuelve un string de éxito o error.
            return _usuarioDAL.ProbarConexion();
        }


        /// <summary>
        /// Valida los datos y registra un nuevo usuario en el sistema.
        /// </summary>
        /// <returns>Un mensaje de éxito o el detalle del error.</returns>
        public string RegistrarNuevoUsuario(string email, string contrasena, string nombreUsuario, string nombre, string apellido, string rol, string telefono, string numeroIdentificacion)
        {
            // La validación de longitud/formato (ej. email) también debe ir aquí.
            if (contrasena.Length < 6)
            {
                return "Error: La contraseña debe tener al menos 6 caracteres.";
            }

            try
            {
                // 1. PREPARACIÓN DE DATOS (SEGURIDAD CRÍTICA)
                string contrasenaHash = HashearContrasena(contrasena);

                // 2. PERSISTENCIA (Llamada al DAL)
                if (_usuarioDAL.RegistrarUsuario(email, contrasenaHash, nombreUsuario, nombre, apellido, rol, telefono, numeroIdentificacion))
                {
                    return "Éxito: Usuario registrado correctamente.";
                }
                else
                {
                    // Esto ocurre si el DAL no lanza una excepción pero devuelve false (ej. filas afectadas = 0)
                    return "Error: No se pudo completar la operación de registro. Ninguna fila fue afectada.";
                }
            }
            catch (Npgsql.PostgresException pgEx)
            {
                // Manejo y traducción de errores comunes de PostgreSQL:

                // Código 23505 = Violación de restricción UNIQUE (Duplicidad)
                if (pgEx.SqlState == "23505")
                {
                    // Devolvemos un mensaje amigable al usuario, no el error técnico.
                    return "Error de Duplicidad: El email, nombre de usuario o identificación ya existe. Por favor, verifique los datos.";
                }
                // Código 23502 = Violación de NOT NULL (Campo obligatorio vacío, aunque ya se validó en el UI)
                else if (pgEx.SqlState == "23502")
                {
                    // pgEx.ColumnName contiene el nombre de la columna que falló.
                    return "Error de BD: El campo '" + pgEx.ColumnName + "' es obligatorio y está vacío.";
                }
                // Manejar otros errores de PostgreSQL
                else
                {
                    return "Error de Base de Datos (código " + pgEx.SqlState + "): " + pgEx.Detail;
                }
            }
            catch (Exception ex)
            {
                // Captura cualquier otro error (ej. conexión fallida)
                return "Error inesperado del sistema: " + ex.Message;
            }
        }

        // ----------------------------------------------------------------------------------
        // --- MÉTODO DE HASHING (SEGURIDAD) ---
        // ----------------------------------------------------------------------------------
        /// <summary>
        /// Convierte una cadena (contraseña) a un hash SHA256. (Usar librerías más fuertes como BCrypt para producción).
        /// </summary>
        private string HashearContrasena(string contrasena)
        {
            // ⚠️ ADVERTENCIA DE SEGURIDAD: Este método está devolviendo la contraseña en texto plano,
            // lo cual es inseguro. Debería implementarse el hashing correctamente.
            return contrasena;
        }

        public DataTable IniciarSesion(string nombreUsuario, string contrasena)
        {
            // ⚠️ Si el Hashing estuviera activo:
            // 1. Hashear la contraseña ingresada: string contrasenaHasheada = HashearContrasena(contrasena);
            // 2. Llamar al DAL con el hash: return _usuarioDAL.AutenticarUsuario(nombreUsuario, contrasenaHasheada);

            try
            {
                // Como el Hashing está DESACTIVADO, enviamos la contraseña en texto plano al DAL.
                DataTable resultado = _usuarioDAL.AutenticarUsuario(nombreUsuario, contrasena);

                if (resultado != null && resultado.Rows.Count == 1)
                {
                    return resultado; // Éxito: Usuario encontrado.
                }
                else
                {
                    return null; // Fracaso: No se encontró el usuario o la contraseña es incorrecta.
                }
            }
            catch (Exception ex)
            {
                // Podrías devolver un mensaje específico en lugar de relanzar la excepción.
                throw new Exception("Error en la capa de negocio al iniciar sesión: " + ex.Message);
            }
        }

        // ----------------------------------------------------------------------------------
        // ✅ MÉTODO IMPLEMENTADO: Devolver objeto Usuario
        // ----------------------------------------------------------------------------------
        /// <summary>
        /// Método auxiliar para iniciar sesión y devolver un objeto Usuario completo.
        /// Se usa tras un registro exitoso o en el formulario de inicio de sesión.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario.</param>
        /// <param name="contrasena">Contraseña sin hashear.</param>
        /// <returns>Objeto Usuario si el inicio de sesión es exitoso, null en caso contrario.</returns>
        internal Usuario InicioSesionUsuario(string nombreUsuario, string contrasena)
        {
            // Reutilizamos el método existente que verifica credenciales y devuelve el DataTable
            DataTable dt = IniciarSesion(nombreUsuario, contrasena);

            if (dt != null && dt.Rows.Count == 1)
            {
                DataRow row = dt.Rows[0];

                // Mapear la fila de la tabla a un objeto Usuario
                Usuario usuario = new Usuario
                {
                    // Asegúrate de que los nombres de las columnas coincidan exactamente con
                    // los nombres de las columnas devueltas por el método AutenticarUsuario del DAL.
                    IdUsuario = Convert.ToInt32(row["UsuarioID"]),
                    Nombre = row["Nombre"].ToString(),
                    Apellido = row["Apellido"].ToString(),
                    Email = row["Email"].ToString(),
                    Rol = row["Rol"].ToString(),
                    // Manejar el campo Telefono si puede ser DBNull
                    // Si tienes más propiedades en tu modelo Usuario, añádelas aquí.
                };

                return usuario;
            }

            return null; // Fallo al autenticar o al obtener datos.
        }
    }
}