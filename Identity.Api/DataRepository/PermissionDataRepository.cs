using Microsoft.EntityFrameworkCore;
using Modelo.laconcordia.Modelo.Database;
using System.Linq.Expressions;

namespace Identity.Api.DataRepository
{
    public class PermissionDataRepository
    {
        #region User Permissions

        public async Task<List<UserNavigationPermission>> GetUserPermissionsAsync(string userId)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.UserNavigationPermissions
                    .Include(p => p.NavigationItem)
                    .Where(p => p.UserId == userId)
                    .ToListAsync();
            }
        }

        public async Task<UserNavigationPermission?> GetUserPermissionAsync(string userId, int navigationItemId)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.UserNavigationPermissions
                    .Include(p => p.NavigationItem)
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.NavigationItemId == navigationItemId);
            }
        }

        public async Task<UserNavigationPermission> CreateUserPermissionAsync(UserNavigationPermission permission)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                permission.GrantedAt = DateTime.Now;
                context.UserNavigationPermissions.Add(permission);
                await context.SaveChangesAsync();
                return permission;
            }
        }

        public async Task UpdateUserPermissionAsync(UserNavigationPermission permission)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                var existing = await context.UserNavigationPermissions
                    .FirstOrDefaultAsync(p => p.UserId == permission.UserId &&
                                            p.NavigationItemId == permission.NavigationItemId);

                if (existing != null)
                {
                    existing.CanView = permission.CanView;
                    existing.CanCreate = permission.CanCreate;
                    existing.CanEdit = permission.CanEdit;
                    existing.CanDelete = permission.CanDelete;
                    existing.GrantedAt = DateTime.Now;
                    existing.GrantedBy = permission.GrantedBy;

                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteUserPermissionAsync(string userId, int navigationItemId)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                var permission = await context.UserNavigationPermissions
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.NavigationItemId == navigationItemId);

                if (permission != null)
                {
                    context.UserNavigationPermissions.Remove(permission);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<List<UserNavigationPermission>> GetUserPermissionsByNavigationItemAsync(int navigationItemId)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.UserNavigationPermissions
                    .Include(p => p.User)
                    .Where(p => p.NavigationItemId == navigationItemId)
                    .ToListAsync();
            }
        }

        #endregion

        #region Role Permissions

        public async Task<List<RoleNavigationPermission>> GetRolePermissionsAsync(string roleId)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.RoleNavigationPermissions
                    .Include(p => p.NavigationItem)
                    .Where(p => p.RoleId == roleId)
                    .ToListAsync();
            }
        }

        public async Task<RoleNavigationPermission?> GetRolePermissionAsync(string roleId, int navigationItemId)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.RoleNavigationPermissions
                    .Include(p => p.NavigationItem)
                    .FirstOrDefaultAsync(p => p.RoleId == roleId && p.NavigationItemId == navigationItemId);
            }
        }

        public async Task<RoleNavigationPermission> CreateRolePermissionAsync(RoleNavigationPermission permission)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                permission.GrantedAt = DateTime.Now;
                context.RoleNavigationPermissions.Add(permission);
                await context.SaveChangesAsync();
                return permission;
            }
        }

        public async Task UpdateRolePermissionAsync(RoleNavigationPermission permission)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                var existing = await context.RoleNavigationPermissions
                    .FirstOrDefaultAsync(p => p.RoleId == permission.RoleId &&
                                            p.NavigationItemId == permission.NavigationItemId);

                if (existing != null)
                {
                    existing.CanView = permission.CanView;
                    existing.CanCreate = permission.CanCreate;
                    existing.CanEdit = permission.CanEdit;
                    existing.CanDelete = permission.CanDelete;
                    existing.GrantedAt = DateTime.Now;
                    existing.GrantedBy = permission.GrantedBy;

                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteRolePermissionAsync(string roleId, int navigationItemId)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                var permission = await context.RoleNavigationPermissions
                    .FirstOrDefaultAsync(p => p.RoleId == roleId && p.NavigationItemId == navigationItemId);

                if (permission != null)
                {
                    context.RoleNavigationPermissions.Remove(permission);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<List<RoleNavigationPermission>> GetRolePermissionsByNavigationItemAsync(int navigationItemId)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.RoleNavigationPermissions
                    .Include(p => p.Role)
                    .Where(p => p.NavigationItemId == navigationItemId)
                    .ToListAsync();
            }
        }

        #endregion

        #region Combined Queries

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.AspNetUserRoles
                    .Where(ur => ur.UserId == userId)
                    .Select(ur => ur.RoleId)
                    .ToListAsync();
            }
        }

        public async Task<List<RoleNavigationPermission>> GetRolePermissionsForUserAsync(string userId)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                var userRoles = await context.AspNetUserRoles
                    .Where(ur => ur.UserId == userId)
                    .Select(ur => ur.RoleId)
                    .ToListAsync();

                return await context.RoleNavigationPermissions
                    .Include(rp => rp.NavigationItem)
                    .Where(rp => userRoles.Contains(rp.RoleId))
                    .ToListAsync();
            }
        }

        public async Task<List<NavigationItem>> GetAllNavigationItemsAsync()
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.NavigationItems
                    .Where(ni => ni.IsActive == true)
                    .OrderBy(ni => ni.Order)
                    .ToListAsync();
            }
        }

        public async Task<NavigationItem?> GetNavigationItemByIdAsync(int id)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.NavigationItems
                    .FirstOrDefaultAsync(ni => ni.Id == id);
            }
        }

        public async Task<AspNetUser?> GetUserByIdAsync(string userId)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.AspNetUsers
                    .Include(u => u.AspNetUserRoles)
                        .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId);
            }
        }

        public async Task<AspNetRole?> GetRoleByIdAsync(string roleId)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.AspNetRoles
                    .FirstOrDefaultAsync(r => r.Id == roleId);
            }
        }

        public async Task<List<AspNetRole>> GetAllRolesAsync()
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.AspNetRoles.ToListAsync();
            }
        }

        public async Task<List<AspNetUser>> GetUsersInRoleAsync(string roleId)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.AspNetUserRoles
                    .Where(ur => ur.RoleId == roleId)
                    .Select(ur => ur.User)
                    .ToListAsync();
            }
        }

        #endregion

        #region Reports

        public async Task<int> CountUserPermissionsForNavigationItemAsync(int navigationItemId)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.UserNavigationPermissions
                    .CountAsync(up => up.NavigationItemId == navigationItemId && up.CanView);
            }
        }

        public async Task<int> CountRolePermissionsForNavigationItemAsync(int navigationItemId)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.RoleNavigationPermissions
                    .CountAsync(rp => rp.NavigationItemId == navigationItemId && rp.CanView);
            }
        }

        public async Task<List<AspNetUser>> GetAllUsersWithPermissionsAsync()
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return await context.AspNetUsers
                    .Include(u => u.UserNavigationPermissionUsers)
                        .ThenInclude(up => up.NavigationItem)
                    .Include(u => u.AspNetUserRoles)
                        .ThenInclude(ur => ur.Role)
                    .Where(u => u.UserNavigationPermissionUsers.Any())
                    .ToListAsync();
            }
        }

        #endregion
    }
}