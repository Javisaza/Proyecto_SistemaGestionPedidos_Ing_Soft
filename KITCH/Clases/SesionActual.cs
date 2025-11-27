// KITCH.Globales/SesionActual.cs

using System;

namespace KITCH.Globales
{
    public static class SesionActual
    {
        // El ID del usuario que ha iniciado sesión
        public static int IdUsuario { get; set; }

        // CLAVE: El ID del restaurante seleccionado, usado para filtrar todos los datos.
        public static int IdRestaurante { get; set; }

        public static string NombreRestaurante { get; set; }

        // El ID de la fila en la tabla 'sesiones_trabajo' (0 si no se registró en BD)
        public static int IdSesionTrabajo { get; set; }

        /// <summary>
        /// Carga los datos de la sesión actual (se llama al seleccionar un restaurante).
        /// </summary>
        public static void CargarSesion(int idUsuario, int idRestaurante, string nombreRestaurante, int idSesion)
        {
            IdUsuario = idUsuario;
            IdRestaurante = idRestaurante;
            NombreRestaurante = nombreRestaurante;
            IdSesionTrabajo = idSesion;
        }

        /// <summary>
        /// Limpia los datos de la sesión al cerrar.
        /// </summary>
        public static void CerrarSesion()
        {
            IdUsuario = 0;
            IdRestaurante = 0;
            NombreRestaurante = string.Empty;
            IdSesionTrabajo = 0;
        }
    }
}