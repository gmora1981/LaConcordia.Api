namespace Identity.Api.Model.DTO.PermissionDTOs
{
    // DTO para actualizar permisos de rol
    public class UpdateRolePermissionDto
    {
        public string RoleId { get; set; } = string.Empty;
        public int NavigationItemId { get; set; }
        public bool CanView { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}
