using System;

namespace KITCH.Modelos
{
    /// <summary>
    /// Modelo para almacenar los datos del usuario logueado.
    /// </summary>
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Rol { get; set; }
        public string NombreCompleto => $"{Nombre} {Apellido}";

        /// <summary>
        /// Verifica si el usuario tiene el rol de Administrador Principal.
        /// </summary>
        public bool EsAdministradorPrincipal => Rol == "Administrador Principal";
    }
}