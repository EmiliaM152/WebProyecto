using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactVentas.Models;
using ReactVentas.Models.DTO;
using System.Text.RegularExpressions;

namespace ReactVentas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilidadController : ControllerBase
    {

        private readonly DBREACT_VENTAContext _context;
        public UtilidadController(DBREACT_VENTAContext context)
        {
            _context = context;

        }
        [HttpGet]
        [Route("Dashboard")]

        public async Task<IActionResult> Dashboard()
        {
            DtoDashboard config = new DtoDashboard();

            // Establece las fechas para el rango de 30 días y 7 días atrás desde la fecha actual
            DateTime fecha = DateTime.Now;
            DateTime fecha2 = DateTime.Now;
            fecha = fecha.AddDays(-30);
            fecha2 = fecha2.AddDays(-7);
            try
            {
                // Se configura el objeto DtoDashboard con la información requerida
                config.TotalVentas = _context.Venta.Where(v => v.FechaRegistro >= fecha).Count().ToString(); //fecha de registro es mayor o igual a la fecha especificada y convierte el resultado a una cadena.
                config.TotalIngresos = _context.Venta.Where(v => v.FechaRegistro >= fecha).Sum(v => v.Total).ToString(); //calcula la suma total de los montos totales de las ventas 
                config.TotalProductos = _context.Productos.Count().ToString(); //número total de productos y convierte el resultado a una cadena.
                config.TotalCategorias = _context.Categoria.Count().ToString(); //número total de categorías y convierte el resultado a una cadena.

                // Se obtiene los productos más vendidos (hasta 4 productos)

                config.ProductosVendidos = (from p in _context.Productos
                                            join d in _context.DetalleVenta on p.IdProducto equals d.IdProducto // Une la colección 'DetalleVenta' con 'Productos' basado en el 'IdProducto'.
                                            group p by p.Descripcion into g // Agrupa la colección resultante por la propiedad 'Descripcion' de cada producto. La colección agrupada es representada por 'g'.
                                            orderby g.Count() ascending // Ordena la colección agrupada en orden ascendente basado en la cantidad de elementos en cada grupo.
                                            select new DtoProductoVendidos { Producto = g.Key, Total = g.Count().ToString() }).Take(4).ToList();

                // Obtiene el total de ventas por día en el último rango de 7 días
                config.VentasporDias = (from v in _context.Venta
                                        where v.FechaRegistro.Value.Date >= fecha2.Date // Filtra las ventas cuya 'FechaRegistro' es mayor o igual a 'fecha2' (7 dias ).
                                        group v by v.FechaRegistro.Value.Date into g // Agrupa la colección resultante por la propiedad 'FechaRegistro' de cada venta. La colección agrupada es representada por 'g'.
                                        orderby g.Key ascending // Ordena la colección agrupada en orden ascendente basado en la clave de cada grupo, que es la 'FechaRegistro'.
                                        select new DtoVentasDias { Fecha = g.Key.ToString("dd/MM/yyyy"), Total = g.Count().ToString() }).ToList();

                return StatusCode(StatusCodes.Status200OK, config);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, config);
            }
        }

    }
}