using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactVentas.Models;

namespace ReactVentas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly DBREACT_VENTAContext _context;

        // Constructor que recibe el contexto de la base de datos mediante inyección de dependencias.
        public CategoriaController(DBREACT_VENTAContext context)
        {
            _context = context;

        }

        // Método para obtener la lista de categorías (HTTP GET).
        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            List<Categoria> lista = new List<Categoria>();
            try
            {
                // Obtener la lista de categorías ordenadas por IdCategoria en orden descendente.

                lista = await _context.Categoria.OrderByDescending(c => c.IdCategoria).ToListAsync();

                // Devolver la lista con un código de estado 200 (OK) en caso de éxito.
                return StatusCode(StatusCodes.Status200OK, lista);
            }
            catch (Exception ex)
            {
                // Devolver un código de estado 500 (Internal Server Error) en caso de error.
                return StatusCode(StatusCodes.Status500InternalServerError, lista);
            }
        }

        // Método para agregar una nueva categoría (HTTP POST).
        [HttpPost]
        [Route("Guardar")]
        public async Task<IActionResult> Guardar([FromBody] Categoria request)
        {
            try
            {
                // Agregar la nueva categoría al contexto.
                await _context.Categoria.AddAsync(request);

                // Guardar los cambios en la base de datos.
                await _context.SaveChangesAsync();

                // Devolver un código de estado 200 (OK) en caso de éxito.
                return StatusCode(StatusCodes.Status200OK, "ok");
            }
            catch (Exception ex)
            {
                // Devolver un código de estado 500 (Internal Server Error) con el mensaje de error en caso de fallo.
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // Método para editar una categoría existente (HTTP PUT).
        [HttpPut]
        [Route("Editar")]
        public async Task<IActionResult> Editar([FromBody] Categoria request)
        {
            try
            {
                // Marcar la categoría como modificada en el contexto.
                _context.Categoria.Update(request);

                // Guardar los cambios en la base de datos.
                await _context.SaveChangesAsync();

                // Devolver un código de estado 200 (OK) en caso de éxito.
                return StatusCode(StatusCodes.Status200OK, "ok");
            }
            catch (Exception ex)
            {
                // Devolver un código de estado 500 (Internal Server Error) con el mensaje de error en caso de fallo.
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // Método para eliminar una categoría por su Id (HTTP DELETE).
        [HttpDelete]
        [Route("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                // Buscar la categoría por su Id.
                Categoria categoria = _context.Categoria.Find(id);

                // Marcar la categoría como eliminada en el contexto.
                _context.Categoria.Remove(categoria);

                // Guardar los cambios en la base de datos.
                await _context.SaveChangesAsync();

                // Devolver un código de estado 200 (OK) en caso de éxito.
                return StatusCode(StatusCodes.Status200OK, "ok");
            }
            catch (Exception ex)
            {
                // Devolver un código de estado 500 (Internal Server Error) con el mensaje de error en caso de fallo.
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}