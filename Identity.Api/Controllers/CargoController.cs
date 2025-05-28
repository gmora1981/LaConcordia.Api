using Identity.Api.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CargoController : Controller
    {

        private readonly ICargo _Cargo;
        public CargoController(ICargo iCargo)
        {
            _Cargo = iCargo;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("CargoInfoAll")]
        public IActionResult Get1()
        {
            return Ok(_Cargo.CargoInfoAll);
        }
      
        [HttpPost("InsertCargo")]
        public IActionResult Create([FromBody] Cargo NewItem)
        {
            try
            {
                if (NewItem == null || !ModelState.IsValid)
                {
                    return BadRequest("Error: Envio de datos");
                }

                //continuo con el ingreso de datos
                _Cargo.InsertCargo(NewItem);

            }
            catch (Exception ex)
            {
                return BadRequest("Error:" + ex.Message);
            }

            return Ok(NewItem);
        }

        [HttpPut("UpdateCargo")]
        public IActionResult Update([FromBody] Cargo UpdItem)
        {
            try
            {
                if (UpdItem == null || !ModelState.IsValid)
                {
                    return BadRequest("Error: Envio de datos");
                }

                //continuo con el ingreso de datos
                _Cargo.UpdateCargo(UpdItem);

            }
            catch (Exception ex)
            {
                return BadRequest("Error:" + ex.Message);
            }

            return NoContent();
        }

        [HttpDelete("DeleteCargo")]
        public IActionResult Delete([FromBody] Cargo DelItem)
        {
            try
            {
                if (DelItem == null || !ModelState.IsValid)
                {
                    return BadRequest("Error: Envio de datos");
                }

                //continuo con el ingreso de datos
                _Cargo.DeleteCargo(DelItem);

            }
            catch (Exception ex)
            {
                return BadRequest("Error:" + ex.Message);
            }

            return NoContent();
        }

        [HttpDelete("DeleteCargo/{IdRegistrado}")]
        public IActionResult Delete2(int IdRegistrado)
        {
            try
            {

                //continuo con el ingreso de datos
                _Cargo.DeleteCargo2(IdRegistrado);

            }
            catch (Exception ex)
            {
                return BadRequest("Error:" + ex.Message);
            }

            return NoContent();
        }

    }
}
