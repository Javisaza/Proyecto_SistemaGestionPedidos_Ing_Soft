using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KITCH.CapaNegocio;
using KITCH.Utilidades;
using KITCH.Modelos; // ⚠️ AÑADIR ESTO

namespace KITCH.Registro_e_inicio_de_sesion
{
    public partial class InicioSesion : Form
    {
        private UsuarioBLL _usuarioBLL = new UsuarioBLL();

        public InicioSesion()
        {
            InitializeComponent();
            txtContraseñaUser.PasswordChar = '*';
        }

        private void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            string nombreUsuario = txtNombreUser.Text.Trim();
            string contrasena = txtContraseñaUser.Text;

            // ... (Validación de campos vacíos) ...

            try
            {
                DataTable datosUsuario = _usuarioBLL.IniciarSesion(nombreUsuario, contrasena);

                if (datosUsuario != null && datosUsuario.Rows.Count > 0)
                {
                    DataRow row = datosUsuario.Rows[0];

                    // ----------------------------------------------------------------------
                    // ✅ CORRECCIÓN CLAVE: OBTENER Y ASIGNAR ID DEL RESTAURANTE
                    // ----------------------------------------------------------------------

                    // 1. Asignar datos a la Sesión Global
                    int idRestauranteLogueado = Convert.ToInt32(row["id_restaurante"]);

                    // 🔑 Asignación a la clase global de sesión (que se usa en MenuCamarero)
                    KITCH.Globales.SesionActual.IdRestaurante = idRestauranteLogueado;
                    KITCH.Globales.SesionActual.IdUsuario = Convert.ToInt32(row["id_usuario"]);
                    // (Asigna también el Nombre del Restaurante si lo tienes)

                    // 2. Crear el objeto UsuarioLogueado (para pasar al siguiente formulario)
                    Usuario usuarioLogueado = new Usuario
                    {
                        IdUsuario = KITCH.Globales.SesionActual.IdUsuario,
                        Email = row["email"].ToString(),
                        Nombre = row["nombre"].ToString(),
                        // ... (Otras propiedades)
                        // ⚠️ No te olvides de cargar el IdRestaurante en el objeto Usuario si es necesario
                    };

                    MessageBox.Show($"Bienvenido, {usuarioLogueado.Nombre}! Ha iniciado sesión en Restaurante ID: {idRestauranteLogueado}.", "Acceso Concedido", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 3. Redirección
                    this.Hide();

                    SeleccionRestaurante formPrincipal = new SeleccionRestaurante(usuarioLogueado);
                    PantallaCarga pantallaCarga = new PantallaCarga(formPrincipal);
                    pantallaCarga.Show();
                    Application.DoEvents();

                }
                else
                {
                    // INICIO DE SESIÓN FALLIDO (Credenciales no válidas)
                    MessageBox.Show("Usuario o contraseña incorrectos. Verifique sus credenciales.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores de conexión o del sistema
                MessageBox.Show("Ha ocurrido un error al intentar iniciar sesión: " + ex.Message, "Error del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

        }

        // --- Resto de los métodos (lblRegistrate_Click, lblIniciarSesionCreada_Click, imgBtnVerContra_Click) se mantienen igual ---
        private void lblRegistrate_Click(object sender, EventArgs e)
        {
            RegistroUsuario formRegistro = new RegistroUsuario();
            formRegistro.Show();
            this.Hide();
        }

        private void lblIniciarSesionCreada_Click(object sender, EventArgs e)
        {
            RegistroUsuario registroUsuario = new RegistroUsuario();
            registroUsuario.Show();
            this.Hide();
        }

        private void imgBtnVerContra_Click(object sender, EventArgs e)
        {
            if (txtContraseñaUser.PasswordChar == '*')
            {
                txtContraseñaUser.PasswordChar = '\0'; // Mostrar contraseña
            }
            else
            {
                txtContraseñaUser.PasswordChar = '*'; // Ocultar contraseña
            }
        }
    }
}