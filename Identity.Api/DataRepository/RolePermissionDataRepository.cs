using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class RolePermissionDataRepository
    {
        public async Task<List<RoleNavigationPermission>> GetAllRolePermissionsAsync()
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.RoleNavigationPermissions
                    .Include(p => p.Role)
                    .Include(p => p.NavigationItem)
                    .ToListAsync();
            }
        }

        public async Task<List<RoleNavigationPermission>> GetRolePermissionsPaginatedAsync(int page, int pageSize)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.RoleNavigationPermissions
                    .Include(p => p.Role)
                    .Include(p => p.NavigationItem)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
        }

        public async Task<bool> RoleHasAnyPermissionAsync(string roleId)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.RoleNavigationPermissions
                    .AnyAsync(p => p.RoleId == roleId && p.CanView);
            }
        }

        public async Task BulkCreateRolePermissionsAsync(List<RoleNavigationPermission> permissions)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                foreach (var permission in permissions)
                {
                    permission.GrantedAt = DateTime.Now;
                }

                context.RoleNavigationPermissions.AddRange(permissions);
                await context.SaveChangesAsync();
            }
        }

        public async Task BulkDeleteRolePermissionsAsync(string roleId)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                var permissions = await context.RoleNavigationPermissions
                    .Where(p => p.RoleId == roleId)
                    .ToListAsync();

                context.RoleNavigationPermissions.RemoveRange(permissions);
                await context.SaveChangesAsync();
            }
        }

        public async Task CopyRolePermissionsAsync(string sourceRoleId, string targetRoleId, string grantedBy)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                var sourcePermissions = await context.RoleNavigationPermissions
                    .Where(p => p.RoleId == sourceRoleId)
                    .ToListAsync();

                var newPermissions = sourcePermissions.Select(p => new RoleNavigationPermission
                {
                    RoleId = targetRoleId,
                    NavigationItemId = p.NavigationItemId,
                    CanView = p.CanView,
                    CanCreate = p.CanCreate,
                    CanEdit = p.CanEdit,
                    CanDelete = p.CanDelete,
                    GrantedAt = DateTime.Now,
                    GrantedBy = grantedBy
                }).ToList();

                context.RoleNavigationPermissions.AddRange(newPermissions);
                await context.SaveChangesAsync();
            }
        }
    }
}