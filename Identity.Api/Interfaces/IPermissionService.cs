using Identity.Api.Model.DTO;
using Identity.Api.Model.DTO.PermissionDTOs;


namespace Identity.Api.Interfaces
{
    public interface IPermissionService
    {
        // Permisos de Usuario
        Task<UserPermissionsDto> GetUserPermissionsAsync(string userId);
        Task<NavigationPermissionDto> GetUserPermissionForItemAsync(string userId, int navigationItemId);
        Task UpdateUserPermissionAsync(UpdateUserPermissionDto dto, string grantedBy);
        Task RemoveUserPermissionAsync(string userId, int navigationItemId);
        Task AssignBulkUserPermissionsAsync(string userId, BulkPermissionAssignmentDto dto, string grantedBy);

        // Permisos de Rol
        Task<RolePermissionsDto> GetRolePermissionsAsync(string roleId);
        Task<NavigationPermissionDto> GetRolePermissionForItemAsync(string roleId, int navigationItemId);
        Task UpdateRolePermissionAsync(UpdateRolePermissionDto dto, string grantedBy);
        Task RemoveRolePermissionAsync(string roleId, int navigationItemId);
        Task AssignBulkRolePermissionsAsync(string roleId, BulkPermissionAssignmentDto dto, string grantedBy);

        // Permisos Efectivos (combina usuario + roles)
        Task<List<NavigationPermissionDto>> GetEffectivePermissionsAsync(string userId);
        Task<NavigationMenuDto> GetUserNavigationMenuAsync(string userId);
        Task<bool> CheckPermissionAsync(CheckPermissionDto dto);

        // Gestión de Roles
        Task<List<RoleDTO>> GetAllRolesAsync();
        Task<List<UserDTO>> GetUsersInRoleAsync(string roleId);

        // Reportes
        Task<List<NavigationPermissionDto>> GetAllNavigationItemsWithPermissionCountAsync();
        Task<List<UserPermissionsDto>> GetAllUsersWithPermissionsAsync();
    }
}