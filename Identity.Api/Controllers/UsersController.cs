using Identity.Api.Helpers;
using Identity.Api.Model.DTO;
//using Identity.Api.Modelo;
using Identity.Api.Persistence.DataBase;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Identity.Api.Model;

namespace Identity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public UsersController(ApplicationDBContext context,
            UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Users.AsQueryable();
            await HttpContext.InsertPaginationParametersInResponse(queryable, paginationDTO.RecordsPerPage);
            return await queryable.Paginate(paginationDTO)
                .Select(x => new UserDTO { Email = x.Email ?? "Unknown", UserId = x.Id, FirstName = x.FirstName, LastName = x.LastName }).ToListAsync();
        }

        [HttpGet("roles")]
        public async Task<ActionResult<List<RoleDTO>>> Get()
        {
            return await context.Roles
                .Select(x => new RoleDTO { RoleName = x.Name ?? "Unknown" }).ToListAsync();
        }

        [HttpPost("assignRole")]
        //public async Task<ActionResult> AssignRole(EditRoleDTO editRoleDTO)
        //{
        //    var user = await userManager.FindByIdAsync(editRoleDTO.UserId);
        //    await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, editRoleDTO.RoleName));
        //    return NoContent();
        //}
        public async Task<ActionResult> AssignRole(EditRoleDTO editRoleDTO)
        {
            if (editRoleDTO == null)
            {
                return BadRequest("EditRoleDTO cannot be null.");
            }

            var user = await userManager.FindByIdAsync(editRoleDTO.UserId);
            if (user == null)
            {
                return NotFound($"User with ID {editRoleDTO.UserId} not found.");
            }

            var result = await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, editRoleDTO.RoleName));
            if (!result.Succeeded)
            {
                // Aquí puedes manejar los errores, por ejemplo, devolviendo los errores específicos
                return StatusCode(500, "Failed to add role claim.");
            }

            return NoContent();
        }

        [HttpPost("removeRole")]
        //public async Task<ActionResult> RemoveRole(EditRoleDTO editRoleDTO)
        //{
        //    var user = await userManager.FindByIdAsync(editRoleDTO.UserId);
        //    await userManager.RemoveClaimAsync(user , new Claim(ClaimTypes.Role, editRoleDTO.RoleName));
        //    return NoContent();
        //}

        public async Task<ActionResult> RemoveRole(EditRoleDTO editRoleDTO)
        {
            if (editRoleDTO == null)
            {
                return BadRequest("EditRoleDTO cannot be null.");
            }
            var user = await userManager.FindByIdAsync(editRoleDTO.UserId);
            if (user == null)
            {
                return NotFound($"User with ID {editRoleDTO.UserId} not found.");
            }

            var result = await userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, editRoleDTO.RoleName));
            if (!result.Succeeded)
            {
                // Aquí puedes manejar los errores, por ejemplo, devolviendo los errores específicos
                return StatusCode(500, "Failed to add role claim.");
            }
            return NoContent();
        }

    }
}
