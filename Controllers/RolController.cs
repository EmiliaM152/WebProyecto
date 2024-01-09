using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactVentas.Models;

namespace ReactVentas.Controllers
{
    // Definición del controlador para gestionar roles
    [Route("api/[controller]")]
    [ApiController]
    public class RolController : ControllerBase
    {
        // Definición del controlador para gestionar roles
        private readonly DBREACT_VENTAContext _context;
        // Constructor que recibe el contexto de la base de datos a través de la inyección de dependencias
        public RolController(DBREACT_VENTAContext context)
        {
            _context = context;
        }

        // Acción para obtener la lista de roles
        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            // Lista para almacenar los roles
            List<Rol> lista = new List<Rol>();
            try
            {
                // Obtener la lista de roles desde la base de datos
                lista = await _context.Rols.ToListAsync();

                return StatusCode(StatusCodes.Status200OK, lista);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, lista);
            }
        }

    }
}