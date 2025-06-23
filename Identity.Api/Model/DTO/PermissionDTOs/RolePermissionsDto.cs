namespace Identity.Api.Model.DTO.PermissionDTOs
{
    // DTO para mostrar todos los permisos de un rol
    public class RolePermissionsDto
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public List<NavigationPermissionDto> Permissions { get; set; } = new List<NavigationPermissionDto>();
    }
}
