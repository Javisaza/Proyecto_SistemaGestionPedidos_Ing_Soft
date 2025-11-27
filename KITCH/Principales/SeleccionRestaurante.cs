using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using KITCH.CapaDatos;
using KITCH.CapaNegocio;
using KITCH.Modelos; // Importar el modelo de usuario
using KITCH.Globales;
using KITCH.UI;

// Usamos el namespace de tu lógica de sesión anterior para evitar problemas de referencia
namespace KITCH.Registro_e_inicio_de_sesion 
{
    public partial class SeleccionRestaurante : Form
    {
        // Campos para almacenar la información del usuario y las capas de negocio
        private Usuario _usuarioLogueado;
        private RestauranteBLL _restauranteBLL = new RestauranteBLL();
   
        private SesionBLL _sesionBLL = new SesionBLL();


        // ----------------------------------------------------------------------
        // CONSTRUCTOR
        // ----------------------------------------------------------------------
        public SeleccionRestaurante(Usuario usuario)
        {
            InitializeComponent();
            _usuarioLogueado = usuario;
            InicializarFormulario();
            // 1. LLAMADA CLAVE: Cargar la lista al iniciar la pantalla
            CargarRestaurantesDisponibles(); 
        }
        
        // ----------------------------------------------------------------------
        // LÓGICA DE CARGA Y RENDERIZADO DE RESTAURANTES
        // ----------------------------------------------------------------------

        private void CargarRestaurantesDisponibles()
        {
            // 1. LIMPIAR EL CONTENEDOR ANTES DE AÑADIR NUEVOS ELEMENTOS (CRUCIAL)
            if (flpRestaurante.Controls.Count > 0)
            {
                flpRestaurante.Controls.Clear();
            }

            // 2. OBTENER LOS DATOS ACTUALIZADOS
            // Nota: Este método debe funcionar correctamente en la DAL (RestauranteDAL.cs) 
            // para que no arroje el error de columna 'nombre' no encontrada.
            DataTable dtRestaurantes = _restauranteBLL.ObtenerRestaurantes();

            if (dtRestaurantes.Rows.Count > 0)
            {
                // 3. ITERAR Y CREAR LOS CONTROLES DE LA UI
                foreach (DataRow fila in dtRestaurantes.Rows)
                {
                    try
                    {
                        // Asegúrate de usar los nombres de columna CORRECTOS de la BD
                        int id = Convert.ToInt32(fila["id_restaurante"]);
                        string nombre = fila["Nombre_restaurante"].ToString();
                        
                        // Asumimos que tienes el número de mesas o usamos 0 si no existe la columna
                        int mesas = fila.Table.Columns.Contains("NumeroMesas") ? Convert.ToInt32(fila["NumeroMesas"]) : 0;
                        
                        // --- CREACIÓN DEL CONTROL PERSONALIZADO ---
                        Control item = CrearItemRestaurante(id, nombre, mesas);
                        
                        // 4. AÑADIR EL CONTROL AL FLOWLAYOUTPANEL
                        flpRestaurante.Controls.Add(item);
                    }
                    catch (Exception ex)
                    {
                        // Mostrar error si alguna fila tiene problemas de formato
                        Console.WriteLine($"Error al procesar fila de restaurante: {ex.Message}");
                    }
                }
            }
            else
            {
                // Manejo de caso sin restaurantes
                Label lblNoRestaurantes = new Label
                {
                    Text = "No hay restaurantes disponibles. Crea uno para empezar.",
                    AutoSize = true,
                    Margin = new Padding(10),
                    ForeColor = Color.White
                };
                flpRestaurante.Controls.Add(lblNoRestaurantes);
            }
        }
        
        // ----------------------------------------------------------------------
        // INICIALIZACIÓN Y CONFIGURACIÓN DE PANTALLA
        // ----------------------------------------------------------------------
        private void InicializarFormulario()
        {
            // 1. Mostrar nombre y rol del usuario
            lblUsuarioRol.Text = $"{_usuarioLogueado.Nombre} ({_usuarioLogueado.Rol})";

            // 2. Configurar el botón de creación (visible solo para Admin Principal)
            // Asumo que tienes una propiedad para verificar el rol.
            btnCrearRestaurante.Visible = _usuarioLogueado.EsAdministradorPrincipal;

            // 3. Mostrar la vista de Selección por defecto
            MostrarPanelSeleccion();

            // 4. Configurar eventos de KeyPress para solo números (si tienes un txtNumeroMesas)
            // if (txtNumeroMesas != null)
            // {
            //    txtNumeroMesas.KeyPress += SoloNumeros_KeyPress; 
            // }
        }

        private void SoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Asegura que solo se puedan ingresar números
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // ----------------------------------------------------------------------
        // GESTIÓN DE VISTAS (Paneles)
        // ----------------------------------------------------------------------

        /// <summary>
        /// Muestra el panel de Selección de Restaurantes y oculta el de Creación.
        /// </summary>
        private void MostrarPanelSeleccion()
        {
            pnlSeleccion.Visible = true;
            pnlCreacion.Visible = false;
            // ⚠️ Importante: Recargar la lista cada vez que volvemos a esta vista
            CargarRestaurantesDisponibles(); 
        }

        /// <summary>
        /// Muestra el panel de Creación de Restaurantes y oculta el de Selección.
        /// </summary>
        private void MostrarPanelCreacion()
        {
            pnlCreacion.Visible = true;
            // Limpiar campos al mostrar
            txtNombreRestaurante.Text = "";
            txtNumeroMesas.Text = "";
            txtNombreRestaurante.Focus();
        }

        // ----------------------------------------------------------------------
        // FUNCIÓN DE RENDERIZADO DE ITEM
        // ----------------------------------------------------------------------

