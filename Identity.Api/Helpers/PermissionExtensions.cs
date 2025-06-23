using Identity.Api.Model.DTO.PermissionDTOs;
using Identity.Api.Model.DTOs;

namespace Identity.Api.Helpers
{
    public static class PermissionExtensions
    {
        public static bool HasAnyPermission(this NavigationPermissionDto permission)
        {
            return permission.CanView || permission.CanCreate ||
                   permission.CanEdit || permission.CanDelete;
        }

        public static void GrantAllPermissions(this NavigationPermissionDto permission)
        {
            permission.CanView = true;
            permission.CanCreate = true;
            permission.CanEdit = true;
            permission.CanDelete = true;
        }

        public static void RevokeAllPermissions(this NavigationPermissionDto permission)
        {
            permission.CanView = false;
            permission.CanCreate = false;
            permission.CanEdit = false;
            permission.CanDelete = false;
        }

        public static string ToPermissionString(this NavigationPermissionDto permission)
        {
            var perms = new List<string>();
            if (permission.CanView) perms.Add("Ver");
            if (permission.CanCreate) perms.Add("Crear");
            if (permission.CanEdit) perms.Add("Editar");
            if (permission.CanDelete) perms.Add("Eliminar");

            return perms.Any() ? string.Join(", ", perms) : "Sin permisos";
        }
    }
}