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
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly ILogger<UsersController> logger;

        public UsersController(
            ApplicationDBContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<UsersController> logger)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.logger = logger;
        }

        // GET: api/users
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Users.AsQueryable();
            await HttpContext.InsertPaginationParametersInResponse(queryable, paginationDTO.RecordsPerPage);

            var users = await queryable
                .Paginate(paginationDTO)
                .Select(x => new UserDTO
                {
                    Email = x.Email ?? "Unknown",
                    UserId = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDTO>> GetById(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Usuario con ID {id} no encontrado");
            }

            return Ok(new UserDTO
            {
                UserId = user.Id,
                Email = user.Email ?? "",
                FirstName = user.FirstName,
                LastName = user.LastName
            });
        }

        // GET: api/users/search?term=xxx
        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserDTO>>> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Ok(new List<UserDTO>());
            }

            var users = await context.Users
                .Where(u => u.Email.Contains(term) ||
                           u.FirstName.Contains(term) ||
                           u.LastName.Contains(term))
                .Select(x => new UserDTO
                {
                    UserId = x.Id,
                    Email = x.Email ?? "",
                    FirstName = x.FirstName,
                    LastName = x.LastName
                })
                .Take(20)
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/users/check-email/{email}
        [HttpGet("check-email/{email}")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> CheckEmailExists(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            return Ok(user != null);
        }


        // GET: api/users/roles
        [HttpGet("roles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<RoleDTO>>> GetRoles()
        {
            var roles = await roleManager.Roles
                .Select(x => new RoleDTO { RoleName = x.Name ?? "Unknown" })
                .ToListAsync();

            return Ok(roles);
        }

        // GET: api/users/{id}/roles
        [HttpGet("{id}/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<string>>> GetUserRoles(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Usuario con ID {id} no encontrado");
            }

            var roles = await userManager.GetRolesAsync(user);
            return Ok(roles);
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateUser(string id, [FromBody] UserEditDTO userEditDTO)
        {
            if (id != userEditDTO.UserId)
            {
                return BadRequest("El ID no coincide");
            }

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Usuario con ID {id} no encontrado");
            }

            user.Email = userEditDTO.Email;
            user.UserName = userEditDTO.Email;
            user.FirstName = userEditDTO.FirstName;
            user.LastName = userEditDTO.LastName;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            logger.LogInformation($"Usuario {id} actualizado por {User.Identity?.Name}");
            return NoContent();
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Usuario con ID {id} no encontrado");
            }

            // Evitar eliminar el usuario actual
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (id == currentUserId)
            {
                return BadRequest("No puede eliminar su propio usuario");
            }

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            logger.LogInformation($"Usuario {id} eliminado por {User.Identity?.Name}");
            return NoContent();
        }

        // POST: api/users/assignRole
        [HttpPost("assignRole")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AssignRole([FromBody] EditRoleDTO editRoleDTO)
        {
            if (editRoleDTO == null)
            {
                return BadRequest("EditRoleDTO no puede ser nulo");
            }

            var user = await userManager.FindByIdAsync(editRoleDTO.UserId);
            if (user == null)
            {
                return NotFound($"Usuario con ID {editRoleDTO.UserId} no encontrado");
            }

            // Verificar si el rol existe
            var roleExists = await roleManager.RoleExistsAsync(editRoleDTO.RoleName);
            if (!roleExists)
            {
                return BadRequest($"El rol {editRoleDTO.RoleName} no existe");
            }

            // Verificar si el usuario ya tiene el rol
            var hasRole = await userManager.IsInRoleAsync(user, editRoleDTO.RoleName);
            if (hasRole)
            {
                return BadRequest($"El usuario ya tiene el rol {editRoleDTO.RoleName}");
            }

            var result = await userManager.AddToRoleAsync(user, editRoleDTO.RoleName);
            if (!result.Succeeded)
            {
                return StatusCode(500, "Error al asignar el rol");
            }

            logger.LogInformation($"Rol {editRoleDTO.RoleName} asignado a usuario {editRoleDTO.UserId} por {User.Identity?.Name}");
            return NoContent();
        }



        // POST: api/users/removeRole
        [HttpPost("removeRole")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RemoveRole([FromBody] EditRoleDTO editRoleDTO)
        {
            if (editRoleDTO == null)
            {
                return BadRequest("EditRoleDTO no puede ser nulo");
            }

            var user = await userManager.FindByIdAsync(editRoleDTO.UserId);
            if (user == null)
            {
                return NotFound($"Usuario con ID {editRoleDTO.UserId} no encontrado");
            }

            // Verificar si el usuario tiene el rol
            var hasRole = await userManager.IsInRoleAsync(user, editRoleDTO.RoleName);
            if (!hasRole)
            {
                return BadRequest($"El usuario no tiene el rol {editRoleDTO.RoleName}");
            }

            // Evitar quitar el último admin
            if (editRoleDTO.RoleName == "Admin")
            {
                var admins = await userManager.GetUsersInRoleAsync("Admin");
                if (admins.Count <= 1)
                {
                    return BadRequest("No se puede quitar el último administrador del sistema");
                }
            }

            var result = await userManager.RemoveFromRoleAsync(user, editRoleDTO.RoleName);
            if (!result.Succeeded)
            {
                return StatusCode(500, "Error al remover el rol");
            }

            logger.LogInformation($"Rol {editRoleDTO.RoleName} removido de usuario {editRoleDTO.UserId} por {User.Identity?.Name}");
            return NoContent();
        }

        // POST: api/users/{id}/change-password
        [HttpPost("{id}/change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword(string id, [FromBody] ChangePasswordDTO changePasswordDTO)
        {
            // Los usuarios solo pueden cambiar su propia contraseña a menos que sean admin
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");

            if (id != currentUserId && !isAdmin)
            {
                return Forbid("No tiene permisos para cambiar esta contraseña");
            }

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Usuario con ID {id} no encontrado");
            }

            // Si es admin cambiando contraseña de otro usuario, no validar contraseña actual
            if (isAdmin && id != currentUserId)
            {
                var removeResult = await userManager.RemovePasswordAsync(user);
                if (!removeResult.Succeeded)
                {
                    return BadRequest(removeResult.Errors);
                }

                var addResult = await userManager.AddPasswordAsync(user, changePasswordDTO.NewPassword);
                if (!addResult.Succeeded)
                {
                    return BadRequest(addResult.Errors);
                }
            }
            else
            {
                // Usuario cambiando su propia contraseña
                var result = await userManager.ChangePasswordAsync(user,
                    changePasswordDTO.CurrentPassword,
                    changePasswordDTO.NewPassword);

                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
            }

            logger.LogInformation($"Contraseña cambiada para usuario {id} por {User.Identity?.Name}");
            return NoContent();
        }

        // GET: api/users/usuarios (mantener por compatibilidad)
        [HttpGet("usuarios")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserEditDTO>>> GetUsuarios()
        {
            var users = await context.Users
                .Select(x => new UserEditDTO
                {
                    UserId = x.Id,
                    Email = x.Email ?? "",
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Password = "", // No devolver contraseñas
                    ConfirmPassword = ""
                })
                .ToListAsync();

            return Ok(users);
        }

    }
}
