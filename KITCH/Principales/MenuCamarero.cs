using System;
using System.Data;
using System.Windows.Forms;
using KITCH.CapaNegocio;
using KITCH.Registro_e_inicio_de_sesion;
using Npgsql;
using KITCH.CapaDatos; // Necesario para _conexionDB
using KITCH.Globales;
// Ajusta los using según tu estructura

namespace KITCH.UI
{
    public partial class MenuCamarero : Form
    {
        // 🔑 CORRECCIÓN: Usar la variable de conexión correcta
        private ConexionDB _conexionDB = new ConexionDB();
        private SesionBLL _sesionBLL = new SesionBLL(); // Reemplaza si el nombre es diferente
        private PedidosBLL _pedidosBLL = new PedidosBLL(); // Reemplaza si el nombre es diferente
        private int _mesaActualSeleccionada = 0;

        public MenuCamarero()
        {
            InitializeComponent();
            CargarDatosDeSesion();

            // Llamar a CargarMesasDisponibles aquí puede causar el error 'Restaurante ID: 0' si la sesión no está lista.
            // Es mejor llamarlo solo en btnNuevaOrden_Click.
            // CargarMesasDisponibles(); 

            pnlMesa.Visible = false;
            pnlPedido.Visible = false;
        }

        // ----------------------------------------------------------------------
        // FUNCIÓN PARA CARGAR MESAS DISPONIBLES
        // ----------------------------------------------------------------------
        private void CargarMesasDisponibles()
        {
            // 🔑 CORRECCIÓN 1: Limpiar ComboBox de forma segura (resuelve System.ArgumentException)
            cmbMesasDisponibles.DataSource = null;
            cmbMesasDisponibles.Items.Clear();

            if (SesionActual.IdRestaurante <= 0)
            {
                // Este mensaje aparecerá si el login falla en cargar el ID (Resuelve Advertencia)
                MessageBox.Show($"Advertencia: El ID del Restaurante no se cargó correctamente (ID: {SesionActual.IdRestaurante}). Inicie sesión de nuevo.",
                                "Error de Sesión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 🔑 CORRECCIÓN 2: Cambiar 'mesa' por 'mesas' (Resuelve Error SQL 42P01)
            string query =
                "SELECT id_mesas, numero_mesa FROM mesas WHERE estado_mesa = 'Libre' AND id_restaurante = @idRestaurante ORDER BY numero_mesa";

            try
            {
                using (NpgsqlConnection connection = _conexionDB.GetConnection()) // 🔑 CORRECCIÓN 3: Usar _conexionDB (resuelve CS0103)
                {
                    NpgsqlCommand command = new NpgsqlCommand(query, connection);
                    command.Parameters.AddWithValue("@idRestaurante", SesionActual.IdRestaurante);

                    connection.Open();

                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                    DataTable dtMesas = new DataTable();
                    adapter.Fill(dtMesas);

                    // Si el error 'No se puede enlazar con el nuevo miembro de presentación' persiste, 
                    // verifica que 'id_mesas' y 'numero_mesa' sean los nombres exactos de las columnas en tu DB.
                    cmbMesasDisponibles.DisplayMember = "numero_mesa";
                    cmbMesasDisponibles.ValueMember = "id_mesas";
                    cmbMesasDisponibles.DataSource = dtMesas;

                    if (dtMesas.Rows.Count > 0)
                    {
                        cmbMesasDisponibles.SelectedIndex = 0;
                    }
                    else
                    {
                        MessageBox.Show($"Advertencia: No se encontraron mesas 'Libres' para el Restaurante ID: {SesionActual.IdRestaurante}.",
                                        "Sin Resultados", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las mesas disponibles: " + ex.Message,
                                 "Error de Conexión o Consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ----------------------------------------------------------------------
        // EVENTO: BOTÓN ACEPTAR MESA (Soluciona los errores de conversión)
        // ----------------------------------------------------------------------
        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (cmbMesasDisponibles.SelectedItem == null)
            {
                MessageBox.Show("Seleccione una mesa disponible.", "Advertencia");
                return;
            }

            // 🔑 CORRECCIÓN 4: Uso de Convert.ToInt32(SelectedValue.ToString()) para evitar InvalidCastException
            // Ya que ValueMember es "id_mesas" (un entero en la DB)
            try
            {
                // 1. Obtener y Asignar ID de Mesa (ValueMember: id_mesas)
                _mesaActualSeleccionada = Convert.ToInt32(cmbMesasDisponibles.SelectedValue.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener ID de mesa: " + ex.Message, "Error de Datos");
                return;
            }

            // 2. Obtener y Asignar Número Visible (DisplayMember: numero_mesa)
            DataRowView row = (DataRowView)cmbMesasDisponibles.SelectedItem;

            if (row["numero_mesa"] == DBNull.Value)
            {
                MessageBox.Show("El número de mesa no está definido.", "Error de Datos");
                return;
            }

            // 🔑 CORRECCIÓN 5: Uso de TryParse para manejar texto como "Mesa 3" (Resuelve FormatException)
            // Si la columna numero_mesa en la DB contiene "Mesa 3", esto fallará. 
            // Si solo contiene números, el TryParse será exitoso.
            int numMesaDisplay = 0;
            if (!int.TryParse(row["numero_mesa"].ToString().Replace("Mesa", "").Trim(), out numMesaDisplay))
            {
                MessageBox.Show($"Error de Formato: El valor '{row["numero_mesa"]}' no es un número de mesa válido. Asegúrese de que la columna solo contenga números.",
                                "Error de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string updateQuery = "UPDATE mesas SET estado_mesa = 'Ocupada' WHERE id_mesas = @MesaID";

            try
            {
                using (NpgsqlConnection connection = _conexionDB.GetConnection())
                {
                    NpgsqlCommand command = new NpgsqlCommand(updateQuery, connection);
                    command.Parameters.AddWithValue("@MesaID", _mesaActualSeleccionada);

                    connection.Open();
                    command.ExecuteNonQuery();

                    pnlMesa.Visible = false;
                    pnlPedido.Visible = true;
                    lblTitulo.Text = $"Tomando Pedido para la Mesa: {numMesaDisplay}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar el pedido y actualizar la mesa: " + ex.Message,
                                 "Error de DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ----------------------------------------------------------------------
        // OTROS MÉTODOS
        // ----------------------------------------------------------------------
        private void btnNuevaOrden_Click(object sender, EventArgs e)
        {
            CargarMesasDisponibles();
            pnlMesa.Visible = true;
            pnlPedido.Visible = false;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            pnlMesa.Visible = false;
        }

        private void CargarDatosDeSesion()
        {
            // Implementación aquí (ej: asignación a labels)
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            InicioSesion inicioSesionForm = new InicioSesion();
            inicioSesionForm.Show();
            this.Hide();
        }
    }
}