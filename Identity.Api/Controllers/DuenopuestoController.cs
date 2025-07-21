﻿using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DuenopuestoController : ControllerBase
    {
        private readonly IDuenopuesto _duenoPuesto;

        public DuenopuestoController(IDuenopuesto duenopuesto)
        {
            _duenoPuesto = duenopuesto;
        }


        [HttpGet("DuenopuestoInfoAll")]
        public IActionResult GetAll()
        {
            var lista = _duenoPuesto.GetDuenopuestoInfoAll();

            return Ok(lista);
        }

        [HttpGet("GetDuenopuestoById/{cedula}")]
        public IActionResult GetById(string cedula)
        {
            var item = _duenoPuesto.GetDuenopuestoById(cedula);
            if (item == null)
                return NotFound("Dueño de puesto no encontrado.");
            return Ok(item);
        }

        [HttpPost("InsertDuenopuesto")]
        public IActionResult Create([FromBody] DuenopuestoDTO nueva)
        {
            try
            {
                _duenoPuesto.InsertDuenopuesto(nueva);
                return Ok("Dueño de puesto creado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear: " + ex.Message);
            }
        }

        [HttpPut("UpdateDuenopuesto")]
        public IActionResult Update([FromBody] DuenopuestoDTO actualizada)
        {
            try
            {
                _duenoPuesto.UpdateDuenopuesto(actualizada);
                return Ok("Dueño de puesto actualizado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar: " + ex.Message);
            }
        }

        [HttpDelete("DeleteDuenopuestoById/{cedula}")]
        public IActionResult DeleteById(string cedula)
        {
            try
            {
                _duenoPuesto.DeletePDuenopuestoById(cedula);
                return Ok("Dueño de puesto eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar: " + ex.Message);
            }

        }

    }
}
