using Identity.Api.DataRepository;
using Identity.Api.Interfaces;
using Identity.Api.Model.DTO;
using Identity.Api.Model.DTO.PermissionDTOs;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly PermissionDataRepository _dataRepository;

        public PermissionService()
        {
            _dataRepository = new PermissionDataRepository();
        }

        #region User Permissions

        public async Task<UserPermissionsDto> GetUserPermissionsAsync(string userId)
        {
            var user = await _dataRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"Usuario con ID {userId} no encontrado");

            var userPermissions = new UserPermissionsDto
            {
                UserId = user.Id,
                Username = user.UserName ?? "",
                Email = user.Email ?? "",
                Roles = user.AspNetUserRoles.Select(ur => ur.Role.Name ?? "").ToList()
            };

            var allNavigationItems = await _dataRepository.GetAllNavigationItemsAsync();

            foreach (var navItem in allNavigationItems)
            {
                var permission = await GetEffectivePermissionAsync(userId, navItem.Id);
                userPermissions.Permissions.Add(permission);
            }

            userPermissions.Permissions = BuildPermissionTree(userPermissions.Permissions);
            return userPermissions;
        }

        public async Task<NavigationPermissionDto> GetUserPermissionForItemAsync(string userId, int navigationItemId)
        {
            var permission = await _dataRepository.GetUserPermissionAsync(userId, navigationItemId);
            var navItem = await _dataRepository.GetNavigationItemByIdAsync(navigationItemId);

            if (navItem == null)
                throw new NotFoundException($"Item de navegación {navigationItemId} no encontrado");

            if (permission == null)
            {
                return new NavigationPermissionDto
                {
                    NavigationItemId = navItem.Id,
                    NavigationItemTitle = navItem.Title,
                    NavigationItemUrl = navItem.Url,
                    NavigationItemIcon = navItem.Icon,
                    ParentId = navItem.ParentId,
                    CanView = false,
                    CanCreate = false,
                    CanEdit = false,
                    CanDelete = false
                };
            }

            return MapToPermissionDto(permission);
        }

        public async Task UpdateUserPermissionAsync(UpdateUserPermissionDto dto, string grantedBy)
        {
            var existingPermission = await _dataRepository.GetUserPermissionAsync(dto.UserId, dto.NavigationItemId);

            if (existingPermission == null)
            {
                var newPermission = new UserNavigationPermission
                {
                    UserId = dto.UserId,
                    NavigationItemId = dto.NavigationItemId,
                    CanView = dto.CanView,
                    CanCreate = dto.CanCreate,
                    CanEdit = dto.CanEdit,
                    CanDelete = dto.CanDelete,
                    GrantedBy = grantedBy
                };
                await _dataRepository.CreateUserPermissionAsync(newPermission);
            }
            else
            {
                existingPermission.CanView = dto.CanView;
                existingPermission.CanCreate = dto.CanCreate;
                existingPermission.CanEdit = dto.CanEdit;
                existingPermission.CanDelete = dto.CanDelete;
                existingPermission.GrantedBy = grantedBy;
                await _dataRepository.UpdateUserPermissionAsync(existingPermission);
            }
        }

        public async Task RemoveUserPermissionAsync(string userId, int navigationItemId)
        {
            await _dataRepository.DeleteUserPermissionAsync(userId, navigationItemId);
        }

        public async Task AssignBulkUserPermissionsAsync(string userId, BulkPermissionAssignmentDto dto, string grantedBy)
        {
            foreach (var itemId in dto.NavigationItemIds)
            {
                await UpdateUserPermissionAsync(new UpdateUserPermissionDto
                {
                    UserId = userId,
                    NavigationItemId = itemId,
                    CanView = dto.CanView,
                    CanCreate = dto.CanCreate,
                    CanEdit = dto.CanEdit,
                    CanDelete = dto.CanDelete
                }, grantedBy);
            }
        }

        #endregion

        #region Role Permissions

        public async Task<RolePermissionsDto> GetRolePermissionsAsync(string roleId)
        {
            var role = await _dataRepository.GetRoleByIdAsync(roleId);
            if (role == null)
                throw new NotFoundException($"Rol con ID {roleId} no encontrado");

            var rolePermissions = new RolePermissionsDto
            {
                RoleId = role.Id,
                RoleName = role.Name ?? ""
            };

            var allNavigationItems = await _dataRepository.GetAllNavigationItemsAsync();
            var permissions = await _dataRepository.GetRolePermissionsAsync(roleId);

            foreach (var navItem in allNavigationItems)
            {
                var permission = permissions.FirstOrDefault(p => p.NavigationItemId == navItem.Id);

                rolePermissions.Permissions.Add(new NavigationPermissionDto
                {
                    NavigationItemId = navItem.Id,
                    NavigationItemTitle = navItem.Title,
                    NavigationItemUrl = navItem.Url,
                    NavigationItemIcon = navItem.Icon,
                    ParentId = navItem.ParentId,
                    CanView = permission?.CanView ?? false,
                    CanCreate = permission?.CanCreate ?? false,
                    CanEdit = permission?.CanEdit ?? false,
                    CanDelete = permission?.CanDelete ?? false
                });
            }

            rolePermissions.Permissions = BuildPermissionTree(rolePermissions.Permissions);
            return rolePermissions;
        }

        public async Task<NavigationPermissionDto> GetRolePermissionForItemAsync(string roleId, int navigationItemId)
        {
            var permission = await _dataRepository.GetRolePermissionAsync(roleId, navigationItemId);
            var navItem = await _dataRepository.GetNavigationItemByIdAsync(navigationItemId);

            if (navItem == null)
                throw new NotFoundException($"Item de navegación {navigationItemId} no encontrado");

            if (permission == null)
            {
                return new NavigationPermissionDto
                {
                    NavigationItemId = navItem.Id,
                    NavigationItemTitle = navItem.Title,
                    NavigationItemUrl = navItem.Url,
                    NavigationItemIcon = navItem.Icon,
                    ParentId = navItem.ParentId,
                    CanView = false,
                    CanCreate = false,
                    CanEdit = false,
                    CanDelete = false
                };
            }

            return MapToPermissionDto(permission);
        }

        public async Task UpdateRolePermissionAsync(UpdateRolePermissionDto dto, string grantedBy)
        {
            var existingPermission = await _dataRepository.GetRolePermissionAsync(dto.RoleId, dto.NavigationItemId);

            if (existingPermission == null)
            {
                var newPermission = new RoleNavigationPermission
                {
                    RoleId = dto.RoleId,
                    NavigationItemId = dto.NavigationItemId,
                    CanView = dto.CanView,
                    CanCreate = dto.CanCreate,
                    CanEdit = dto.CanEdit,
                    CanDelete = dto.CanDelete,
                    GrantedBy = grantedBy
                };
                await _dataRepository.CreateRolePermissionAsync(newPermission);
            }
            else
            {
                existingPermission.CanView = dto.CanView;
                existingPermission.CanCreate = dto.CanCreate;
                existingPermission.CanEdit = dto.CanEdit;
                existingPermission.CanDelete = dto.CanDelete;
                existingPermission.GrantedBy = grantedBy;
                await _dataRepository.UpdateRolePermissionAsync(existingPermission);
            }
        }

        public async Task RemoveRolePermissionAsync(string roleId, int navigationItemId)
        {
            await _dataRepository.DeleteRolePermissionAsync(roleId, navigationItemId);
        }

        public async Task AssignBulkRolePermissionsAsync(string roleId, BulkPermissionAssignmentDto dto, string grantedBy)
        {
            foreach (var itemId in dto.NavigationItemIds)
            {
                await UpdateRolePermissionAsync(new UpdateRolePermissionDto
                {
                    RoleId = roleId,
                    NavigationItemId = itemId,
                    CanView = dto.CanView,
                    CanCreate = dto.CanCreate,
                    CanEdit = dto.CanEdit,
                    CanDelete = dto.CanDelete
                }, grantedBy);
            }
        }

        #endregion

        #region Effective Permissions

        public async Task<List<NavigationPermissionDto>> GetEffectivePermissionsAsync(string userId)
        {
            var allNavigationItems = await _dataRepository.GetAllNavigationItemsAsync();
            var effectivePermissions = new List<NavigationPermissionDto>();

            foreach (var navItem in allNavigationItems)
            {
                var permission = await GetEffectivePermissionAsync(userId, navItem.Id);
                effectivePermissions.Add(permission);
            }

            return BuildPermissionTree(effectivePermissions);
        }

        public async Task<NavigationMenuDto> GetUserNavigationMenuAsync(string userId)
        {
            var effectivePermissions = await GetEffectivePermissionsAsync(userId);
            return BuildNavigationMenu(effectivePermissions);
        }

        public async Task<bool> CheckPermissionAsync(CheckPermissionDto dto)
        {
            var permission = await GetEffectivePermissionAsync(dto.UserId, dto.NavigationItemId);

            return dto.PermissionType.ToLower() switch
            {
                "view" => permission.CanView,
                "create" => permission.CanCreate,
                "edit" => permission.CanEdit,
                "delete" => permission.CanDelete,
                _ => false
            };
        }

        #endregion

        #region Role Management

        public async Task<List<RoleDTO>> GetAllRolesAsync()
        {
            var roles = await _dataRepository.GetAllRolesAsync();
            return roles.Select(r => new RoleDTO
            {
                RoleId = r.Id,           // Agregar esta línea
                RoleName = r.Name ?? ""
            }).ToList();
        }

        public async Task<AspNetRole?> GetRoleByIdAsync(string roleId)
        {
            return await _dataRepository.GetRoleByIdAsync(roleId);
        }

        public async Task<NavigationItem?> GetNavigationItemByIdAsync(int navigationItemId)
        {
            return await _dataRepository.GetNavigationItemByIdAsync(navigationItemId);
        }

        public async Task<List<UserDTO>> GetUsersInRoleAsync(string roleId)
        {
            var users = await _dataRepository.GetUsersInRoleAsync(roleId);
            return users.Select(u => new UserDTO
            {
                UserId = u.Id,
                Email = u.Email ?? "",
                FirstName = u.FirstName,
                LastName = u.LastName
            }).ToList();
        }

        #endregion

        #region Reports

        public async Task<List<NavigationPermissionDto>> GetAllNavigationItemsWithPermissionCountAsync()
        {
            var navItems = await _dataRepository.GetAllNavigationItemsAsync();
            var result = new List<NavigationPermissionDto>();

            foreach (var item in navItems)
            {
                var userCount = await _dataRepository.CountUserPermissionsForNavigationItemAsync(item.Id);
                var roleCount = await _dataRepository.CountRolePermissionsForNavigationItemAsync(item.Id);

                result.Add(new NavigationPermissionDto
                {
                    NavigationItemId = item.Id,
                    NavigationItemTitle = $"{item.Title} (Usuarios: {userCount}, Roles: {roleCount})",
                    NavigationItemUrl = item.Url,
                    NavigationItemIcon = item.Icon,
                    ParentId = item.ParentId
                });
            }

            return BuildPermissionTree(result);
        }

        public async Task<List<UserPermissionsDto>> GetAllUsersWithPermissionsAsync()
        {
            var users = await _dataRepository.GetAllUsersWithPermissionsAsync();
            var result = new List<UserPermissionsDto>();

            foreach (var user in users)
            {
                var userPerms = new UserPermissionsDto
                {
                    UserId = user.Id,
                    Username = user.UserName ?? "",
                    Email = user.Email ?? "",
                    Roles = user.AspNetUserRoles.Select(ur => ur.Role.Name ?? "").ToList()
                };

                var directPermissions = user.UserNavigationPermissionUsers
                    .Where(p => p.CanView || p.CanCreate || p.CanEdit || p.CanDelete)
                    .Select(p => new NavigationPermissionDto
                    {
                        NavigationItemId = p.NavigationItemId,
                        NavigationItemTitle = p.NavigationItem?.Title ?? "",
                        CanView = p.CanView,
                        CanCreate = p.CanCreate,
                        CanEdit = p.CanEdit,
                        CanDelete = p.CanDelete
                    })
                    .ToList();

                userPerms.Permissions = directPermissions;
                result.Add(userPerms);
            }

            return result;
        }

        #endregion

        #region Private Helper Methods

        private async Task<NavigationPermissionDto> GetEffectivePermissionAsync(string userId, int navigationItemId)
        {
            var navItem = await _dataRepository.GetNavigationItemByIdAsync(navigationItemId);
            if (navItem == null)
                throw new NotFoundException($"Item de navegación {navigationItemId} no encontrado");

            var permission = new NavigationPermissionDto
            {
                NavigationItemId = navItem.Id,
                NavigationItemTitle = navItem.Title,
                NavigationItemUrl = navItem.Url,
                NavigationItemIcon = navItem.Icon,
                ParentId = navItem.ParentId,
                CanView = false,
                CanCreate = false,
                CanEdit = false,
                CanDelete = false
            };

            // Primero, verificar permisos por rol
            var rolePermissions = await _dataRepository.GetRolePermissionsForUserAsync(userId);
            var relevantRolePermissions = rolePermissions.Where(rp => rp.NavigationItemId == navigationItemId);

            foreach (var rp in relevantRolePermissions)
            {
                permission.CanView = permission.CanView || rp.CanView;
                permission.CanCreate = permission.CanCreate || rp.CanCreate;
                permission.CanEdit = permission.CanEdit || rp.CanEdit;
                permission.CanDelete = permission.CanDelete || rp.CanDelete;
            }

            // Luego, aplicar permisos específicos del usuario (sobrescriben los del rol)
            var userPermission = await _dataRepository.GetUserPermissionAsync(userId, navigationItemId);
            if (userPermission != null)
            {
                permission.CanView = userPermission.CanView;
                permission.CanCreate = userPermission.CanCreate;
                permission.CanEdit = userPermission.CanEdit;
                permission.CanDelete = userPermission.CanDelete;
            }

            return permission;
        }

        private NavigationPermissionDto MapToPermissionDto(UserNavigationPermission permission)
        {
            return new NavigationPermissionDto
            {
                NavigationItemId = permission.NavigationItemId,
                NavigationItemTitle = permission.NavigationItem?.Title ?? "",
                NavigationItemUrl = permission.NavigationItem?.Url,
                NavigationItemIcon = permission.NavigationItem?.Icon,
                ParentId = permission.NavigationItem?.ParentId,
                CanView = permission.CanView,
                CanCreate = permission.CanCreate,
                CanEdit = permission.CanEdit,
                CanDelete = permission.CanDelete
            };
        }

        private NavigationPermissionDto MapToPermissionDto(RoleNavigationPermission permission)
        {
            return new NavigationPermissionDto
            {
                NavigationItemId = permission.NavigationItemId,
                NavigationItemTitle = permission.NavigationItem?.Title ?? "",
                NavigationItemUrl = permission.NavigationItem?.Url,
                NavigationItemIcon = permission.NavigationItem?.Icon,
                ParentId = permission.NavigationItem?.ParentId,
                CanView = permission.CanView,
                CanCreate = permission.CanCreate,
                CanEdit = permission.CanEdit,
                CanDelete = permission.CanDelete
            };
        }

        private List<NavigationPermissionDto> BuildPermissionTree(List<NavigationPermissionDto> allPermissions)
        {
            var lookup = allPermissions.ToLookup(p => p.ParentId);

            foreach (var permission in allPermissions)
            {
                permission.Children = lookup[permission.NavigationItemId].ToList();
            }

            return lookup[null].ToList();
        }

        private NavigationMenuDto BuildNavigationMenu(List<NavigationPermissionDto> permissions)
        {
            var root = new NavigationMenuDto
            {
                Title = "Root",
                Children = new List<NavigationMenuDto>()
            };

            foreach (var permission in permissions.Where(p => p.CanView))
            {
                var menuItem = new NavigationMenuDto
                {
                    Id = permission.NavigationItemId,
                    Title = permission.NavigationItemTitle,
                    Url = permission.NavigationItemUrl,
                    Icon = permission.NavigationItemIcon,
                    HasAccess = permission.CanView,
                    CanCreate = permission.CanCreate,
                    CanEdit = permission.CanEdit,
                    CanDelete = permission.CanDelete,
                    Children = BuildNavigationMenuChildren(permission.Children)
                };

                root.Children.Add(menuItem);
            }

            return root;
        }

        private List<NavigationMenuDto> BuildNavigationMenuChildren(List<NavigationPermissionDto> children)
        {
            var result = new List<NavigationMenuDto>();

            foreach (var child in children.Where(c => c.CanView))
            {
                result.Add(new NavigationMenuDto
                {
                    Id = child.NavigationItemId,
                    Title = child.NavigationItemTitle,
                    Url = child.NavigationItemUrl,
                    Icon = child.NavigationItemIcon,
                    HasAccess = child.CanView,
                    CanCreate = child.CanCreate,
                    CanEdit = child.CanEdit,
                    CanDelete = child.CanDelete,
                    Children = BuildNavigationMenuChildren(child.Children)
                });
            }

            return result;
        }

        #endregion
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}