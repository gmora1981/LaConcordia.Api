using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class UserPermissionDataRepository
    {
        public async Task<List<UserNavigationPermission>> GetAllUserPermissionsAsync()
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.UserNavigationPermissions
                    .Include(p => p.User)
                    .Include(p => p.NavigationItem)
                    .ToListAsync();
            }
        }

        public async Task<List<UserNavigationPermission>> GetUserPermissionsPaginatedAsync(int page, int pageSize)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.UserNavigationPermissions
                    .Include(p => p.User)
                    .Include(p => p.NavigationItem)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
        }

        public async Task<bool> UserHasAnyPermissionAsync(string userId)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.UserNavigationPermissions
                    .AnyAsync(p => p.UserId == userId && p.CanView);
            }
        }

        public async Task BulkCreateUserPermissionsAsync(List<UserNavigationPermission> permissions)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                foreach (var permission in permissions)
                {
                    permission.GrantedAt = DateTime.Now;
                }

                context.UserNavigationPermissions.AddRange(permissions);
                await context.SaveChangesAsync();
            }
        }

        public async Task BulkDeleteUserPermissionsAsync(string userId)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                var permissions = await context.UserNavigationPermissions
                    .Where(p => p.UserId == userId)
                    .ToListAsync();

                context.UserNavigationPermissions.RemoveRange(permissions);
                await context.SaveChangesAsync();
            }
        }

        public async Task CopyUserPermissionsAsync(string sourceUserId, string targetUserId, string grantedBy)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                var sourcePermissions = await context.UserNavigationPermissions
                    .Where(p => p.UserId == sourceUserId)
                    .ToListAsync();

                var newPermissions = sourcePermissions.Select(p => new UserNavigationPermission
                {
                    UserId = targetUserId,
                    NavigationItemId = p.NavigationItemId,
                    CanView = p.CanView,
                    CanCreate = p.CanCreate,
                    CanEdit = p.CanEdit,
                    CanDelete = p.CanDelete,
                    GrantedAt = DateTime.Now,
                    GrantedBy = grantedBy
                }).ToList();

                context.UserNavigationPermissions.AddRange(newPermissions);
                await context.SaveChangesAsync();
            }
        }
    }
}