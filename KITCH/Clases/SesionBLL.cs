// KITCH.CapaNegocio/SesionBLL.cs

using KITCH.CapaDatos;
using System;

namespace KITCH.CapaNegocio
{
    public class SesionBLL
    {
        private SesionDAL _sesionDAL = new SesionDAL();

        public int IniciarSesion(int idUsuario, int idRestaurante)
        {
            // Aquí se pueden agregar validaciones de negocio antes de llamar al DAL
            return _sesionDAL.IniciarSesionTrabajo(idUsuario, idRestaurante);
        }

        public bool CerrarSesion(int idSesionTrabajo)
        {
            if (idSesionTrabajo <= 0) return true; // No hay sesión de BD que cerrar
            return _sesionDAL.CerrarSesionTrabajo(idSesionTrabajo);
        }
    }
}