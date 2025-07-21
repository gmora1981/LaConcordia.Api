﻿using Identity.Api.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ParentescoController : ControllerBase
    {
        private readonly IParentesco _parentesco;

        public ParentescoController(IParentesco parentesco)
        {
            _parentesco = parentesco;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("ParentescoInfoAll")]
        public IActionResult GetAll()
        {
            var lista = _parentesco.GetParentescoInfoAll();

            return Ok(lista);
        }

        [HttpGet("BuscarParentesco/{id}")]
        public IActionResult GetById(int id)
        {
            var item = _parentesco.GetParentescoById(id);
            if (item == null)
                return NotFound("Parentesco no encontrado.");
            return Ok(item);
        }

        [HttpPost("InsertParentesco")]
        public IActionResult Create([FromBody] Parentesco nueva)
        {
            try
            {
                _parentesco.InsertParentesco(nueva);
                return Ok("Parentesco creado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear: " + ex.Message);
            }
        }

        [HttpPut("UpdateParentesco")]
        public IActionResult Update([FromBody] Parentesco actualizada)
        {
            try
            {
                _parentesco.UpdateParentesco(actualizada);
                return Ok("Parentesco actualizado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar: " + ex.Message);
            }
        }


        [HttpDelete("DeleteParentescoById/{id}")]
        public IActionResult DeleteById(int id)
        {
            try
            {
                _parentesco.DeleteParentescoById(id);
                return Ok("Parentesco eliminado por ID correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar por ID: " + ex.Message);
            }
        }
    }

}
