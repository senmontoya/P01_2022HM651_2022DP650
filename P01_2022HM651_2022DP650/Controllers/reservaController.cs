using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P01_2022HM651_2022DP650.Models;

namespace P01_2022HM651_2022DP650.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class reservaController : ControllerBase
    {
        private readonly parqueoContext _parqueoContexto;

        public reservaController(parqueoContext restauranteContexto)
        {
            _parqueoContexto = restauranteContexto;
        }

        
        [HttpPost]
        [Route("CrearReserva")]
        public IActionResult CrearReserva([FromBody] reserva nuevaReserva)
        {
            try
            {
                var espacio = _parqueoContexto.espaciosparqueo.FirstOrDefault(e => e.Id == nuevaReserva.EspacioParqueoId && e.Estado == "Disponible");
                if (espacio == null)
                {
                    return BadRequest("El espacio no está disponible");
                }

                nuevaReserva.CostoTotal = espacio.CostoPorHora * nuevaReserva.CantidadHoras;
                nuevaReserva.Estado = "Activa";
                _parqueoContexto.reservas.Add(nuevaReserva);
                espacio.Estado = "Ocupado";
                _parqueoContexto.SaveChanges();
                return Ok("Reserva creada exitosamente");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        [HttpGet]
        [Route("ObtenerReservasUsuario/{usuarioId}")]
        public IActionResult ObtenerReservasUsuario(int usuarioId)
        {
            var reservas = (from r in _parqueoContexto.reservas
                            join u in _parqueoContexto.usuarios on r.UsuarioId equals u.Id
                            where r.UsuarioId == usuarioId && r.Estado == "Activa"
                            select new
                            {
                                r.Id,
                                r.FechaReserva,
                                r.CantidadHoras,
                                r.CostoTotal,
                                r.Estado,
                                UsuarioNombre = u.Nombre,
                                UsuarioCorreo = u.Correo
                            }).ToList();

            if (reservas.Count == 0)
            {
                return NotFound("No hay reservas activas");
            }
            return Ok(reservas);
        }

       
        [HttpPut]
        [Route("CancelarReserva/{reservaId}")]
        public IActionResult CancelarReserva(int reservaId)
        {
            try
            {
                var reserva = _parqueoContexto.reservas.FirstOrDefault(r => r.Id == reservaId);
                if (reserva == null || reserva.Estado == "Cancelada")
                {
                    return NotFound("Reserva no encontrada o ya cancelada");
                }

                reserva.Estado = "Cancelada";
                var espacio = _parqueoContexto.espaciosparqueo.FirstOrDefault(e => e.Id == reserva.EspacioParqueoId);
                if (espacio != null)
                {
                    espacio.Estado = "Disponible";
                }
                _parqueoContexto.SaveChanges();
                return Ok("Reserva cancelada exitosamente");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        [HttpGet]
        [Route("ObtenerReservasPorDia/{fecha}")]
        public IActionResult ObtenerReservasPorDia(DateTime fecha)
        {
            var reservas = (from r in _parqueoContexto.reservas
                            join e in _parqueoContexto.espaciosparqueo on r.EspacioParqueoId equals e.Id
                            where r.FechaReserva.Date == fecha.Date
                            select new
                            {
                                r.Id,
                                r.FechaReserva,
                                r.CantidadHoras,
                                r.CostoTotal,
                                r.Estado,
                                EspacioNumero = e.Numero,
                                EspacioSucursalId = e.SucursalId
                            }).ToList();

            if (reservas == null)
            {
                return NotFound("No hay reservas en esta fecha");
            }
            return Ok(reservas);
        }

        
        [HttpGet]
        [Route("ObtenerReservasPorRangoFechas/{sucursalId}/{fechaInicio}/{fechaFin}")]
        public IActionResult ObtenerReservasPorRangoFechas(int sucursalId, DateTime fechaInicio, DateTime fechaFin)
        {
            var reservas = (from r in _parqueoContexto.reservas
                            join e in _parqueoContexto.espaciosparqueo on r.EspacioParqueoId equals e.Id
                            join s in _parqueoContexto.sucursales on e.SucursalId equals s.Id
                            where e.SucursalId == sucursalId && r.FechaReserva.Date >= fechaInicio.Date && r.FechaReserva.Date <= fechaFin.Date
                            select new
                            {
                                r.Id,
                                r.FechaReserva,
                                r.CantidadHoras,
                                r.CostoTotal,
                                r.Estado,
                                EspacioNumero = e.Numero,
                                SucursalNombre = s.Nombre,
                                SucursalDireccion = s.Direccion
                            }).ToList();

            if (reservas.Count == 0)
            {
                return NotFound("No hay reservas en el rango de fechas especificado para esta sucursal");
            }
            return Ok(reservas);
        }
    }
}
