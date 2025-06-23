namespace Identity.Api.Model.DTO.PermissionDTOs
{
    // DTO para verificar permisos
    public class CheckPermissionDto
    {
        public string UserId { get; set; } = string.Empty;
        public int NavigationItemId { get; set; }
        public string PermissionType { get; set; } = "View"; // View, Create, Edit, Delete
    }
}
