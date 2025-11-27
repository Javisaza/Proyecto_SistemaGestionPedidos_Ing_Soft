using System;
using System.Data;
using KITCH.CapaDatos;
using Npgsql;
// using KITCH.Modelos; // Descomentar si tienes un modelo Restaurante

namespace KITCH.CapaNegocio
{
    public class RestauranteBLL
    {
        private RestauranteDAL _restauranteDAL = new RestauranteDAL();
        private SesionDAL _sesionDAL = new SesionDAL();

        // Agregar una instancia de conexión a la base de datos
        private ConexionDB _conexionDB = new ConexionDB();

        public int IniciarSesion(int idUsuario, int idRestaurante)
        {
            return _sesionDAL.IniciarSesionTrabajo(idUsuario, idRestaurante);
        }

        public bool CerrarSesion(int idSesionTrabajo)
        {
            return _sesionDAL.CerrarSesionTrabajo(idSesionTrabajo);
        }
        /// <summary>
        /// Crea un nuevo restaurante y sus mesas iniciales, orquestando las llamadas al DAL.
        /// </summary>
        /// <param name="nombre">Nombre del restaurante (Obligatorio).</param>
        /// <param name="direccion">Dirección del restaurante (Opcional).</param>
        /// <param name="telefono">Teléfono del restaurante (Opcional).</param>
        /// <param name="idAdminPrincipal">ID del usuario que crea el restaurante.</param>
        /// <param name="numeroMesas">Cantidad de mesas a crear.</param>
        /// <param name="capacidadMesa">Capacidad por defecto de cada mesa.</param>
        /// <returns>Mensaje de éxito o detalle del error.</returns>
        // KITCH.Globales/SesionActual.cs

        public static class SesionActual
        {
            // Almacena el ID del usuario logueado
            public static int IdUsuario { get; set; }

            // CLAVE PARA EL AISLAMIENTO: ID del restaurante seleccionado
            public static int IdRestaurante { get; set; }

            // Opcional: Nombre del restaurante para mostrar en el menú
            public static string NombreRestaurante { get; set; }

            // ID de la fila en 'sesiones_trabajo' para poder cerrarla al salir
            public static int IdSesionTrabajo { get; set; }

            /// <summary>
            /// Limpia todos los datos al cerrar sesión.
            /// </summary>
            public static void CerrarSesion()
            {
                IdUsuario = 0;
                IdRestaurante = 0;
                NombreRestaurante = string.Empty;
                IdSesionTrabajo = 0;
            }
        }

        public string CrearRestaurante(string nombre, int idAdminPrincipal, int numeroMesas, int nummesas)
        {
            // 1. Validaciones básicas de Negocio
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return "Error: El nombre del restaurante es obligatorio.";
            }

            try
            {
                // 2. CREAR EL RESTAURANTE (Llamada al DAL)
                // El DAL ahora devuelve el ID del restaurante o 0 si falla.
                int idRestaurante = _restauranteDAL.CrearRestaurante(nombre, idAdminPrincipal, nummesas);

                if (idRestaurante > 0)
                {
                    // 3. CREAR LAS MESAS INICIALES (Llamada al DAL)
                    bool mesasCreadas = _restauranteDAL.CrearMesasIniciales(idRestaurante, numeroMesas);

                    if (mesasCreadas)
                    {
                        return "Éxito: Restaurante '" + nombre + "' y sus " + numeroMesas + " mesas iniciales fueron creados correctamente.";
                    }
                    else
                    {
                        // Si las mesas fallan, es un fallo crítico y se debe notificar.
                        return "Advertencia: El restaurante se creó con éxito, pero falló la creación de las " + numeroMesas + " mesas iniciales.";
                    }
                }
                else
                {
                    // Falló la creación del restaurante.
                    return "Error: No se pudo crear el restaurante. Verifique si el nombre ya está en uso u otros errores de la base de datos.";
                }
            }
            catch (Exception ex)
            {
                // Captura cualquier otro error inesperado (ej. problemas de conexión).
                return "Error inesperado en la creación del restaurante: " + ex.Message;
            }
        }

        /// <summary>
        /// Obtiene un DataTable con la lista de todos los restaurantes disponibles.
        /// </summary>
        public DataTable ObtenerRestaurantes()
        {
            // Columnas corregidas según tu DDL.

            string sql = @"SELECT id_restaurante, nombre_restaurante, numeromesas FROM Restaurantes ORDER BY nombre_restaurante;";

            using (NpgsqlConnection conn = _conexionDB.GetConnection())
            {
                conn.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);

                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    return dt;
                }
            }
        }
    }
}