        /// <summary>
        /// Crea un Panel estilizado para representar un restaurante en la lista.
        /// </summary>
        private Control CrearItemRestaurante(int id, string nombre, int mesas)
        {
            Panel pnlItem = new Panel();
            pnlItem.Name = $"pnlRestaurante_{id}";
            pnlItem.Size = new Size(200, 100);
            pnlItem.Margin = new Padding(10);
            pnlItem.BackColor = Color.FromArgb(40, 44, 52); // Fondo oscuro
            pnlItem.BorderStyle = BorderStyle.FixedSingle;
            pnlItem.Cursor = Cursors.Hand;
            pnlItem.Tag = id; // Almacenamos el ID en el Tag

            // Título
            Label lblNombre = new Label();
            lblNombre.Text = nombre;
            lblNombre.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblNombre.ForeColor = Color.White;
            lblNombre.Location = new Point(10, 10);
            lblNombre.AutoSize = true;

            // Subtítulo
            Label lblDetalles = new Label();
            lblDetalles.Text = $"Mesas: {mesas} (Click para seleccionar)";
            lblDetalles.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            lblDetalles.ForeColor = Color.Silver;
            lblDetalles.Location = new Point(10, 40);
            lblDetalles.AutoSize = true;

            pnlItem.Controls.Add(lblNombre);
            pnlItem.Controls.Add(lblDetalles);

            // Asignar el evento de clic para simular la selección/unión
            pnlItem.Click += ItemRestaurante_Click;
            lblNombre.Click += ItemRestaurante_Click; // Permitir clic en el Label
            lblDetalles.Click += ItemRestaurante_Click; // Permitir clic en el Label

            return pnlItem;
        }

        private void ItemRestaurante_Click(object sender, EventArgs e)
        {
            // Obtener el control que fue clickeado (puede ser el Panel o uno de sus Labels)
            Control clickedControl = sender as Control;

            // Subir hasta encontrar el Panel que contiene el Tag del ID
            Panel pnlItem = clickedControl as Panel;
            if (pnlItem == null)
            {
                pnlItem = clickedControl.Parent as Panel;
            }

            if (pnlItem != null && pnlItem.Tag != null)
            {
                int idRestauranteSeleccionado = (int)pnlItem.Tag;
                string nombreRestaurante = ((Label)pnlItem.Controls[0]).Text;

                // Verificar si 'idSesionTrabajo' está definido en el contexto actual
                int idSesionTrabajo = SesionActual.IdSesionTrabajo; // Asumimos que este valor ya está configurado en 'SesionActual'

                // Lógica real de "Unirse" o "Seleccionar" iría aquí
                MessageBox.Show($"Restaurante Seleccionado: {nombreRestaurante} (ID: {idRestauranteSeleccionado}).\n" +
                                 "Ahora se abriría la ventana principal del POS.", "Selección Exitosa",
                                 MessageBoxButtons.OK, MessageBoxIcon.Information);

                SesionActual.CargarSesion(
                    _usuarioLogueado.IdUsuario,
                    idRestauranteSeleccionado,
                    nombreRestaurante,
                    idSesionTrabajo
                );
                MenuCamarero menuForm = new MenuCamarero();
                menuForm.Show();
                this.Hide();
            }
        }

        // ----------------------------------------------------------------------
        // EVENTOS DE BOTONES
        // ----------------------------------------------------------------------

        private void btnAgregarRestaurante_Click_1(object sender, EventArgs e)
        {
            //Condicional para mostrar el panel de creación solo si el usuario es Admin Principal
            if (!_usuarioLogueado.EsAdministradorPrincipal)
            {
                MessageBox.Show("Solo los Administradores Principales pueden crear nuevos restaurantes.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MostrarPanelCreacion();
            pnlSeleccion.Size = new Size(729,194); // Ajusta el tamaño del panel de selección al de creación
        }
        private void btnCancelar_Click_1(object sender, EventArgs e)
        {
            MostrarPanelSeleccion(); // Vuelve al panel de selección y recarga la lista
            pnlSeleccion.Size = new Size(729, 391); // Restaurar el tamaño original del panel de selección
        }
        
        private void btnCrearRestaurante_Click(object sender, EventArgs e)
        {
            string nombre = txtNombreRestaurante.Text.Trim();
            string numMesasStr = txtNumeroMesas.Text.Trim();

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("El nombre del restaurante no puede estar vacío.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(numMesasStr, out int numeroMesas) || numeroMesas <= 0)
            {
                MessageBox.Show("Por favor, ingrese un número válido de mesas.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Llamada a la Capa de Negocio (con los 4 parámetros requeridos por la BLL)
            string resultado = _restauranteBLL.CrearRestaurante(
                nombre,
                _usuarioLogueado.IdUsuario,
                numeroMesas,
                numeroMesas // Se pasa el mismo valor para el parámetro "nummesas"
            );

            if (resultado.StartsWith("Éxito"))
            {
                MessageBox.Show(resultado, "Creación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 2. LLAMADA CLAVE: Volver a la selección y refrescar la lista
                MostrarPanelSeleccion();
            }
            else
            {
                MessageBox.Show(resultado, "Error de Creación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            pnlSeleccion.Size = new Size(729, 391); // Restaurar el tamaño original del panel de selección
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            // Lógica para cerrar sesión
            MessageBox.Show("Sesión cerrada. Volviendo a la pantalla de Login.", "Cerrar Sesión", MessageBoxButtons.OK, MessageBoxIcon.Information);

            InicioSesion loginForm = new InicioSesion();
            loginForm.Show();

            // Cerrar el formulario actual
            this.Close();
        }

        private void flpRestaurante_Paint_1(object sender, PaintEventArgs e)
        {

        }
    }
}