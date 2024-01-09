using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactVentas.Models;
using ReactVentas.Models.DTO;
using System.Globalization;

namespace ReactVentas.Controllers
{
    // Definimos la ruta base y especificamos que esta clase es un controlador de API
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        // Se declara una variable privada para el contexto de la base de datos
        private readonly DBREACT_VENTAContext _context;

        // Se define el constructor del controlador, que recibe el contexto de la base de datos
        public ProductoController(DBREACT_VENTAContext context)
        {
            _context = context;

        }
        // Método GET para obtener la lista de productos
        [HttpGet]
        [Route("Lista")]

        // Lista para almacenar los productos
        public async Task<IActionResult> Lista()
        {
            // Se declara e inicializa una nueva lista de productos
            List<Producto> lista = new List<Producto>();
            try
            {
                // Consulta a la base de datos para obtener la lista de productos con información de la categoría asociada
                lista = await _context.Productos.Include(c => c.IdCategoriaNavigation).OrderByDescending(c => c.IdProducto).ToListAsync();

                // Retornamos un código de estado 200 con la lista de productos
                return StatusCode(StatusCodes.Status200OK, lista);
            }
            catch (Exception ex)
            {
                // En caso de error, retornamos un código de estado 500 con la lista vacía
                return StatusCode(StatusCodes.Status500InternalServerError, lista);
            }
        }

        // Método POST para guardar un nuevo producto
        [HttpPost]
        [Route("Guardar")]
        //Añadir el nuevo producto al contexto de la base de datos
        public async Task<IActionResult> Guardar([FromBody] Producto request)
        {
            try
            {
                // Añadimos el nuevo producto a la base de datos y guardamos los cambios
                await _context.Productos.AddAsync(request);
                // Guardamos los cambios en la base de datos de forma asíncrona
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, "ok");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("Editar")]

        // Método asincrónico para editar un producto, recibe un objeto Producto desde el cuerpo de la solicitud
        public async Task<IActionResult> Editar([FromBody] Producto request)
        {
            try
            {
                // Actualiza la entidad Producto con los datos proporcionados en la solicitud
                _context.Productos.Update(request);
                // Guarda los cambios en la base de datos de forma asíncrona
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, "ok");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Route("Eliminar/{id:int}")]

        // Método asincrónico para eliminar un producto, recibe el ID del producto a eliminar
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                // Busca el producto con el ID proporcionado en la base de datos
                Producto usuario = _context.Productos.Find(id);
                // Remueve la entidad Producto de la base de datos
                _context.Productos.Remove(usuario);
                await _context.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, "ok");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("Reporte")]
        public async Task<IActionResult> Reporte(int? stockMinimo = null, int? stockMaximo = null)
        {
            // Lista para almacenar el reporte de productos con stock
            List<DtoReporteStock> lista_producto = new List<DtoReporteStock>();
            try
            {
                // Consulta LINQ para obtener los datos necesarios para el reporte
                var query = _context.Productos
                    .Where(p => (stockMinimo == null || p.Stock >= stockMinimo)
                             && (stockMaximo == null || p.Stock <= stockMaximo))
                    .Select(p => new DtoReporteStock
                    {
                        Producto = p.Descripcion,
                        Stock = p.Stock ?? 0 // Se asume que Stock no puede ser nulo aquí
                    });

                lista_producto = await query.ToListAsync();

                // Calcular el total de stock
                decimal stockTotal = lista_producto.Sum(p => p.Stock);

                // Agregar el total de stock al reporte
                lista_producto.Add(new DtoReporteStock
                {
                    Producto = "Total",
                    Stock = stockTotal
                });

                // Retornamos un código de estado 200 con el reporte de productos
                return StatusCode(StatusCodes.Status200OK, lista_producto);
            }
            catch (Exception ex)
            {
                // En caso de error, retornamos un código de estado 500 con un mensaje de error
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



    }
}