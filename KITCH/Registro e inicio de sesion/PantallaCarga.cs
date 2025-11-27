using System;
using System.Windows.Forms;
// Nota: 'guna2ProgressBar1' y 'lblEstado' son controles asumidos de tu diseño.

namespace KITCH.Registro_e_inicio_de_sesion
{
    public partial class PantallaCarga : Form
    {
        // Campo privado para almacenar la referencia del formulario a mostrar
        private Form _formSiguiente;
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        // El constructor ahora recibe un tipo Form genérico para mayor flexibilidad
        public PantallaCarga(Form formToShow)
        {
            InitializeComponent();
            // 1. ALMACENAR LA REFERENCIA
            _formSiguiente = formToShow;
            InicializarCarga();

            // Configuración visual recomendada (si no está en el diseñador)
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
        }

        private void InicializarCarga()
        {
            // Asegúrate de que los nombres de los controles 'guna2ProgressBar1' y 'lblEstado' 
            // coincidan con los nombres de tus componentes en el diseñador.
            guna2ProgressBar1.Minimum = 0;
            guna2ProgressBar1.Maximum = 100;
            guna2ProgressBar1.Value = 0;

            timer.Interval = 50; // velocidad de carga
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (guna2ProgressBar1.Value < guna2ProgressBar1.Maximum)
            {
                guna2ProgressBar1.Value += 2;
                lblEstado.Text = $"Cargando... {guna2ProgressBar1.Value}%";
            }
            else
            {
                timer.Stop();
                lblEstado.Text = "Carga completa";

                // 2. MOSTRAR EL FORMULARIO ALMACENADO Y CERRAR ESTE
                // Nota: El formulario de inicio de sesión ya fue ocultado por el Login.
                if (_formSiguiente != null)
                {
                    _formSiguiente.Show();
                }
                this.Close(); // Cierra el formulario de carga
            }
        }
    }
}