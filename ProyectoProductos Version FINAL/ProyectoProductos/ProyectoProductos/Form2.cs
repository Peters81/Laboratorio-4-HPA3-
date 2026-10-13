
using System;
using System.Windows.Forms;

namespace ProyectoProductos
{
    public partial class Form2 : Form
    {
        public int UsuarioId { get; private set; }
        public string NombreUsuario { get; private set; }

        public Form2()
        {
            InitializeComponent();

           
            txtContra.UseSystemPasswordChar = true;
            this.AcceptButton = button1;
            this.CancelButton = button2;
        }

        // Botón INGRESAR
        private void button1_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contra = txtContra.Text;

          
            if (string.IsNullOrWhiteSpace(usuario))
            {
                MessageBox.Show("Ingrese su usuario.");
                txtUsuario.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(contra))
            {
                MessageBox.Show("Ingrese su contraseña.");
                txtContra.Focus();
                return;
            }

        
            int idUsuario = Conexion.ValidarUsuario(usuario, contra);

            if (idUsuario > 0)
            {
         
                UsuarioId = idUsuario;
                NombreUsuario = usuario;

                MessageBox.Show(
                    "Bienvenido, " + usuario,
                    "Inicio de sesión",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // Indicar que el login fue exitoso
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(
                    "Usuario o contraseña incorrectos.",
                    "Error de acceso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtContra.Clear();
                txtContra.Focus();
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

    }
}
