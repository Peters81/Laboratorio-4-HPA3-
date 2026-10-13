
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoProductos
{
    internal static class Program
    {
        
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

           
            using (Form2 login = new Form2())
            {
                if (login.ShowDialog() == DialogResult.OK &&
                    login.UsuarioId > 0)
                {
                    
                    Application.Run(
                        new Form1(login.UsuarioId, login.NombreUsuario));
                }
            }
        }
    }
}
