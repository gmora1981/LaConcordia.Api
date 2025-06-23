namespace Identity.Api.Model.DTO.PermissionDTOs
{
    // DTO para mostrar todos los permisos de un usuario
    public class UserPermissionsDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public List<NavigationPermissionDto> Permissions { get; set; } = new List<NavigationPermissionDto>();
    }
}
