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
        private Dictionary<string, object> myProducto = new Dictionary<string, object>();
        private string rutaImagenSeleccionada = "";

        public Form1()
        {
            InitializeComponent();
            listaProductos = new List<Producto>();

            // Vincula el evento de selección de fila por código para evitar fallos
            dgvProductos.CellClick += dgvProductos_CellClick;
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            // Ajusta la imagen en la celda para que se vea completa (Zoom)
            if (dgvProductos.Columns.Contains("imagen") && dgvProductos.Columns["imagen"] is DataGridViewImageColumn col)
            {
                col.ImageLayout = DataGridViewImageCellLayout.Zoom;
            }

            cargarProductos();
        }

        private void cargarProductos(string filtro = "")
        {
            dgvProductos.Rows.Clear();
            dgvProductos.Refresh();
            listaProductos = Conexion.GetProductos(filtro);

            foreach (var prod in listaProductos)
            {
                Image img = null;
                if (prod.Imagen != null && prod.Imagen.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream(prod.Imagen))
                    {
                        using (Bitmap bmp = new Bitmap(ms))
                        {
                            img = new Bitmap(bmp);
                        }
                    }
                }
                dgvProductos.Rows.Add(prod.Id, prod.Nombre, prod.Precio, prod.Cantidad, img);
            }
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            cargarProductos(txtBusqueda.Text.Trim());
        }

        // Evento del PictureBox fijo/lupa (no realiza acción de carga de archivos)
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        // EVENTO DEL PICTUREBOX DEL PRODUCTO (pictureBox2)
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Seleccionar imagen del producto";
                openFileDialog.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    rutaImagenSeleccionada = openFileDialog.FileName;
                    pictureBox2.Image = Image.FromFile(rutaImagenSeleccionada);
                    pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!datosCorrectos())
            {
                return;
            }

            CargarDatosProductos();

            if (Conexion.InsertSeguro("productos", myProducto))
            {
                MessageBox.Show("Se ha guardado satisfactoriamente el registro");
                cargarProductos();
                limpiarCampos();
            }
        }

        private void CargarDatosProductos()
        {
            myProducto.Clear();
            myProducto["nombre"] = txtNombre.Text.Trim();
            myProducto["precio"] = decimal.Parse(txtPrecio.Text.Trim());
            myProducto["cantidad"] = int.Parse(txtCantidad.Text.Trim());
            myProducto["imagen"] = ImageToByteArray(pictureBox2.Image);
        }

        // Conversor a prueba de fallos de codificador (Encoder Null Fix)
        private byte[] ImageToByteArray(Image image)
        {
            if (image == null)
                return null;

            try
            {
                // Si la imagen proviene de una selección en disco, la leemos directamente
                if (!string.IsNullOrEmpty(rutaImagenSeleccionada) && File.Exists(rutaImagenSeleccionada))
                {
                    return File.ReadAllBytes(rutaImagenSeleccionada);
                }

                // Si viene de Properties.Resources o la celda, redibujamos en un lienzo limpio de memoria
                using (Bitmap bmpLimpio = new Bitmap(image.Width, image.Height))
                {
                    using (Graphics g = Graphics.FromImage(bmpLimpio))
                    {
                        g.DrawImage(image, 0, 0, image.Width, image.Height);
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        bmpLimpio.Save(ms, ImageFormat.Png);
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool datosCorrectos()
        {
            if (txtNombre.Text.Trim().Equals(""))
            {
                MessageBox.Show("Ingrese el Nombre del Producto");
                return false;
            }

            if (txtPrecio.Text.Trim().Equals(""))
            {
                MessageBox.Show("Ingrese el Precio");
                return false;
            }

            if (txtCantidad.Text.Trim().Equals(""))
            {
                MessageBox.Show("Ingrese la Cantidad");
                return false;
            }

            if (!decimal.TryParse(txtPrecio.Text.Trim(), out decimal precio))
            {
                MessageBox.Show("Ingrese un Precio correcto");
                return false;
            }

            if (!int.TryParse(txtCantidad.Text.Trim(), out int cantidad))
            {
                MessageBox.Show("Ingrese una cantidad correcta");
                return false;
            }

            return true;
        }

        /* Código para los demás botones del formulario */

        private int idSeleccionado = 0;

        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvProductos.Rows[e.RowIndex];

                if (fila.Cells[0].Value != null)
                {
                    idSeleccionado = Convert.ToInt32(fila.Cells[0].Value);
                    txtNombre.Text = fila.Cells[1].Value?.ToString();
                    txtPrecio.Text = fila.Cells[2].Value?.ToString();
                    txtCantidad.Text = fila.Cells[3].Value?.ToString();

                    if (fila.Cells[4].Value is Image img)
                    {
                        pictureBox2.Image = img; // Muestra en pictureBox2
                        pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    else
                    {
                        pictureBox2.Image = Properties.Resources.imagen;
                    }
                }
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un producto de la lista para modificar.");
                return;
            }

            if (!datosCorrectos()) return;

            CargarDatosProductos();

            if (Conexion.UpdateSeguro("productos", myProducto, idSeleccionado))
            {
                MessageBox.Show("Registro actualizado correctamente.");
                cargarProductos();
                limpiarCampos();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un producto de la lista para eliminar.");
                return;
            }

            DialogResult confirmacion = MessageBox.Show("¿Está seguro de eliminar este producto?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirmacion == DialogResult.Yes)
            {
                if (Conexion.DeleteSeguro("productos", idSeleccionado))
                {
                    MessageBox.Show("Producto eliminado correctamente.");
                    cargarProductos();
                    limpiarCampos();
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiarCampos();
        }

        private void limpiarCampos()
        {
            txtNombre.Clear();
            txtPrecio.Clear();
            txtCantidad.Clear();
            pictureBox2.Image = Properties.Resources.imagen; // Restaura el icono por defecto
            rutaImagenSeleccionada = "";                     // Reinicia la ruta seleccionada
            idSeleccionado = 0;
        }
    }
}