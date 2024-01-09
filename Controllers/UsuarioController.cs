using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactVentas.Models;

namespace ReactVentas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly DBREACT_VENTAContext _context;
        // Constructor que recibe el contexto de la base de datos
        public UsuarioController(DBREACT_VENTAContext context)
        {
            _context = context;

        }

        //Obtener la lista de usuarios
        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            List<Usuario> lista = new List<Usuario>();
            try
            {
                // Consulta la base de datos para obtener la lista de usuarios, incluyendo la relación con el rol
                lista = await _context.Usuarios.Include(r => r.IdRolNavigation).OrderByDescending(c => c.IdUsuario).ToListAsync();

                return StatusCode(StatusCodes.Status200OK, lista);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, lista);
            }
        }

        //Guardar un nuevo usuario
        [HttpPost]
        [Route("Guardar")]
        public async Task<IActionResult> Guardar([FromBody] Usuario request)
        {
            try
            {
                // Añade el nuevo usuario a la base de datos y guarda los cambios
                await _context.Usuarios.AddAsync(request);
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, "ok");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //Editar un usuario existente
        [HttpPut]
        [Route("Editar")]
        public async Task<IActionResult> Editar([FromBody] Usuario request)
        {
            try
            {
                // Actualiza el usuario en la base de datos y guarda los cambios
                _context.Usuarios.Update(request);
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, "ok");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //Eliminar un usuario por su ID 
        [HttpDelete]
        [Route("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                // Busca el usuario por su ID
                Usuario usuario = _context.Usuarios.Find(id);
                // Elimina el usuario de la base de datos y guarda los cambios
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, "ok");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}