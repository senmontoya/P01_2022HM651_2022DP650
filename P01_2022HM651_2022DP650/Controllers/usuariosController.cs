using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using P01_2022HM651_2022DP650.Models;
using Microsoft.EntityFrameworkCore;

namespace P01_2022HM651_2022DP650.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class usuariosController : ControllerBase
    {
        private readonly parqueoContext _parqueoContexto;

        public usuariosController(parqueoContext restauranteContexto)
        {
            _parqueoContexto = restauranteContexto;
        }

        [HttpGet]
        [Route("GetUsuarios")]
        public IActionResult ObtenerUsuarios()
        {
            List<usuario> usuarios = (from uu in _parqueoContexto.usuarios select uu).ToList();

            if (usuarios.Count == 0)
            {
                return NotFound("No hay usuarios");
            }
            return Ok(usuarios);
        }

        [HttpPost]
        [Route("CrearUsuario")]
        public IActionResult AddUsuario([FromBody] usuario usuario)
        {
            try
            {
                _parqueoContexto.usuarios.Add(usuario);
                _parqueoContexto.SaveChanges();
                return Ok("Usuario creado");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("ActualizarUsuario/{id}")]
        public IActionResult UpdateUsuario(int id, [FromBody] usuario usuario)
        {
            try
            {
               usuario? usuarioActual = (from uu in _parqueoContexto.usuarios where uu.Id == id select uu).FirstOrDefault();
                if (usuarioActual == null)
                {
                    return NotFound("Usuario no encontrado");
                }
                usuarioActual.Nombre = usuario.Nombre;
                usuarioActual.Correo = usuario.Correo;
                usuarioActual.Telefono = usuario.Telefono;
                usuarioActual.Contraseña = usuario.Contraseña;
                usuarioActual.rol = usuario.rol;
                _parqueoContexto.Entry(usuarioActual).State = EntityState.Modified;
                _parqueoContexto.SaveChanges();
                return Ok("Usuario actualizado");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("EliminarUsuario/{id}")]
        public IActionResult DeleteUsuario(int id)
        {
            try
            {
                
                usuario? usuario = (from uu in _parqueoContexto.usuarios where uu.Id == id select uu).FirstOrDefault();
                if (usuario == null)
                
                    return NotFound("Usuario no encontrado");
                _parqueoContexto.usuarios.Attach(usuario);
                _parqueoContexto.usuarios.Remove(usuario);
                _parqueoContexto.SaveChanges();
                return Ok("Usuario eliminado");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("Login/{nombre}/{pass}")]
        public IActionResult Login(string nombre, string pass)
        {
            usuario? usuarioActual = (from uu in _parqueoContexto.usuarios where uu.Nombre == nombre && uu.Contraseña == pass select uu).FirstOrDefault();
            if (usuarioActual == null)
            {
                return NotFound("Credenciales no validas");
            }
            return Ok("Credenciales validas");
        }
    }
}
