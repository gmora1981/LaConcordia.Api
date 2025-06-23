using Identity.Api.Model.DTOs;

namespace Identity.Api.Interfaces
{
    public interface IAdvancedPermissionService
    {
        Task<bool> CopyPermissionsFromUserToUserAsync(string sourceUserId, string targetUserId, string grantedBy);
        Task<bool> CopyPermissionsFromRoleToRoleAsync(string sourceRoleId, string targetRoleId, string grantedBy);
        Task<bool> ResetUserPermissionsAsync(string userId);
        Task<bool> ResetRolePermissionsAsync(string roleId);
        Task<PermissionAuditDto> GetPermissionAuditAsync(DateTime startDate, DateTime endDate);
        Task<PermissionMatrixDto> GetPermissionMatrixAsync();
    }
}