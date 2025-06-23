using Identity.Api.DataRepository;
using Identity.Api.Interfaces;
using Identity.Api.Model.DTOs;

namespace Identity.Api.Services
{
    public class AdvancedPermissionService : IAdvancedPermissionService
    {
        private readonly UserPermissionDataRepository _userPermissionRepo;
        private readonly RolePermissionDataRepository _rolePermissionRepo;
        private readonly PermissionDataRepository _permissionRepo;

        public AdvancedPermissionService()
        {
            _userPermissionRepo = new UserPermissionDataRepository();
            _rolePermissionRepo = new RolePermissionDataRepository();
            _permissionRepo = new PermissionDataRepository();
        }

        public async Task<bool> CopyPermissionsFromUserToUserAsync(string sourceUserId, string targetUserId, string grantedBy)
        {
            try
            {
                await _userPermissionRepo.CopyUserPermissionsAsync(sourceUserId, targetUserId, grantedBy);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CopyPermissionsFromRoleToRoleAsync(string sourceRoleId, string targetRoleId, string grantedBy)
        {
            try
            {
                await _rolePermissionRepo.CopyRolePermissionsAsync(sourceRoleId, targetRoleId, grantedBy);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ResetUserPermissionsAsync(string userId)
        {
            try
            {
                await _userPermissionRepo.BulkDeleteUserPermissionsAsync(userId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ResetRolePermissionsAsync(string roleId)
        {
            try
            {
                await _rolePermissionRepo.BulkDeleteRolePermissionsAsync(roleId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<PermissionAuditDto> GetPermissionAuditAsync(DateTime startDate, DateTime endDate)
        {
            var userPermissions = await _userPermissionRepo.GetAllUserPermissionsAsync();
            var rolePermissions = await _rolePermissionRepo.GetAllRolePermissionsAsync();

            var audit = new PermissionAuditDto
            {
                StartDate = startDate,
                EndDate = endDate,
                UserPermissionChanges = userPermissions
                    .Where(p => p.GrantedAt >= startDate && p.GrantedAt <= endDate)
                    .Select(p => new PermissionChangeDto
                    {
                        EntityId = p.UserId,
                        EntityType = "User",
                        NavigationItemId = p.NavigationItemId,
                        NavigationItemTitle = p.NavigationItem?.Title ?? "",
                        GrantedAt = p.GrantedAt,
                        GrantedBy = p.GrantedBy ?? "System"
                    }).ToList(),
                RolePermissionChanges = rolePermissions
                    .Where(p => p.GrantedAt >= startDate && p.GrantedAt <= endDate)
                    .Select(p => new PermissionChangeDto
                    {
                        EntityId = p.RoleId,
                        EntityType = "Role",
                        NavigationItemId = p.NavigationItemId,
                        NavigationItemTitle = p.NavigationItem?.Title ?? "",
                        GrantedAt = p.GrantedAt,
                        GrantedBy = p.GrantedBy ?? "System"
                    }).ToList()
            };

            return audit;
        }

        public async Task<PermissionMatrixDto> GetPermissionMatrixAsync()
        {
            var navItems = await _permissionRepo.GetAllNavigationItemsAsync();
            var roles = await _permissionRepo.GetAllRolesAsync();
            var rolePermissions = await _rolePermissionRepo.GetAllRolePermissionsAsync();

            var matrix = new PermissionMatrixDto
            {
                NavigationItems = navItems.Select(n => new NavigationItemSummaryDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    ParentId = n.ParentId
                }).ToList(),
                Roles = roles.Select(r => new RoleSummaryDto
                {
                    Id = r.Id,
                    Name = r.Name ?? ""
                }).ToList(),
                Permissions = new List<PermissionMatrixEntryDto>()
            };

            foreach (var role in roles)
            {
                foreach (var navItem in navItems)
                {
                    var permission = rolePermissions.FirstOrDefault(p =>
                        p.RoleId == role.Id && p.NavigationItemId == navItem.Id);

                    matrix.Permissions.Add(new PermissionMatrixEntryDto
                    {
                        RoleId = role.Id,
                        NavigationItemId = navItem.Id,
                        CanView = permission?.CanView ?? false,
                        CanCreate = permission?.CanCreate ?? false,
                        CanEdit = permission?.CanEdit ?? false,
                        CanDelete = permission?.CanDelete ?? false
                    });
                }
            }

            return matrix;
        }
    }
}