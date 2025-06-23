using Identity.Api.Interfaces;
using Identity.Api.Model.DTO;
using Identity.Api.Model.DTO.PermissionDTOs;
using Identity.Api.Model.DTOs;
using Identity.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Identity.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly IAdvancedPermissionService _advancedPermissionService;
        private readonly ILogger<PermissionsController> _logger;

        public PermissionsController(
            IPermissionService permissionService,
            IAdvancedPermissionService advancedPermissionService,
            ILogger<PermissionsController> logger)
        {
            _permissionService = permissionService;
            _advancedPermissionService = advancedPermissionService;
            _logger = logger;
        }

        #region User Permissions

        /// <summary>
        /// Obtiene todos los permisos de un usuario específico
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Permisos del usuario con estructura de árbol</returns>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserPermissionsDto>> GetUserPermissions(string userId)
        {
            try
            {
                var permissions = await _permissionService.GetUserPermissionsAsync(userId);
                return Ok(permissions);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Usuario no encontrado: {UserId}", userId);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener permisos del usuario {UserId}", userId);
                return StatusCode(500, new { error = "Error al obtener permisos del usuario" });
            }
        }

        /// <summary>
        /// Obtiene los permisos del usuario actual
        /// </summary>
        /// <returns>Permisos del usuario autenticado</returns>
        [HttpGet("my-permissions")]
        public async Task<ActionResult<UserPermissionsDto>> GetMyPermissions()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { error = "No se pudo identificar al usuario" });
                }

                var permissions = await _permissionService.GetUserPermissionsAsync(userId);
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener mis permisos");
                return StatusCode(500, new { error = "Error al obtener permisos" });
            }
        }

        /// <summary>
        /// Obtiene el menú de navegación del usuario actual con permisos
        /// </summary>
        /// <returns>Estructura de menú filtrada por permisos</returns>
        [HttpGet("my-navigation-menu")]
        public async Task<ActionResult<NavigationMenuDto>> GetMyNavigationMenu()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { error = "No se pudo identificar al usuario" });
                }

                var menu = await _permissionService.GetUserNavigationMenuAsync(userId);
                return Ok(menu);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener menú de navegación");
                return StatusCode(500, new { error = "Error al obtener menú" });
            }
        }

        /// <summary>
        /// Obtiene el permiso de un usuario para un item específico
        /// </summary>
        [HttpGet("user/{userId}/item/{navigationItemId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<NavigationPermissionDto>> GetUserPermissionForItem(string userId, int navigationItemId)
        {
            try
            {
                var permission = await _permissionService.GetUserPermissionForItemAsync(userId, navigationItemId);
                return Ok(permission);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener permiso del usuario");
                return StatusCode(500, new { error = "Error al obtener permiso" });
            }
        }

        /// <summary>
        /// Actualiza los permisos de un usuario
        /// </summary>
        [HttpPost("user/update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserPermission([FromBody] UpdateUserPermissionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var grantedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _permissionService.UpdateUserPermissionAsync(dto, grantedBy);

                _logger.LogInformation("Permisos actualizados para usuario {UserId} en item {ItemId} por {GrantedBy}",
                    dto.UserId, dto.NavigationItemId, grantedBy);

                return Ok(new
                {
                    success = true,
                    message = "Permisos actualizados correctamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar permisos del usuario");
                return StatusCode(500, new { error = "Error al actualizar permisos" });
            }
        }

        /// <summary>
        /// Elimina los permisos de un usuario para un item específico
        /// </summary>
        [HttpDelete("user/{userId}/item/{navigationItemId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveUserPermission(string userId, int navigationItemId)
        {
            try
            {
                await _permissionService.RemoveUserPermissionAsync(userId, navigationItemId);

                _logger.LogInformation("Permisos eliminados para usuario {UserId} en item {ItemId}",
                    userId, navigationItemId);

                return Ok(new
                {
                    success = true,
                    message = "Permisos eliminados correctamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permisos del usuario");
                return StatusCode(500, new { error = "Error al eliminar permisos" });
            }
        }

        /// <summary>
        /// Asigna permisos masivos a un usuario
        /// </summary>
        [HttpPost("user/{userId}/bulk-assign")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignBulkUserPermissions(string userId, [FromBody] BulkPermissionAssignmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var grantedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _permissionService.AssignBulkUserPermissionsAsync(userId, dto, grantedBy);

                _logger.LogInformation("Permisos masivos asignados a usuario {UserId} para {Count} items por {GrantedBy}",
                    userId, dto.NavigationItemIds.Count, grantedBy);

                return Ok(new
                {
                    success = true,
                    message = $"Permisos asignados a {dto.NavigationItemIds.Count} items",
                    itemsAffected = dto.NavigationItemIds.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar permisos masivos");
                return StatusCode(500, new { error = "Error al asignar permisos" });
            }
        }

        #endregion

        #region Role Permissions

        /// <summary>
        /// Obtiene todos los permisos de un rol
        /// </summary>
        [HttpGet("role/{roleId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RolePermissionsDto>> GetRolePermissions(string roleId)
        {
            try
            {
                var permissions = await _permissionService.GetRolePermissionsAsync(roleId);
                return Ok(permissions);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener permisos del rol {RoleId}", roleId);
                return StatusCode(500, new { error = "Error al obtener permisos del rol" });
            }
        }

        /// <summary>
        /// Obtiene el permiso de un rol para un item específico
        /// </summary>
        [HttpGet("role/{roleId}/item/{navigationItemId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<NavigationPermissionDto>> GetRolePermissionForItem(string roleId, int navigationItemId)
        {
            try
            {
                var permission = await _permissionService.GetRolePermissionForItemAsync(roleId, navigationItemId);
                return Ok(permission);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener permiso del rol");
                return StatusCode(500, new { error = "Error al obtener permiso" });
            }
        }

        /// <summary>
        /// Actualiza los permisos de un rol
        /// </summary>
        [HttpPost("role/update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRolePermission([FromBody] UpdateRolePermissionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var grantedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _permissionService.UpdateRolePermissionAsync(dto, grantedBy);

                _logger.LogInformation("Permisos actualizados para rol {RoleId} en item {ItemId} por {GrantedBy}",
                    dto.RoleId, dto.NavigationItemId, grantedBy);

                return Ok(new
                {
                    success = true,
                    message = "Permisos del rol actualizados correctamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar permisos del rol");
                return StatusCode(500, new { error = "Error al actualizar permisos" });
            }
        }

        /// <summary>
        /// Elimina los permisos de un rol para un item específico
        /// </summary>
        [HttpDelete("role/{roleId}/item/{navigationItemId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveRolePermission(string roleId, int navigationItemId)
        {
            try
            {
                await _permissionService.RemoveRolePermissionAsync(roleId, navigationItemId);

                _logger.LogInformation("Permisos eliminados para rol {RoleId} en item {ItemId}",
                    roleId, navigationItemId);

                return Ok(new
                {
                    success = true,
                    message = "Permisos del rol eliminados correctamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permisos del rol");
                return StatusCode(500, new { error = "Error al eliminar permisos" });
            }
        }

        /// <summary>
        /// Asigna permisos masivos a un rol
        /// </summary>
        [HttpPost("role/{roleId}/bulk-assign")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignBulkRolePermissions(string roleId, [FromBody] BulkPermissionAssignmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var grantedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _permissionService.AssignBulkRolePermissionsAsync(roleId, dto, grantedBy);

                _logger.LogInformation("Permisos masivos asignados a rol {RoleId} para {Count} items por {GrantedBy}",
                    roleId, dto.NavigationItemIds.Count, grantedBy);

                return Ok(new
                {
                    success = true,
                    message = $"Permisos asignados a {dto.NavigationItemIds.Count} items",
                    itemsAffected = dto.NavigationItemIds.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar permisos masivos al rol");
                return StatusCode(500, new { error = "Error al asignar permisos" });
            }
        }

        #endregion

        #region Permission Checking

        /// <summary>
        /// Verifica si un usuario tiene un permiso específico
        /// </summary>
        [HttpPost("check")]
        public async Task<ActionResult<bool>> CheckPermission([FromBody] CheckPermissionDto dto)
        {
            try
            {
                // Si no se especifica usuario, usar el actual
                if (string.IsNullOrEmpty(dto.UserId))
                {
                    dto.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                }

                var hasPermission = await _permissionService.CheckPermissionAsync(dto);

                return Ok(new
                {
                    hasPermission,
                    userId = dto.UserId,
                    navigationItemId = dto.NavigationItemId,
                    permissionType = dto.PermissionType
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar permiso");
                return StatusCode(500, new { error = "Error al verificar permiso" });
            }
        }

        /// <summary>
        /// Obtiene los permisos efectivos de un usuario (combinando roles y permisos directos)
        /// </summary>
        [HttpGet("user/{userId}/effective")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<NavigationPermissionDto>>> GetEffectivePermissions(string userId)
        {
            try
            {
                var permissions = await _permissionService.GetEffectivePermissionsAsync(userId);
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener permisos efectivos");
                return StatusCode(500, new { error = "Error al obtener permisos efectivos" });
            }
        }

        #endregion

        #region Management

        /// <summary>
        /// Obtiene todos los roles disponibles
        /// </summary>
        [HttpGet("roles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<RoleDTO>>> GetAllRoles()
        {
            try
            {
                var roles = await _permissionService.GetAllRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener roles");
                return StatusCode(500, new { error = "Error al obtener roles" });
            }
        }

        /// <summary>
        /// Obtiene los usuarios que pertenecen a un rol específico
        /// </summary>
        [HttpGet("role/{roleId}/users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserDTO>>> GetUsersInRole(string roleId)
        {
            try
            {
                var users = await _permissionService.GetUsersInRoleAsync(roleId);
                return Ok(new
                {
                    roleId,
                    userCount = users.Count,
                    users
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios del rol");
                return StatusCode(500, new { error = "Error al obtener usuarios" });
            }
        }

        #endregion

        #region Reports

        /// <summary>
        /// Obtiene un reporte de items de navegación con conteo de permisos
        /// </summary>
        [HttpGet("report/navigation-items")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<NavigationPermissionDto>>> GetNavigationItemsReport()
        {
            try
            {
                var report = await _permissionService.GetAllNavigationItemsWithPermissionCountAsync();
                return Ok(new
                {
                    generatedAt = DateTime.UtcNow,
                    itemCount = report.Count,
                    items = report
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de navegación");
                return StatusCode(500, new { error = "Error al generar reporte" });
            }
        }

        /// <summary>
        /// Obtiene un reporte de todos los usuarios con sus permisos
        /// </summary>
        [HttpGet("report/users-permissions")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserPermissionsDto>>> GetUsersPermissionsReport()
        {
            try
            {
                var report = await _permissionService.GetAllUsersWithPermissionsAsync();
                return Ok(new
                {
                    generatedAt = DateTime.UtcNow,
                    userCount = report.Count,
                    users = report
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de usuarios");
                return StatusCode(500, new { error = "Error al generar reporte" });
            }
        }

        #endregion

        #region Advanced Operations

        /// <summary>
        /// Copia permisos de un usuario a otro
        /// </summary>
        [HttpPost("copy/user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CopyUserPermissions([FromBody] CopyPermissionsDto dto)
        {
            if (!ModelState.IsValid || dto.EntityType != "User")
            {
                return BadRequest("Datos inválidos");
            }

            try
            {
                var grantedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var success = await _advancedPermissionService.CopyPermissionsFromUserToUserAsync(
                    dto.SourceId, dto.TargetId, grantedBy);

                if (success)
                {
                    _logger.LogInformation("Permisos copiados de usuario {Source} a {Target} por {GrantedBy}",
                        dto.SourceId, dto.TargetId, grantedBy);

                    return Ok(new
                    {
                        success = true,
                        message = "Permisos copiados exitosamente"
                    });
                }

                return BadRequest(new { error = "No se pudieron copiar los permisos" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al copiar permisos de usuario");
                return StatusCode(500, new { error = "Error al copiar permisos" });
            }
        }

        /// <summary>
        /// Copia permisos de un rol a otro
        /// </summary>
        [HttpPost("copy/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CopyRolePermissions([FromBody] CopyPermissionsDto dto)
        {
            if (!ModelState.IsValid || dto.EntityType != "Role")
            {
                return BadRequest("Datos inválidos");
            }

            try
            {
                var grantedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var success = await _advancedPermissionService.CopyPermissionsFromRoleToRoleAsync(
                    dto.SourceId, dto.TargetId, grantedBy);

                if (success)
                {
                    _logger.LogInformation("Permisos copiados de rol {Source} a {Target} por {GrantedBy}",
                        dto.SourceId, dto.TargetId, grantedBy);

                    return Ok(new
                    {
                        success = true,
                        message = "Permisos copiados exitosamente"
                    });
                }

                return BadRequest(new { error = "No se pudieron copiar los permisos" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al copiar permisos de rol");
                return StatusCode(500, new { error = "Error al copiar permisos" });
            }
        }

        /// <summary>
        /// Resetea todos los permisos de un usuario
        /// </summary>
        [HttpDelete("reset/user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetUserPermissions(string userId)
        {
            try
            {
                var success = await _advancedPermissionService.ResetUserPermissionsAsync(userId);

                if (success)
                {
                    _logger.LogWarning("Permisos reseteados para usuario {UserId}", userId);

                    return Ok(new
                    {
                        success = true,
                        message = "Todos los permisos del usuario han sido eliminados"
                    });
                }

                return BadRequest(new { error = "No se pudieron resetear los permisos" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al resetear permisos de usuario");
                return StatusCode(500, new { error = "Error al resetear permisos" });
            }
        }

        /// <summary>
        /// Resetea todos los permisos de un rol
        /// </summary>
        [HttpDelete("reset/role/{roleId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetRolePermissions(string roleId)
        {
            try
            {
                var success = await _advancedPermissionService.ResetRolePermissionsAsync(roleId);

                if (success)
                {
                    _logger.LogWarning("Permisos reseteados para rol {RoleId}", roleId);

                    return Ok(new
                    {
                        success = true,
                        message = "Todos los permisos del rol han sido eliminados"
                    });
                }

                return BadRequest(new { error = "No se pudieron resetear los permisos" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al resetear permisos de rol");
                return StatusCode(500, new { error = "Error al resetear permisos" });
            }
        }

        /// <summary>
        /// Obtiene la matriz de permisos (roles vs items de navegación)
        /// </summary>
        [HttpGet("matrix")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PermissionMatrixDto>> GetPermissionMatrix()
        {
            try
            {
                var matrix = await _advancedPermissionService.GetPermissionMatrixAsync();
                return Ok(matrix);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar matriz de permisos");
                return StatusCode(500, new { error = "Error al generar matriz" });
            }
        }

        /// <summary>
        /// Obtiene auditoría de cambios de permisos
        /// </summary>
        [HttpGet("audit")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PermissionAuditDto>> GetPermissionAudit(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.UtcNow.AddDays(-30);
                var end = endDate ?? DateTime.UtcNow;

                var audit = await _advancedPermissionService.GetPermissionAuditAsync(start, end);

                return Ok(new
                {
                    audit,
                    totalChanges = audit.UserPermissionChanges.Count + audit.RolePermissionChanges.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener auditoría");
                return StatusCode(500, new { error = "Error al obtener auditoría" });
            }
        }

        #endregion

        #region Health Check

        /// <summary>
        /// Verifica el estado del sistema de permisos
        /// </summary>
        [HttpGet("health")]
        [AllowAnonymous]
        public ActionResult GetHealth()
        {
            return Ok(new
            {
                status = "healthy",
                service = "PermissionService",
                timestamp = DateTime.UtcNow,
                version = "3.0"
            });
        }

        #endregion
    }
}