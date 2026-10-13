
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ProyectoProductos
{
    public static class Imagenes
    {
      
        public static byte[] ImageToByteArray(Image image)
        {
            if (image == null)
                return null;

            try
            {
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

      
        public static Bitmap ByteArrayToBitmap(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            try
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    using (Bitmap bmp = new Bitmap(ms))
                    {
                        return new Bitmap(bmp);
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
