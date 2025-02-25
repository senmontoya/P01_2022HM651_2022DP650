// P01_2022HM651_2022DP650/Controllers/espaciosParqueoController.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using P01_2022HM651_2022DP650.Models;
using Microsoft.EntityFrameworkCore;

namespace P01_2022HM651_2022DP650.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class espaciosParqueoController : ControllerBase
    {
        private readonly parqueoContext _parqueoContexto;

        public espaciosParqueoController(parqueoContext parqueoContexto)
        {
            _parqueoContexto = parqueoContexto;
        }

        [HttpPost]
        [Route("CrearEspacio")]
        public IActionResult AddEspacio([FromBody] espacioparqueo espacio)
        {
            try
            {
                sucursal? sucursal = (from s in _parqueoContexto.sucursales where s.Id == espacio.SucursalId select s).FirstOrDefault();
                if (sucursal == null)
                    return BadRequest("Sucursal no encontrada");

                if (espacio.Estado != "Disponible" && espacio.Estado != "Ocupado")
                    return BadRequest("Estado debe ser 'Disponible' o 'Ocupado'");

                // Ignorar el objeto sucursal anidado y usar solo SucursalId
                espacio.Id = 0; // Forzar que Id sea 0 para que SQL Server lo genere
                espacio.Sucursal = null; // Evitar que se intente insertar el objeto anidado
                _parqueoContexto.espaciosParqueo.Add(espacio);
                if (espacio.Estado == "Disponible") sucursal.NumEspaciosDisponibles++;
                _parqueoContexto.SaveChanges();
                return Ok("Espacio creado");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetEspaciosDisponibles")]
        public IActionResult ObtenerEspaciosDisponibles()
        {
            var espacios = (from e in _parqueoContexto.espaciosParqueo
                            join s in _parqueoContexto.sucursales on e.SucursalId equals s.Id
                            where e.Estado == "Disponible"
                            select new
                            {
                                e.Id,
                                e.Numero,
                                e.Ubicacion,
                                e.CostoPorHora,
                                SucursalNombre = s.Nombre,
                                SucursalDireccion = s.Direccion
                            }).ToList();

            if (espacios.Count == 0)
            {
                return NotFound("No hay espacios disponibles");
            }
            return Ok(espacios);
        }

        [HttpPut]
        [Route("ActualizarEspacio/{id}")]
        public IActionResult UpdateEspacio(int id, [FromBody] espacioparqueo espacio)
        {
            try
            {
                espacioparqueo? espacioActual = (from e in _parqueoContexto.espaciosParqueo where e.Id == id select e).FirstOrDefault();
                if (espacioActual == null)
                {
                    return NotFound("Espacio no encontrado");
                }

                sucursal? sucursal = (from s in _parqueoContexto.sucursales where s.Id == espacio.SucursalId select s).FirstOrDefault();
                if (sucursal == null)
                    return BadRequest("Sucursal no encontrada");

                if (espacio.Estado != "Disponible" && espacio.Estado != "Ocupado")
                    return BadRequest("Estado debe ser 'Disponible' o 'Ocupado'");

                if (espacioActual.Estado != espacio.Estado)
                {
                    if (espacio.Estado == "Disponible") sucursal.NumEspaciosDisponibles++;
                    else if (espacioActual.Estado == "Disponible") sucursal.NumEspaciosDisponibles--;
                }

                espacioActual.SucursalId = espacio.SucursalId;
                espacioActual.Numero = espacio.Numero;
                espacioActual.Ubicacion = espacio.Ubicacion;
                espacioActual.CostoPorHora = espacio.CostoPorHora;
                espacioActual.Estado = espacio.Estado;
                _parqueoContexto.Entry(espacioActual).State = EntityState.Modified;
                _parqueoContexto.SaveChanges();
                return Ok("Espacio actualizado");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("EliminarEspacio/{id}")]
        public IActionResult DeleteEspacio(int id)
        {
            try
            {
                espacioparqueo? espacio = (from e in _parqueoContexto.espaciosParqueo where e.Id == id select e).FirstOrDefault();
                if (espacio == null)
                    return NotFound("Espacio no encontrado");

                sucursal? sucursal = (from s in _parqueoContexto.sucursales where s.Id == espacio.SucursalId select s).FirstOrDefault();
                if (sucursal == null)
                    return BadRequest("Sucursal no encontrada");

                if (espacio.Estado == "Disponible") sucursal.NumEspaciosDisponibles--;

                _parqueoContexto.espaciosParqueo.Attach(espacio);
                _parqueoContexto.espaciosParqueo.Remove(espacio);
                _parqueoContexto.SaveChanges();
                return Ok("Espacio eliminado");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}