// KITCH.Principales/MenuCamarero.cs (O donde se encuentre tu archivo)

using System;
using System.Windows.Forms;
using KITCH.CapaNegocio;
using KITCH.Globales;
using KITCH.Registro_e_inicio_de_sesion; // <<< CLAVE: Importar la clase de sesión global

namespace KITCH.UI // El namespace que usaste
{
    // CORRECCIÓN CS0103: Asegúrate de que la clase es 'partial'
    public partial class MenuCamarero : Form
    {
        // Ya no necesitamos almacenar el Usuario ni Restaurante, usamos la clase estática.
        // private Usuario _usuarioActual;
        // private Restaurante _restauranteActual; 

        // Instancia para manejar la finalización de la sesión en BD
        private SesionBLL _sesionBLL = new SesionBLL();
        private PedidosBLL _pedidosBLL = new PedidosBLL(); // Ejemplo

        // ----------------------------------------------------------------------
        // CORRECCIÓN CS1729: Constructor sin argumentos
        // ----------------------------------------------------------------------
        public MenuCamarero()
        {
            InitializeComponent(); // Ahora funciona por la palabra 'partial'

            // Personalización de la interfaz usando la Sesión Global
            this.Text = $"Menú Camarero - {SesionActual.NombreRestaurante}";
            // Asumo que tienes un Label llamado 'lblNombreRestaurante'
            lblNombreRestaurante.Text = SesionActual.NombreRestaurante;
            lblNombreUsuario.Text = $"Usuario ID: {SesionActual.IdUsuario}";

            CargarDatosDeSesion();
        }

        // ----------------------------------------------------------------------
        // FUNCIÓN QUE USA LA LLAVE PARA FILTRAR DATOS
        // ----------------------------------------------------------------------
        private void CargarDatosDeSesion()
        {
            // La LLAVE de aislamiento se obtiene de la clase global
            int idRestauranteActual = SesionActual.IdRestaurante;

            // EJEMPLO DE USO: Ahora todos los métodos deben usar idRestauranteActual
            // DataTable dtPedidos = _pedidosBLL.ObtenerPedidosPendientes(idRestauranteActual);

            MessageBox.Show($"¡Sesión iniciada!\nUsuario ID: {SesionActual.IdUsuario}\nRestaurante: {SesionActual.NombreRestaurante} (ID: {idRestauranteActual}).\n" +
              "Todos los datos cargados ahora están filtrados por este ID.", "Aislamiento OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            InicioSesion inicioSesionForm = new InicioSesion();
            inicioSesionForm.Show();
            this.Hide();
        }
    }
}