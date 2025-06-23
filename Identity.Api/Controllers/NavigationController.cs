using Identity.Api.Interfaces;
using Identity.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NavigationController : Controller
    {
        private readonly INavigation _navigation;
        public NavigationController(INavigation inavigation)
        {
            _navigation = inavigation;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NavigationItem>>> GetAll()
        {
            try
            {
                var items = await _navigation.GetAllAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("tree")]
        public async Task<ActionResult<IEnumerable<NavigationItem>>> GetTree()
        {
            try
            {
                var tree = await _navigation.GetActiveTreeAsync();
                return Ok(tree);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        [HttpGet("{id}")]
        public async Task<ActionResult<NavigationItem>> GetById(int id)
        {
            try
            {
                var item = await _navigation.GetByIdAsync(id);
                if (item == null)
                {
                    return NotFound();
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("role/{role}")]
        public async Task<ActionResult<IEnumerable<NavigationItem>>> GetByRole(string role)
        {
            try
            {
                var items = await _navigation.GetByRoleAsync(role);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<NavigationItem>> Create([FromBody] NavigationItem dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var created = await _navigation.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] NavigationItem dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _navigation.UpdateAsync(dto);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _navigation.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("{id}/move/{direction}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MoveItem(int id, string direction)
        {
            if (direction.ToLower() != "up" && direction.ToLower() != "down")
            {
                return BadRequest(new { error = "Direction must be 'up' or 'down'" });
            }

            try
            {
                await _navigation.MoveItemAsync(id, direction);
                return Ok(new { message = "Item moved successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("reorder/{parentId?}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReorderItems(int? parentId = null)
        {
            try
            {
                await _navigation.ReorderItemsAsync(parentId);
                return Ok(new { message = "Items reordered successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("next-order/{parentId?}")]
        public async Task<ActionResult<int>> GetNextOrder(int? parentId = null)
        {
            try
            {
                var nextOrder = await _navigation.GetNextOrderAsync(parentId);
                return Ok(new { nextOrder });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
