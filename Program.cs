using Pre_Entrega;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

class Program
{
    static void Main(string[] args)
    {
        var listaUsuario = new List<Usuario>();
        var listaProducto = new List<Producto>();
        var listaProductoVendido = new List<ProductoVendido>();
        var listaVenta = new List<Venta>();

        SqlConnectionStringBuilder conecctionbuilder = new();
        conecctionbuilder.DataSource = "DESKTOP-AJ3GPNR";
        conecctionbuilder.InitialCatalog = "SistemaGestion";
        conecctionbuilder.IntegratedSecurity = true;
        var cs = conecctionbuilder.ConnectionString;


        using (SqlConnection conn = new SqlConnection(cs))
        {
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            Console.WriteLine("Iniciar Sesión. Ingrese su Nombre de Usuario");
            string nickname = Console.ReadLine();
            Console.WriteLine("Ingrese su Contraseña");
            string Contraseña = Console.ReadLine();
            Usuario usuario = IniciarSesion(nickname, Contraseña, cmd);



            if (string.Equals(nickname, usuario.NombreUsuario))
            {
                Console.WriteLine("---- USUARIO ----- ");
                Console.WriteLine("Id = " + usuario.Id);
                Console.WriteLine("Nombre = " + usuario.Nombre);
                Console.WriteLine("Apellido = " + usuario.Apellido);
                Console.WriteLine("Nombre Usuario = " + usuario.NombreUsuario);
                Console.WriteLine("Contraseña = " + usuario.Contraseña);
                Console.WriteLine("Mail = " + usuario.Mail);
                Console.WriteLine("--------------");
                List<Producto> p = GetProducto(usuario.Id, cmd);
                if (p.Count > 0)
                {
                    Console.WriteLine("---- PRODUCTOS ----- ");
                    foreach (var producto in p)
                    {
                        Console.WriteLine("Id = " + producto.Id);
                        Console.WriteLine("Descripciones = " + producto.Descripciones);
                        Console.WriteLine("Costo = " + producto.Costo);
                        Console.WriteLine("Precio de Venta = " + producto.PrecioVenta);
                        Console.WriteLine("Stock = " + producto.Stock);
                        Console.WriteLine("Id Usuario = " + producto.IdUsuario);
                        Console.WriteLine("--------------");
                    }
                    List<Producto> lista = GetProductoVendido(usuario.Id, cmd);
                    Console.WriteLine("---- PRODUCTOS VENDIDOS ----- ");
                    foreach (var pv in lista)
                    {
                        Console.WriteLine("Id = " + pv.Id);
                        Console.WriteLine("Descripciones = " + pv.Descripciones);
                        Console.WriteLine("Costo = " + pv.Costo);
                        Console.WriteLine("Precio de Venta = " + pv.PrecioVenta);
                        Console.WriteLine("Stock = " + pv.Stock);
                        Console.WriteLine("Id Usuario = " + pv.IdUsuario);
                        Console.WriteLine("--------------");

                    }
                    List<Venta> ventas = GetVenta(usuario.Id, cmd);
                    Console.WriteLine("---- VENTAS ----- ");
                    foreach (var venta in ventas)
                    {
                        Console.WriteLine("Id Venta = " + venta.Id);
                        Console.WriteLine("Comentarios = " + venta.Comentarios);
                        Console.WriteLine("Id Usuario = " + venta.IdUsuario);
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("No hay productos para mostrar para el usuario {0}", nickname));
                }
            }
            else
            {
                Console.WriteLine("---- USUARIO ----- ");
                Console.WriteLine("Id = " + usuario.Id);
                Console.WriteLine("Nombre = " + usuario.Nombre);
                Console.WriteLine("Apellido = " + usuario.Apellido);
                Console.WriteLine("Nombre Usuario = " + usuario.NombreUsuario);
                Console.WriteLine("Contraseña = " + usuario.Contraseña);
                Console.WriteLine("Mail = " + usuario.Mail);
                Console.WriteLine("--------------");
            }
            conn.Close();
        }
    }
    // a) TRAER USUARIO //
    public static Usuario GetUsuario(string nombre, SqlCommand cmd)
    {
        Usuario usuario = new Usuario();

        cmd.CommandText = string.Format("Select * FROM Usuario where NombreUsuario = '{0}'", nombre);
        var reader = cmd.ExecuteReader();

        if (reader.HasRows)
        {
            while (reader.Read())
            {

                usuario.Id = Convert.ToInt32(reader.GetValue(0));
                usuario.Nombre = reader.GetValue(1).ToString();
                usuario.Apellido = reader.GetValue(2).ToString();
                usuario.NombreUsuario = reader.GetValue(3).ToString();
                usuario.Contraseña = reader.GetValue(4).ToString();
                usuario.Mail = reader.GetValue(5).ToString();
            }
        }
        else
        {
            usuario = null;
        }
        reader.Close();
        return usuario;
    }
    // b) TRAER PRODUCTO //
    public static List<Producto> GetProducto(int idUsuario, SqlCommand cmd)
    {
        var listaProducto = new List<Producto>();

        cmd.CommandText = string.Format("Select* FROM Producto where IdUsuario = {0}", idUsuario);
        var reader = cmd.ExecuteReader();

        if (reader.HasRows)
        {
            while (reader.Read())
            {
                Producto producto = new Producto();

                producto.Id = Convert.ToInt32(reader.GetValue(0));
                producto.Descripciones = reader.GetValue(1).ToString();
                producto.Costo = Convert.ToDouble(reader.GetValue(2));
                producto.PrecioVenta = Convert.ToDouble(reader.GetValue(3));
                producto.Stock = Convert.ToInt32(reader.GetValue(4));
                producto.IdUsuario = Convert.ToInt32(reader.GetValue(5));
                listaProducto.Add(producto);
            }
        }
        reader.Close();
        return listaProducto;
    }

