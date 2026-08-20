using Identity.Api.DTO;
using Identity.Api.Model;
//using Identity.Api.Modelo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountsController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Create")]

        public async Task<ActionResult<UserToken>> CreateUser([FromBody] UserInfo model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, Cedula = model.Cedula, Ruc = model.Ruc };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return await BuildToken(model);
            }
            else
            {
                return BadRequest("Username or password invalid");
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserToken>> Login([FromBody] UserInfo userInfo)
        {
            var result = await _signInManager.PasswordSignInAsync(userInfo.Email,
                userInfo.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await BuildToken(userInfo);
            }
            else
            {
                return BadRequest("Invalid login attempt");
            }
        }

        [HttpGet("RenewToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserToken>> Renew()
        {
            var email = HttpContext.User.Identity.Name;
            var identityUser = await _userManager.FindByEmailAsync(email);
            var firstName = identityUser.FirstName; // O de cualquier otra fuente válida
            var lastName = identityUser.LastName;
            var password = identityUser.PasswordHash;

            var userInfo = new UserInfo()
            {
                Email = HttpContext.User.Identity.Name,
                FirstName = firstName,
                LastName = lastName,
                Password = password

            };

            return await BuildToken(userInfo);
        }

        // Método para asignar rol Admin a un usuario
        [HttpPost("AssignAdminRole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> AssignAdminRole([FromBody] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            // Verificar si el rol Admin existe, si no, crearlo
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
            }

            var result = await _userManager.AddToRoleAsync(user, "Admin");
            if (result.Succeeded)
            {
                return Ok("Admin role assigned successfully");
            }
            else
            {
                return BadRequest("Failed to assign Admin role");
            }
        }

        // POST: api/Accounts/ChangeMyPassword
        // Cambio de contraseña del propio usuario autenticado (distinto del reseteo que un
        // Admin hace sobre otro usuario en UsersController).
        [HttpPost("ChangeMyPassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ChangeMyPassword([FromBody] ChangeMyPasswordDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Usuario no encontrado");
            }

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new { success = true, message = "Contraseña actualizada correctamente." });
            }

            var errores = string.Join("; ", result.Errors.Select(e => e.Description));
            return BadRequest(new { success = false, message = errores });
        }

        private async Task<UserToken> BuildToken(UserInfo userinfo)
        {
            // 🔥 IMPORTANTE: Obtener el usuario de la base de datos para tener el ID
            var identityUser = await _userManager.FindByEmailAsync(userinfo.Email);
            if (identityUser == null)
            {
                throw new Exception("Usuario no encontrado");
            }

            var claims = new List<Claim>()
    {
        // 🚨 CRÍTICO: Agregar el NameIdentifier claim con el ID del usuario
        new Claim(ClaimTypes.NameIdentifier, identityUser.Id),
        new Claim(ClaimTypes.Name, userinfo.Email),
        new Claim(ClaimTypes.Email, userinfo.Email),
        new Claim("myvalue", "whatever I want")
    };

            var claimsDB = await _userManager.GetClaimsAsync(identityUser);
            claims.AddRange(claimsDB);

            // IMPORTANTE: Agregar los roles del usuario como claims
            var roles = await _userManager.GetRolesAsync(identityUser);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(12);

            JwtSecurityToken token = new JwtSecurityToken(
                  issuer: null,
               audience: null,
               claims: claims,
               expires: expiration,
               signingCredentials: creds);

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
                Rol = roles.FirstOrDefault() ?? "User" // Asignar el primer rol o "User" si no hay roles
            };
        }
    }
}
