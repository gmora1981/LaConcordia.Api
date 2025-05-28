using Identity.Api.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MenuInfoController : Controller
    {
        private readonly IMenuInfo _MenuInfo;
        public MenuInfoController(IMenuInfo iMenuInfo)
        {
            _MenuInfo = iMenuInfo;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("MenuInfoAll")]
        public IActionResult Get1()
        {
            return Ok(_MenuInfo.ListMenuInfoAll);
        }
    }
}
