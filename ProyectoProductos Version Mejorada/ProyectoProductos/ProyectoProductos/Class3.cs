using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Proyecto_Productos
{
    public static class Imagenes
    {
        public static byte[] ImageToByteArray(Image image)
        {
            if (image == null)
                return null;

            try
            {
                // Creamos un Bitmap limpio a partir de la imagen para evitar 
                // errores de formato nulo/recursos cuando la imagen viene de Properties.Resources
                using (Bitmap bmp = new Bitmap(image))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bmp.Save(ms, ImageFormat.Png);
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}