    // c) TRAER PRODUCTOS VENDIDOS //
    public static List<Producto> GetProductoVendido(int idUsuario, SqlCommand cmd)
    {
        List<Producto> productos = new List<Producto>();
        cmd.CommandText = string.Format("select p.Id, p.Descripciones,p.Costo,p.PrecioVenta, p.Stock,p.IdUsuario from ProductoVendido pv inner join Producto p on pv.IdProducto = p.Id inner join usuario u on p.idusuario = u.id where u.id = {0} order by id", idUsuario);
        var reader = cmd.ExecuteReader();


        if (reader.HasRows)
        {
            while (reader.Read())
            {
                Producto producto = new Producto();
                producto.Id = Convert.ToInt32(reader.GetValue(0));
                producto.Descripciones = reader.GetValue(1).ToString();
                producto.Costo = Convert.ToDouble(reader.GetValue(2));
                producto.PrecioVenta = Convert.ToDouble(reader.GetValue(3));
                producto.Stock = Convert.ToInt32(reader.GetValue(4));
                producto.IdUsuario = Convert.ToInt32(reader.GetValue(5));
                productos.Add(producto);

            }
        }
        reader.Close();
        return productos;
    }

    // d) TRAER VENTAS //
    public static List<Venta> GetVenta(int idUsuario, SqlCommand cmd)
    {
        List<Venta> ventas = new List<Venta>();
        cmd.CommandText = String.Format("select * from Venta where idusuario = {0}", idUsuario);
        var reader = cmd.ExecuteReader();

        if (reader.HasRows)
        {
            while (reader.Read())
            {
                Venta venta = new Venta();
                venta.Id = Convert.ToInt32(reader.GetValue(0));
                venta.Comentarios = reader.GetValue(1).ToString();
                venta.IdUsuario = Convert.ToInt32(reader.GetValue(2));
                ventas.Add(venta);
            }
        }
        reader.Close();
        return ventas;
    }

    // e) INICIAR SESIÓN //
    public static Usuario IniciarSesion(string NombreUsuario, string Contraseña, SqlCommand cmd)
    {
        Usuario usuario = new Usuario();
        cmd.CommandText = String.Format("select * from Usuario where NombreUsuario = '{0}' and Contraseña = '{1}'", NombreUsuario, Contraseña);
        var reader = cmd.ExecuteReader();

        if (reader.HasRows)
        {
            while(reader.Read())
            {
                usuario.Id = Convert.ToInt32(reader.GetValue(0));
                usuario.Nombre = reader.GetValue(1).ToString();
                usuario.Apellido = reader.GetValue(2).ToString();
                usuario.NombreUsuario = reader.GetValue(3).ToString();
                usuario.Contraseña = reader.GetValue(4).ToString();
                usuario.Mail = reader.GetValue(5).ToString();
            }
        }
        reader.Close();
        return usuario;
    }
}
