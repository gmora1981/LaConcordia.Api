using Identity.Api.DataRepository;
using Identity.Api.Interfaces;
using Identity.Api.Model.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UnidadController : Controller
    {
        private readonly IUnidad _Unidad;
        public UnidadController(IUnidad iunidad)
        {
            _Unidad = iunidad;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        [HttpGet("UnidadInfoAll")]
        public IActionResult Get1()
        {
            return Ok(_Unidad.UnidadInfoAll);
        }


        [HttpGet("UnidadxUnidad")]
        public IActionResult GetUnidad(string unidad)
        {
            return Ok(_Unidad.UnidadXUnidad(unidad));
        }

        [HttpPost("InsertUnidad")]
        public IActionResult Create([FromBody] Unidad NewItem)
        {
            try
            {
                if (NewItem == null || !ModelState.IsValid)
                {
                    return BadRequest("Error: Envio de datos");
                }

                //continuo con el ingreso de datos
                _Unidad.InsertUnidad(NewItem);

            }
            catch (Exception ex)
            {
                return BadRequest("Error:" + ex.Message);
            }

            return Ok(NewItem);
        }

    }
}
