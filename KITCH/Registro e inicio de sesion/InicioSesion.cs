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

            // 1. Validación de campos vacíos
            if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(contrasena))
            {
                MessageBox.Show("Por favor, ingrese el nombre de usuario y la contraseña.", "Campos Requeridos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 2. Llamada a la Capa de Negocio para autenticar
                DataTable datosUsuario = _usuarioBLL.IniciarSesion(nombreUsuario, contrasena);

                if (datosUsuario != null && datosUsuario.Rows.Count > 0)
                {
                    // INICIO DE SESIÓN EXITOSO
                    DataRow row = datosUsuario.Rows[0];

                    // ----------------------------------------------------------------------
                    // ✅ CREACIÓN DEL OBJETO USUARIO Y PASO DE DATOS
                    // ----------------------------------------------------------------------
                    Usuario usuarioLogueado = new Usuario
                    {
                        IdUsuario = Convert.ToInt32(row["id_usuario"]),
                        Email = row["email"].ToString(),
                        Nombre = row["nombre"].ToString(),
                        Apellido = row["apellido"].ToString(),
                        Rol = row["rol"].ToString()
                    };

                    MessageBox.Show($"Bienvenido, {usuarioLogueado.Nombre}! Ha iniciado sesión como {usuarioLogueado.Rol}.", "Acceso Concedido", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // ----------------------------------------------------------------------
                    // ✅ 3. LÓGICA DE REDIRECCIÓN
                    // ----------------------------------------------------------------------

                    this.Hide();

                    // ⚠️ PASAR EL OBJETO USUARIO AL CONSTRUCTOR DEL FORMULARIO PRINCIPAL
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