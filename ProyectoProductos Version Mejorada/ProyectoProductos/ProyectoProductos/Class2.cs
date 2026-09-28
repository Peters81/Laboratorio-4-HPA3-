using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ProyectoProductos
{
    public class Conexion
    {
        private static string cadenaConexion = "Server=localhost; Database=cristell; Uid=root; Pwd=peters;";

        public static MySqlConnection ObtenerConexion()
        {
            try
            {
                MySqlConnection conexion = new MySqlConnection(cadenaConexion);
                conexion.Open();
                return conexion;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al conectar: " + ex.Message);
                return null;
            }
        }

        public static List<Producto> GetProductos(string filtro)
        {
            List<Producto> listaProductos = new List<Producto>();
            string Sqlquery = "SELECT id, nombre, precio, cantidad, imagen FROM productos";

            if (!string.IsNullOrEmpty(filtro))
            {
                Sqlquery += " WHERE id LIKE @filtro OR nombre LIKE @filtro OR precio LIKE @filtro OR cantidad LIKE @filtro";
            }

            using (MySqlConnection conn = ObtenerConexion())
            {
                if (conn == null) return listaProductos;

                using (MySqlCommand cmd = new MySqlCommand(Sqlquery, conn))
                {
                    if (!string.IsNullOrEmpty(filtro))
                    {
                        cmd.Parameters.AddWithValue("@filtro", "%" + filtro + "%");
                    }

                    using (MySqlDataReader mReader = cmd.ExecuteReader())
                    {
                        while (mReader.Read())
                        {
                            Producto prod = new Producto();
                            prod.Id = Convert.ToInt32(mReader["id"]);
                            prod.Nombre = mReader["nombre"].ToString();
                            prod.Precio = Convert.ToDecimal(mReader["precio"]);
                            prod.Cantidad = Convert.ToInt32(mReader["cantidad"]);
                            prod.Imagen = mReader["imagen"] is DBNull ? null : (byte[])mReader["imagen"];

                            listaProductos.Add(prod);
                        }
                        mReader.Close();
                    }
                }
            }

            return listaProductos;
        }

        public static bool InsertSeguro(string tbName, Dictionary<string, object> data)
        {
            var columns = string.Join(", ", data.Keys);
            var placeholders = "@" + string.Join(", @", data.Keys);

            string sql = $"INSERT INTO {tbName} ({columns}) VALUES ({placeholders})";

            try
            {
                using (MySqlConnection conexion = ObtenerConexion())
                {
                    if (conexion == null) return false;

                    using (MySqlCommand stmt = new MySqlCommand(sql, conexion))
                    {
                        foreach (var kvp in data)
                        {
                            stmt.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                        }

                        stmt.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error en INSERT: " + ex.Message);
                return false;
            }
        }

        /*aqui agregue el codigo que no esta en el pdf pero que es para los demas botones del design*/

        public static bool UpdateSeguro(string tbName, Dictionary<string, object> data, int id)
        {
            List<string> setClauses = new List<string>();
            foreach (var key in data.Keys)
            {
                setClauses.Add($"{key} = @{key}");
            }

            string sql = $"UPDATE {tbName} SET {string.Join(", ", setClauses)} WHERE id = @id";

            try
            {
                using (MySqlConnection conexion = ObtenerConexion())
                {
                    if (conexion == null) return false;

                    using (MySqlCommand stmt = new MySqlCommand(sql, conexion))
                    {
                        foreach (var kvp in data)
                        {
                            stmt.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                        }
                        stmt.Parameters.AddWithValue("@id", id);

                        stmt.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al actualizar: " + ex.Message);
                return false;
            }
        }

        public static bool DeleteSeguro(string tbName, int id)
        {
            string sql = $"DELETE FROM {tbName} WHERE id = @id";

            try
            {
                using (MySqlConnection conexion = ObtenerConexion())
                {
                    if (conexion == null) return false;

                    using (MySqlCommand stmt = new MySqlCommand(sql, conexion))
                    {
                        stmt.Parameters.AddWithValue("@id", id);
                        stmt.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
                return false;
            }
        }
    }
}