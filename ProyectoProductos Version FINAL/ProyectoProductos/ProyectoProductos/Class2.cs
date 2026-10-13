
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ProyectoProductos
{
    public class Conexion : IProductoRepository
    {
        private static string cadenaConexion =
            "Server=localhost; Database=cristell; Uid=root; Pwd=peters;";

        // Usuario que inició sesión
        public int UsuarioActualId { get; set; }


        // CONEXIÓN A LA BASSE DE DATOS


        public static MySqlConnection ObtenerConexion()
        {
            try
            {
                MySqlConnection conexion =
                    new MySqlConnection(cadenaConexion);

                conexion.Open();
                return conexion;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al conectar: " + ex.Message);
                return null;
            }
        }

       
        // CONSULTAR PRODUCTOS
       

        public List<Producto> GetProductos(string filtro)
        {
            List<Producto> listaProductos = new List<Producto>();

            string Sqlquery =
                "SELECT id, nombre, precio, cantidad, imagen " +
                "FROM productos WHERE anulado = 0";

            if (!string.IsNullOrEmpty(filtro))
            {
                Sqlquery +=
                    " AND (id LIKE @filtro " +
                    "OR nombre LIKE @filtro " +
                    "OR precio LIKE @filtro " +
                    "OR cantidad LIKE @filtro)";
            }

            using (MySqlConnection conn = ObtenerConexion())
            {
                if (conn == null)
                    return listaProductos;

                using (MySqlCommand cmd =
                    new MySqlCommand(Sqlquery, conn))
                {
                    if (!string.IsNullOrEmpty(filtro))
                    {
                        cmd.Parameters.AddWithValue(
                            "@filtro", "%" + filtro + "%");
                    }

                    using (MySqlDataReader mReader =
                        cmd.ExecuteReader())
                    {
                        while (mReader.Read())
                        {
                            Producto prod = new Producto();

                            prod.Id =
                                Convert.ToInt32(mReader["id"]);

                            prod.Nombre =
                                mReader["nombre"].ToString();

                            prod.Precio =
                                Convert.ToDecimal(mReader["precio"]);

                            prod.Cantidad =
                                Convert.ToInt32(mReader["cantidad"]);

                            prod.Imagen =
                                mReader["imagen"] is DBNull
                                ? null
                                : (byte[])mReader["imagen"];

                            listaProductos.Add(prod);
                        }
                    }
                }
            }

            return listaProductos;
        }

        // ========================================
        // INSERTAR PRODUCTOS
        // ========================================

        public bool InsertSeguro(
            string tbName,
            Dictionary<string, object> data)
        {
            if (tbName != "productos" ||
                data == null || data.Count == 0 ||
                UsuarioActualId <= 0)
                return false;

            string sql =
                "INSERT INTO productos " +
                "(nombre, precio, cantidad, imagen, " +
                "usuario_creacion, fecha_creacion) " +
                "VALUES (@nombre, @precio, @cantidad, @imagen, " +
                "@usuario, NOW())";

            try
            {
                using (MySqlConnection conexion = ObtenerConexion())
                {
                    if (conexion == null)
                        return false;

                    using (MySqlTransaction transaccion =
                        conexion.BeginTransaction())
                    {
                        try
                        {
                            int idProducto;

                            using (MySqlCommand stmt =
                                new MySqlCommand(
                                    sql, conexion, transaccion))
                            {
                                stmt.Parameters.AddWithValue(
                                    "@nombre", data["nombre"]);

                                stmt.Parameters.AddWithValue(
                                    "@precio", data["precio"]);

                                stmt.Parameters.AddWithValue(
                                    "@cantidad", data["cantidad"]);

                                stmt.Parameters.AddWithValue(
                                    "@imagen",
                                    data["imagen"] ?? DBNull.Value);

                                stmt.Parameters.AddWithValue(
                                    "@usuario", UsuarioActualId);

                                stmt.ExecuteNonQuery();

                                idProducto =
                                    Convert.ToInt32(stmt.LastInsertedId);
                            }

                            RegistrarAuditoria(
                                conexion,
                                transaccion,
                                idProducto,
                                "INSERTAR",
                                "Registro del producto " +
                                data["nombre"].ToString());

                            transaccion.Commit();
                            return true;
                        }
                        catch
                        {
                            transaccion.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al insertar: " + ex.Message);
                return false;
            }
        }

        // ========================================
        // MODIFICAR PRODUCTOS
        // ========================================

        public bool UpdateSeguro(
            string tbName,
            Dictionary<string, object> data,
            int id)
        {
            if (tbName != "productos" ||
                data == null || data.Count == 0 ||
                UsuarioActualId <= 0)
                return false;

            string sql =
                "UPDATE productos SET " +
                "nombre = @nombre, " +
                "precio = @precio, " +
                "cantidad = @cantidad, " +
                "imagen = @imagen, " +
                "usuario_modificacion = @usuario, " +
                "fecha_modificacion = NOW() " +
                "WHERE id = @id AND anulado = 0";

            try
            {
                using (MySqlConnection conexion = ObtenerConexion())
                {
                    if (conexion == null)
                        return false;

                    using (MySqlTransaction transaccion =
                        conexion.BeginTransaction())
                    {
                        try
                        {
                            int filas;

                            using (MySqlCommand stmt =
                                new MySqlCommand(
                                    sql, conexion, transaccion))
                            {
                                stmt.Parameters.AddWithValue(
                                    "@nombre", data["nombre"]);

                                stmt.Parameters.AddWithValue(
                                    "@precio", data["precio"]);

                                stmt.Parameters.AddWithValue(
                                    "@cantidad", data["cantidad"]);

                                stmt.Parameters.AddWithValue(
                                    "@imagen",
                                    data["imagen"] ?? DBNull.Value);

                                stmt.Parameters.AddWithValue(
                                    "@usuario", UsuarioActualId);

                                stmt.Parameters.AddWithValue(
                                    "@id", id);

                                filas = stmt.ExecuteNonQuery();
                            }

                            if (filas == 0)
                            {
                                transaccion.Rollback();
                                return false;
                            }

                            RegistrarAuditoria(
                                conexion,
                                transaccion,
                                id,
                                "MODIFICAR",
                                "Modificación del producto " +
                                data["nombre"].ToString());

                            transaccion.Commit();
                            return true;
                        }
                        catch
                        {
                            transaccion.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al actualizar: " + ex.Message);
                return false;
            }
        }

        // ========================================
        // ELIMINACIÓN LÓGICA
       

        public bool DeleteSeguro(string tbName, int id)
        {
            if (tbName != "productos" ||
                UsuarioActualId <= 0)
                return false;

            string sql =
                "UPDATE productos SET " +
                "anulado = 1, " +
                "usuario_modificacion = @usuario, " +
                "fecha_modificacion = NOW() " +
                "WHERE id = @id AND anulado = 0";

            try
            {
                using (MySqlConnection conexion = ObtenerConexion())
                {
                    if (conexion == null)
                        return false;

                    using (MySqlTransaction transaccion =
                        conexion.BeginTransaction())
                    {
                        try
                        {
                            int filas;

                            using (MySqlCommand stmt =
                                new MySqlCommand(
                                    sql, conexion, transaccion))
                            {
                                stmt.Parameters.AddWithValue(
                                    "@usuario", UsuarioActualId);

                                stmt.Parameters.AddWithValue(
                                    "@id", id);

                                filas = stmt.ExecuteNonQuery();
                            }

                            if (filas == 0)
                            {
                                transaccion.Rollback();
                                return false;
                            }

                            RegistrarAuditoria(
                                conexion,
                                transaccion,
                                id,
                                "ANULAR",
                                "Anulación del producto ID " + id);

                            transaccion.Commit();
                            return true;
                        }
                        catch
                        {
                            transaccion.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al anular: " + ex.Message);
                return false;
            }
        }

        // ========================================
        // REGISTRAR AUDITORÍA
  

        private void RegistrarAuditoria(
            MySqlConnection conexion,
            MySqlTransaction transaccion,
            int idProducto,
            string accion,
            string descripcion)
        {
            string sql =
                "INSERT INTO auditoria " +
                "(id_usuario, id_producto, accion, " +
                "descripcion, fecha_accion) " +
                "VALUES (@usuario, @producto, @accion, " +
                "@descripcion, NOW())";

            using (MySqlCommand cmd =
                new MySqlCommand(
                    sql, conexion, transaccion))
            {
                cmd.Parameters.AddWithValue(
                    "@usuario", UsuarioActualId);

                cmd.Parameters.AddWithValue(
                    "@producto", idProducto);

                cmd.Parameters.AddWithValue(
                    "@accion", accion);

                cmd.Parameters.AddWithValue(
                    "@descripcion", descripcion);

                cmd.ExecuteNonQuery();
            }
        }

        // ========================================
        // INICIO DE SESIÓN
        // ========================================

        public static int ValidarUsuario(
            string usuario, string contra)
        {
            string sql =
                "SELECT id_usuario FROM usuarios " +
                "WHERE usuario = @usuario " +
                "AND password_hash = @contra " +
                "AND activo = 1";

            using (MySqlConnection conexion = ObtenerConexion())
            {
                if (conexion == null)
                    return 0;

                using (MySqlCommand cmd =
                    new MySqlCommand(sql, conexion))
                {
                    cmd.Parameters.AddWithValue(
                        "@usuario", usuario);

                    cmd.Parameters.AddWithValue(
                        "@contra", contra);

                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null)
                    {
                        return Convert.ToInt32(resultado);
                    }
                }
            }

            return 0;
        }
    }
}
