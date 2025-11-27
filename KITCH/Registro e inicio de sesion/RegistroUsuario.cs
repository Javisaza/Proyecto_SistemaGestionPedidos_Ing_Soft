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
using KITCH.Modelos;
using KITCH.Registro_e_inicio_de_sesion; // Capa de Negocio

namespace KITCH.Utilidades
{
    public partial class RegistroUsuario : Form
    {
        private UsuarioBLL _usuarioBLL = new UsuarioBLL();

        public RegistroUsuario()
        {
            InitializeComponent();
            CargarRoles();
            // Oculta el texto de la contraseña
            txtContraseñaUser.PasswordChar = '*';

            // Prueba de conexión al inicio
            string resultadoConexion = _usuarioBLL.ProbarConexionDB();

            // 2. MOSTRAR EL RESULTADO
            if (resultadoConexion.StartsWith("Éxito"))
            {
                // Mostrar mensaje de éxito si la conexión es correcta
                MessageBox.Show(resultadoConexion, "Conexión Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Si la conexión es exitosa, se procede a cargar el resto del formulario
            }
            else
            {
                // Mostrar mensaje de error si la conexión falla
                MessageBox.Show(resultadoConexion, "Error Crítico de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Opcional: Deshabilitar el botón de registro si no hay conexión
                // btnRegistrarse.Enabled = false; 
            }
        }

        private void CargarRoles()
        {
            // Estos valores deben coincidir con la restricción CHECK de tu tabla Usuarios
            cmbRol.Items.AddRange(new object[] {
            "Administrador Principal",
            "Administrador",
            "Mesero",
            "Cocinero"
            });
            cmbRol.SelectedIndex = -1; // No selecciona nada inicialmente
        }

        private void btnRegistrarse_Click(object sender, EventArgs e)
        {
            // --- 1. RECOLECCIÓN Y VALIDACIÓN DE DATOS (Ajustada) ---

            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string numeroIdentificacion = txtIdentificacion.Text.Trim();
            string email = txtEmail.Text.Trim();
            string telefono = txtTelefono.Text.Trim(); // Opcional
            string nombreUsuario = txtNombreUser.Text.Trim();
            string contrasena = txtContraseñaUser.Text;
            string rol = cmbRol.SelectedItem?.ToString();

            // Validar que TODOS los campos obligatorios NO estén vacíos
            if (string.IsNullOrEmpty(nombre) ||
                string.IsNullOrEmpty(apellido) ||
                string.IsNullOrEmpty(numeroIdentificacion) ||
                string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(nombreUsuario) ||
                string.IsNullOrEmpty(contrasena) ||
                string.IsNullOrEmpty(rol))
            {
                // La validación ahora solo incluye los campos existentes como obligatorios.
                MessageBox.Show("Debe completar todos los campos obligatorios del formulario (excepto Teléfono).", "Validación Incompleta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Llamada a la Lógica de Negocio (BLL)
            // Se envía la contraseña al BLL para ser hasheada e insertada.
            string resultado = _usuarioBLL.RegistrarNuevoUsuario(
                email,
                contrasena,
                nombreUsuario,
                nombre,
                apellido,
                rol,
                telefono, // Pasa el valor al DAL para ser manejado como NULL si está vacío
                numeroIdentificacion
            );

            // 3. Manejo y presentación del resultado
            if (resultado.StartsWith("Éxito"))
            {
                MessageBox.Show(resultado, "Registro Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();

                // ----------------------------------------------------------------------------------
                // ✅ FLUJO CORREGIDO: Iniciar sesión automáticamente después del registro exitoso
                // ----------------------------------------------------------------------------------

                // Intenta iniciar sesión con las credenciales que se acaban de registrar.
                // IMPORTANTE: Debes asegurarte que este método exista en tu BLL y devuelva el objeto Usuario.
                Usuario usuarioLogueado = _usuarioBLL.InicioSesionUsuario(nombreUsuario, contrasena);

                if (usuarioLogueado != null)
                {
                    // Crear el formulario de selección de restaurante, pasándole el usuario logueado
                    SeleccionRestaurante siguienteFormulario = new SeleccionRestaurante(usuarioLogueado);

                    // Mostrar la pantalla de carga, pasándole el siguiente formulario a abrir
                    PantallaCarga pantallaCarga = new PantallaCarga(siguienteFormulario);
                    pantallaCarga.Show();

                    // Ocultar el formulario actual
                    this.Hide();
                }
                else
                {
                    // Esto no debería pasar si el registro fue exitoso, pero es bueno manejarlo
                    MessageBox.Show("Registro exitoso, pero falló el inicio de sesión automático. Por favor, inicie sesión manualmente.", "Error de Sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    // Redirigir al login si el inicio de sesión automático falla
                    InicioSesion loginForm = new InicioSesion();
                    loginForm.Show();
                    this.Hide();
                }
            }
            else
            {
                // Mostrar el error devuelto por la lógica de negocio (BLL)
                MessageBox.Show(resultado, "Error de Registro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Eliminamos las líneas que estaban fuera del "if" y causaban el error al no tener usuario.
            // PantallaCarga pantallaCarga = new PantallaCarga(new SeleccionRestaurante());
            // pantallaCarga.Show();
            // this.Hide();
            // Application.DoEvents(); 
        }

        /// <summary>
        /// Método auxiliar para limpiar los campos del formulario.
        /// </summary>
        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtApellido.Clear();
            txtIdentificacion.Clear();
            txtEmail.Clear();
            txtTelefono.Clear();
            cmbRol.SelectedIndex = -1;
            txtNombreUser.Clear();
            txtContraseñaUser.Clear();
        }

        // --- MANEJO DEL BOTÓN DE MOSTRAR/OCULTAR CONTRASEÑA ---

        private void guna2ImageButton1_Click_1(object sender, EventArgs e)
        {
            if (txtContraseñaUser.PasswordChar != '*')
            {
                // Ocultar la contraseña
                txtContraseñaUser.PasswordChar = '*';
            }
            else
            {
                // Mostrar la contraseña
                txtContraseñaUser.PasswordChar = '\0';
            }
        }

        private void lblIniciarSesionCreada_Click(object sender, EventArgs e)
        {
            InicioSesion inicioSesion = new InicioSesion();
            inicioSesion.Show();
            this.Hide();
        }
    }
}