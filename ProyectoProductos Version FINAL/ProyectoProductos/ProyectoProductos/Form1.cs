
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoProductos
{
    public partial class Form1 : Form
    {
        private List<Producto> listaProductos;

        private Dictionary<string, object> myProducto =
            new Dictionary<string, object>();

        private string rutaImagenSeleccionada = "";
        private IProductoRepository conexionRepo;
        private int idSeleccionado = 0;

        // Datos del usuario que inició sesión
        private int idUsuarioActual;
        private string nombreUsuarioActual;

        public Form1(int idUsuario, string nombreUsuario)
        {
            InitializeComponent();

            // DATOS del usuario autenticado
            idUsuarioActual = idUsuario;
            nombreUsuarioActual = nombreUsuario;

            listaProductos = new List<Producto>();

           
            conexionRepo = new Conexion
            {
                UsuarioActualId = idUsuarioActual
            };

           
            dgvProductos.CellClick += dgvProductos_CellClick;

           
            this.Text = "Gestión de Productos - " + nombreUsuarioActual;
        }

        // Cargar el formulario 1 ----------------------------------------------------------------------------------------------
        private void Form1_Load_1(object sender, EventArgs e)
        {
            if (dgvProductos.Columns.Contains("imagen") &&
                dgvProductos.Columns["imagen"] is DataGridViewImageColumn col)
            {
                col.ImageLayout = DataGridViewImageCellLayout.Zoom;
            }

            errorProvider1.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            cargarProductos();
        }

        // Mostrar productos activos en el DataGridView
        private void cargarProductos(string filtro = "")
        {
            dgvProductos.Rows.Clear();

            listaProductos = conexionRepo.GetProductos(filtro);

            foreach (var prod in listaProductos)
            {
                Image img = Imagenes.ByteArrayToBitmap(prod.Imagen);

                dgvProductos.Rows.Add(
                    prod.Id,
                    prod.Nombre,
                    prod.Precio,
                    prod.Cantidad,
                    img
                );
            }
        }

        // Buscar productos
        private void txtBusqueda_TextChanged(
            object sender, EventArgs e)
        {
            cargarProductos(txtBusqueda.Text.Trim());
        }

        // PictureBox de búsqueda
        private void pictureBox1_Click(
            object sender, EventArgs e)
        {
        }

        // Seleccionar imagen del producto
        private void pictureBox2_Click(
            object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog =
                new OpenFileDialog())
            {
                openFileDialog.Title =
                    "Seleccionar imagen del producto";

                openFileDialog.Filter =
                    "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp";

                if (openFileDialog.ShowDialog() ==
                    DialogResult.OK)
                {
                    try
                    {
                        rutaImagenSeleccionada =
                            openFileDialog.FileName;

                        
                        byte[] datosImagen =
                            File.ReadAllBytes(rutaImagenSeleccionada);

                        Bitmap imagen =
                            Imagenes.ByteArrayToBitmap(datosImagen);

                        if (imagen == null)
                        {
                            MessageBox.Show(
                                "No se pudo cargar la imagen.");
                            return;
                        }

                        pictureBox2.Image = imagen;

                        pictureBox2.SizeMode =
                            PictureBoxSizeMode.StretchImage;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Error al cargar la imagen: " +
                            ex.Message);
                    }
                }
            }
        }

        // Botón Guardar
        private void btnGuardar_Click(
            object sender, EventArgs e)
        {
            if (!datosCorrectos())
                return;

            CargarDatosProductos();

            if (conexionRepo.InsertSeguro(
                "productos", myProducto))
            {
                MessageBox.Show(
                    "Se ha guardado satisfactoriamente el registro");

                cargarProductos();
                limpiarCampos();
            }
        }

        // Preparar datos del producto
        private void CargarDatosProductos()
        {
            myProducto.Clear();

            myProducto["nombre"] =
                txtNombre.Text.Trim();

            myProducto["precio"] =
                decimal.Parse(txtPrecio.Text.Trim());

            myProducto["cantidad"] =
                int.Parse(txtCantidad.Text.Trim());

            myProducto["imagen"] =
                Imagenes.ImageToByteArray(pictureBox2.Image);
        }

        // ErrorProvider
        private bool datosCorrectos()
        {
            errorProvider1.Clear();

            bool valido = true;

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                errorProvider1.SetError(
                    txtNombre,
                    "Ingrese el nombre del producto");

                valido = false;
            }

            if (!decimal.TryParse(
                txtPrecio.Text.Trim(), out decimal precio))
            {
                errorProvider1.SetError(
                    txtPrecio,
                    "Ingrese un precio válido");

                valido = false;
            }
            else if (precio < 0)
            {
                errorProvider1.SetError(
                    txtPrecio,
                    "El precio no puede ser negativo");

                valido = false;
            }

            if (!int.TryParse(
                txtCantidad.Text.Trim(), out int cantidad))
            {
                errorProvider1.SetError(
                    txtCantidad,
                    "Ingrese una cantidad válida");

                valido = false;
            }
            else if (cantidad < 0)
            {
                errorProvider1.SetError(
                    txtCantidad,
                    "La cantidad no puede ser negativa");

                valido = false;
            }

            return valido;
        }

        // Seleccionar una fila del DataGridView
        private void dgvProductos_CellClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 ||
                e.RowIndex >= dgvProductos.Rows.Count)
                return;

            DataGridViewRow fila =
                dgvProductos.Rows[e.RowIndex];

            if (fila.IsNewRow || fila.Cells[0].Value == null)
                return;

            idSeleccionado =
                Convert.ToInt32(fila.Cells[0].Value);

            txtNombre.Text =
                fila.Cells[1].Value?.ToString();

            txtPrecio.Text =
                fila.Cells[2].Value?.ToString();

            txtCantidad.Text =
                fila.Cells[3].Value?.ToString();

            if (fila.Cells[4].Value is Image img)
            {
                pictureBox2.Image = img;
            }
            else
            {
                pictureBox2.Image =
                    Properties.Resources.imagen;
            }

            pictureBox2.SizeMode =
                PictureBoxSizeMode.StretchImage;

            
            rutaImagenSeleccionada = "";

            errorProvider1.Clear();
        }

        // Botón Modificar
        private void btnModificar_Click(
            object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show(
                    "Seleccione un producto de la lista para modificar.");

                return;
            }

            if (!datosCorrectos())
                return;

            CargarDatosProductos();

            if (conexionRepo.UpdateSeguro(
                "productos", myProducto, idSeleccionado))
            {
                MessageBox.Show(
                    "Registro actualizado correctamente.");

                cargarProductos();
                limpiarCampos();
            }
            else
            {
                MessageBox.Show(
                    "No se pudo actualizar el producto.");
            }
        }

        // Botón Eliminar - Eliminación lógica
        private void btnEliminar_Click(
            object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show(
                    "Seleccione un producto de la lista para eliminar.");

                return;
            }

            DialogResult confirmacion = MessageBox.Show(
                "¿Está seguro de anular este producto?",
                "Confirmar anulación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirmacion == DialogResult.Yes)
            {
                if (conexionRepo.DeleteSeguro(
                    "productos", idSeleccionado))
                {
                    MessageBox.Show(
                        "Producto anulado correctamente.");

                    cargarProductos();
                    limpiarCampos();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo anular el producto.");
                }
            }
        }

        // Botón Limpiar
        private void btnLimpiar_Click(
            object sender, EventArgs e)
        {
            limpiarCampos();
        }

        // Limpiar los campos del formulario
        private void limpiarCampos()
        {
            txtNombre.Clear();
            txtPrecio.Clear();
            txtCantidad.Clear();

            pictureBox2.Image =
                Properties.Resources.imagen;

            pictureBox2.SizeMode =
                PictureBoxSizeMode.StretchImage;

            rutaImagenSeleccionada = "";
            idSeleccionado = 0;

            myProducto.Clear();

            errorProvider1.Clear();

            dgvProductos.ClearSelection();
        }
    }
}
