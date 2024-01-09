using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ReactVentas.Models;
using ReactVentas.Models.DTO;
using System.Data;
using System.Globalization;
using System.Xml.Linq;

namespace ReactVentas.Controllers
{
    // Definimos la ruta base y especificamos que esta clase es un controlador de API
    [Route("api/[controller]")]
    [ApiController]
    public class VentaController : ControllerBase
    {
        // Se declaro una variable privada para el contexto de la base de datos
        private readonly DBREACT_VENTAContext _context;
        // Se define el constructor del controlador, que recibe el contexto de la base de datos
        public VentaController(DBREACT_VENTAContext context)
        {
            _context = context;

        }
        //Método GET para buscar productos
        [HttpGet]
        [Route("Productos/{busqueda}")]

        //busca productos según una cadena de búsqueda especificada.
        public async Task<IActionResult> Productos(string busqueda)
        {
            //lista para almacenar los productos encontrados
            List<DtoProducto> lista = new List<DtoProducto>();
            try
            {
                // Buscamos en la base de datos los productos que coincidan con la búsqueda
                lista = await _context.Productos
                // Filtra los productos en la base de datos, buscando coincidencias en código, marca y descripción, ignorando mayúsculas y minúsculas
                .Where(p => string.Concat(p.Codigo.ToLower(), p.Marca.ToLower(), p.Descripcion.ToLower()).Contains(busqueda.ToLower()))
                // Proyecta los resultados a un objeto de transferencia de datos (DTO) de producto
                .Select(p => new DtoProducto()
                {
                    // Mapea los atributos del modelo Producto a un objeto de transferencia de datos (DTO)
                    IdProducto = p.IdProducto,
                    Codigo = p.Codigo,
                    Marca = p.Marca,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio
                }).ToListAsync();

                //Si la búsqueda es exitosa, retornamos un código 200 con la lista de productos
                return StatusCode(StatusCodes.Status200OK, lista);
            }
            catch (Exception ex)
            {
                // Si ocurre un error, retornamos un código 500 con la lista vacía
                return StatusCode(StatusCodes.Status500InternalServerError, lista);
            }
        }

        [HttpPost]
        [Route("Registrar")]

