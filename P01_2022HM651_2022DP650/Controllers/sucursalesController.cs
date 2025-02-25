// P01_2022HM651_2022DP650/Controllers/sucursalesController.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using P01_2022HM651_2022DP650.Models;
using Microsoft.EntityFrameworkCore;

namespace P01_2022HM651_2022DP650.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class sucursalesController : ControllerBase
    {
        private readonly parqueoContext _parqueoContexto;

        public sucursalesController(parqueoContext parqueoContexto)
        {
            _parqueoContexto = parqueoContexto;
        }

        [HttpGet]
        [Route("GetSucursales")]
        public IActionResult ObtenerSucursales()
        {
            var sucursales = (from s in _parqueoContexto.sucursales
                              join u in _parqueoContexto.usuarios on s.AdministradorId equals u.Id into adminGroup
                              from u in adminGroup.DefaultIfEmpty()
                              select new
                              {
                                  s.Id,
                                  s.Nombre,
                                  s.Direccion,
                                  s.Telefono,
                                  AdministradorNombre = u != null ? u.Nombre : null,
                                  s.NumEspaciosDisponibles
                              }).ToList();

            if (sucursales.Count == 0)
            {
                return NotFound("No hay sucursales");
            }
            return Ok(sucursales);
        }

        [HttpPost]
        [Route("CrearSucursal")]
        public IActionResult AddSucursal([FromBody] sucursal sucursal)
        {
            try
            {
                if (sucursal.AdministradorId.HasValue && !_parqueoContexto.usuarios.Any(u => u.Id == sucursal.AdministradorId))
                    return BadRequest("Administrador no válido");

                // Ignorar el objeto administrador anidado y usar solo AdministradorId
                sucursal.Id = 0; // Forzar que Id sea 0 para que SQL Server lo genere
                sucursal.Administrador = null; // Evitar que se intente insertar el objeto anidado
                _parqueoContexto.sucursales.Add(sucursal);
                _parqueoContexto.SaveChanges();
                return Ok("Sucursal creada");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("ActualizarSucursal/{id}")]
        public IActionResult UpdateSucursal(int id, [FromBody] sucursal sucursal)
        {
            try
            {
                sucursal? sucursalActual = (from s in _parqueoContexto.sucursales where s.Id == id select s).FirstOrDefault();
                if (sucursalActual == null)
                {
                    return NotFound("Sucursal no encontrada");
                }
                if (sucursal.AdministradorId.HasValue && !_parqueoContexto.usuarios.Any(u => u.Id == sucursal.AdministradorId))
                    return BadRequest("Administrador no válido");

                sucursalActual.Nombre = sucursal.Nombre;
                sucursalActual.Direccion = sucursal.Direccion;
                sucursalActual.Telefono = sucursal.Telefono;
                sucursalActual.AdministradorId = sucursal.AdministradorId;
                sucursalActual.NumEspaciosDisponibles = sucursal.NumEspaciosDisponibles;
                _parqueoContexto.Entry(sucursalActual).State = EntityState.Modified;
                _parqueoContexto.SaveChanges();
                return Ok("Sucursal actualizada");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("EliminarSucursal/{id}")]
        public IActionResult DeleteSucursal(int id)
        {
            try
            {
                sucursal? sucursal = (from s in _parqueoContexto.sucursales where s.Id == id select s).FirstOrDefault();
                if (sucursal == null)
                    return NotFound("Sucursal no encontrada");

                _parqueoContexto.sucursales.Attach(sucursal);
                _parqueoContexto.sucursales.Remove(sucursal);
                _parqueoContexto.SaveChanges();
                return Ok("Sucursal eliminada");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}