        //Este método sincrónico maneja solicitudes HTTP para el registro de ventas.
        public IActionResult Registrar([FromBody] DtoVenta request)
        {
            try
            {
                // Inicializamos una variable para almacenar el número de documento
                string numeroDocumento = "";

                // Creamos un elemento XML para los productos
                XElement productos = new XElement("Productos");  //permite definir y almacenar datos de forma compartible

                // Iteramos a través de cada objeto DtoProducto en la lista de productos proporcionada en la solicitud
                foreach (DtoProducto item in request.listaProductos)
                {
                    // Para cada objeto DtoProducto, creamos un nuevo elemento XML llamado "Item" y lo añadimos al elemento "Productos"
                    productos.Add(new XElement("Item",
                        // Añadimos subelementos como "IdProducto", "Cantidad", "Precio" y "Total" con los valores correspondientes del objeto DtoProducto
                        new XElement("IdProducto", item.IdProducto),
                        new XElement("Cantidad", item.Cantidad),
                        new XElement("Precio", item.Precio),
                        new XElement("Total", item.Total)
                        ));
                }

                // conexión a la base de datos
                using (SqlConnection con = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    // Abrimos la conexión
                    con.Open();
                    // Especifica el tipo de comando es un procedimiento almacenado
                    SqlCommand cmd = new SqlCommand("sp_RegistrarVenta", con);
                    // Añadimos los parámetros necesarios para el procedimiento almacenado
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("documentoCliente", SqlDbType.VarChar, 11).Value = request.documentoCliente;
                    cmd.Parameters.Add("nombreCliente", SqlDbType.VarChar, 40).Value = request.nombreCliente;
                    cmd.Parameters.Add("tipoDocumento", SqlDbType.VarChar, 50).Value = request.tipoDocumento;
                    cmd.Parameters.Add("idUsuario", SqlDbType.Int).Value = request.idUsuario;
                    cmd.Parameters.Add("subTotal", SqlDbType.Decimal).Value = request.subTotal;
                    cmd.Parameters.Add("impuestoTotal", SqlDbType.Decimal).Value = request.igv;
                    cmd.Parameters.Add("total", SqlDbType.Decimal).Value = request.total;
                    cmd.Parameters.Add("productos", SqlDbType.Xml).Value = productos.ToString();
                    cmd.Parameters.Add("nroDocumento", SqlDbType.VarChar, 6).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    // Validación de la longitud del documento del cliente
                    if (request.documentoCliente.Length < 10)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, "El documento de cliente debe tener al menos 10 dígitos");
                    }
                    // Obtenemos el número de documento del procedimiento almacenado
                    numeroDocumento = cmd.Parameters["nroDocumento"].Value.ToString();

                    // Validación de la longitud del documento del cliente nuevamente (se repitió por error en el código original)
                    if (request.documentoCliente.Length < 10)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, "El documento de cliente debe tener al menos 10 dígitos");
                    }
                }

                // Retornamos un código de estado 200 con el número de documento
                return StatusCode(StatusCodes.Status200OK, new { numeroDocumento = numeroDocumento });
            }
            catch (Exception ex)
            {
                // En caso de error, retornamos un código de estado 500 con un objeto vacío
                return StatusCode(StatusCodes.Status500InternalServerError, new { numeroDocumento = "" });
            }
        }


        [HttpGet]
        [Route("Listar")]
        public async Task<IActionResult> Listar()
        {
            // Obtener parámetros de la solicitud
            string buscarPor = HttpContext.Request.Query["buscarPor"];
            string numeroVenta = HttpContext.Request.Query["numeroVenta"];
            string fechaInicio = HttpContext.Request.Query["fechaInicio"];
            string fechaFin = HttpContext.Request.Query["fechaFin"];

            // Conversión de las fechas desde el formato específico (dd/MM/yyyy)
            DateTime _fechainicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("es-PE"));
            DateTime _fechafin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("es-PE"));

            // Lista para almacenar el historial de ventas
            List<DtoHistorialVenta> lista_venta = new List<DtoHistorialVenta>();
            try
            {
                if (buscarPor == "fecha")
                {
                    // Consulta LINQ para obtener el historial de ventas por rango de fechas
                    lista_venta = await _context.Venta
                        .Include(u => u.IdUsuarioNavigation) // Incluye información del usuario asociado a la venta
                        .Include(d => d.DetalleVenta) // Incluye información de los detalles de la venta
                        .ThenInclude(p => p.IdProductoNavigation) // Incluye información de los productos asociados a los detalles de la venta
                        .Where(v => v.FechaRegistro.Value.Date >= _fechainicio.Date && v.FechaRegistro.Value.Date <= _fechafin.Date) // Filtra por rango de fechas
                                                                                                                                     // Proyecta los resultados en un nuevo modelo de datos (DtoHistorialVenta)
                        .Select(v => new DtoHistorialVenta()
                        {
                            FechaRegistro = v.FechaRegistro.Value.ToString("dd/MM/yyyy"), // Fecha de registro de la venta
                            NumeroDocumento = v.NumeroDocumento, // Número de la venta
                            TipoDocumento = v.TipoDocumento, // Tipo de documento de la venta
                            DocumentoCliente = v.DocumentoCliente, // Documento de identidad del cliente
                            NombreCliente = v.NombreCliente, // Nombre del cliente
                            UsuarioRegistro = v.IdUsuarioNavigation.Nombre, // Usuario que registró la venta
                            SubTotal = v.SubTotal.ToString(), // Subtotal de la venta
                            Impuesto = v.ImpuestoTotal.ToString(), // Impuesto de la venta (12%)
                            Total = v.Total.ToString(), // Total de la venta
                            // Proyección de detalles de venta a un nuevo modelo de datos (DtoDetalleVenta)
                            Detalle = v.DetalleVenta.Select(d => new DtoDetalleVenta()
                            {
                                Producto = d.IdProductoNavigation.Descripcion,     // Código del producto
                                Cantidad = d.Cantidad.ToString(),     // Cantidad del producto vendido
                                Precio = d.Precio.ToString(),     // Precio unitario del producto
                                Total = d.Total.ToString()     // Importe del producto
                            }).ToList()
                        })
                        .ToListAsync();
                }
                else
                {
                    // Obtiene la lista de ventas que coincidan con el número de documento especificado.
                    lista_venta = await _context.Venta
                        .Include(u => u.IdUsuarioNavigation)  // Incluye la relación con el usuario que registró la venta.
                        .Include(d => d.DetalleVenta)  // Incluye la relación con los detalles de venta.
                        .ThenInclude(p => p.IdProductoNavigation) // Incluye la relación con el producto de cada detalle de venta.
                        .Where(v => v.NumeroDocumento == numeroVenta) // Filtra la lista para que solo contenga la venta con el número de documento especificado.
                                                                      // Convierte cada venta a un objeto DtoHistorialVenta.
                        .Select(v => new DtoHistorialVenta()
                        {
                            // Asigna las propiedades básicas de la venta al objeto DtoHistorialVenta.
                            FechaRegistro = v.FechaRegistro.Value.ToString("dd/MM/yyyy"),
                            NumeroDocumento = v.NumeroDocumento,
                            TipoDocumento = v.TipoDocumento,
                            DocumentoCliente = v.DocumentoCliente,
                            NombreCliente = v.NombreCliente,
                            UsuarioRegistro = v.IdUsuarioNavigation.Nombre,
                            SubTotal = v.SubTotal.ToString(),
                            Impuesto = v.ImpuestoTotal.ToString(),
                            Total = v.Total.ToString(),
                            // Convierte los detalles de venta a objetos DtoDetalleVenta.
                            Detalle = v.DetalleVenta.Select(d => new DtoDetalleVenta()
                            {
                                // Asigna las propiedades básicas del detalle de venta al objeto DtoDetalleVenta.
                                Producto = d.IdProductoNavigation.Descripcion,
                                Cantidad = d.Cantidad.ToString(),
                                Precio = d.Precio.ToString(),
                                Total = d.Total.ToString()
                            }).ToList()
                        }).ToListAsync();
                }

                // Retornamos un código de estado 200 con la lista de ventas
                return StatusCode(StatusCodes.Status200OK, lista_venta);
            }
            catch (Exception ex)
            {
                var str = ex.Message;
                // En caso de error, retornamos un código de estado 500 con la lista vacía
                return StatusCode(StatusCodes.Status500InternalServerError, lista_venta);
            }


        }

        [HttpGet]
        [Route("Reporte")]
        public async Task<IActionResult> Reporte()

       // public async Task<IActionResult> Reporte(string categoria, string fechaInicio, string fechaFin)
        {
            // Obtener las fechas de inicio y fin de los parámetros de la solicitud
            string fechaInicio = HttpContext.Request.Query["fechaInicio"];
            string fechaFin = HttpContext.Request.Query["fechaFin"];

            // Conversión de las fechas desde el formato específico (dd/MM/yyyy)
            DateTime _fechainicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("es-PE"));
            DateTime _fechafin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("es-PE"));

            decimal stockTotal = 0;

            // Lista para almacenar el reporte de ventas
            List<DtoReporteVenta> lista_venta = new List<DtoReporteVenta>();
            try
            {
                stockTotal = lista_venta.Sum(v => v.StockProducto);

                // Consulta LINQ para obtener los datos necesarios para el reporte
                lista_venta = (from v in _context.Venta
                               join d in _context.DetalleVenta on v.IdVenta equals d.IdVenta  // Une las tablas Venta y DetalleVenta por el campo IdVenta.
                               join p in _context.Productos on d.IdProducto equals p.IdProducto  // Une las tablas DetalleVenta y Productos por el campo IdProducto.
                               where v.FechaRegistro.Value.Date >= _fechainicio.Date && v.FechaRegistro.Value.Date <= _fechafin.Date     // Filtra las ventas por fecha.
                                                                                                                                         // (string.IsNullOrEmpty(categoria) || p.IdCategoriaNavigation.Descripcion == categoria) */// Filtrar por categoría    // Filtra las ventas por fecha.
                                                                                                                                         // Convierte cada registro a un objeto DtoReporteVenta.
                               select new DtoReporteVenta()
                               {
                                   // Asigna las propiedades básicas de la venta al objeto DtoReporteVenta.
                                   FechaRegistro = v.FechaRegistro.Value.ToString("dd/MM/yyyy"),
                                   //NumeroDocumento = v.NumeroDocumento,
                                   //TipoDocumento = v.TipoDocumento,
                                   //DocumentoCliente = v.DocumentoCliente,
                                   //NombreCliente = v.NombreCliente,
                                   // SubTotalVenta = v.SubTotal.ToString(),
                                   //ImpuestoTotalVenta = v.ImpuestoTotal.ToString(),
                                   // TotalVenta = v.Total.ToString(),
                                   // Asigna las propiedades de los detalles de venta al objeto DtoReporteVenta.
                                   Producto = p.Descripcion,
                                   //Cantidad = d.Cantidad.ToString(),
                                   //Precio = d.Precio.ToString(),
                                   //Total = d.Total.ToString()
                                   //StockProducto = d.IdProducto.Stock  // Agregar el stock del producto al detalle
                                   //StockProducto = d.IdProductoNavigation.Stock
                                   StockProducto = d.IdProductoNavigation.Stock.HasValue ? d.IdProductoNavigation.Stock.Value : 0


                               }).ToList();


                //Categoria = p.IdCategoriaNavigation.Descripcion,


                // Retornamos un código de estado 200 con el reporte de ventas
                return StatusCode(StatusCodes.Status200OK, lista_venta);
            }
            catch (Exception ex)
            {
                var str = ex.Message;
                // En caso de error, retornamos un código de estado 500 con la lista vacía
                return StatusCode(StatusCodes.Status500InternalServerError, lista_venta);
            }


        }
    }
}