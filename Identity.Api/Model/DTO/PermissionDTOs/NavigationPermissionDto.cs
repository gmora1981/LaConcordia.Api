namespace Identity.Api.Model.DTO.PermissionDTOs
{
    // DTO para mostrar permisos de navegación
    public class NavigationPermissionDto
    {
        public int NavigationItemId { get; set; }
        public string NavigationItemTitle { get; set; } = string.Empty;
        public string? NavigationItemUrl { get; set; }
        public string? NavigationItemIcon { get; set; }
        public int? ParentId { get; set; }
        public bool CanView { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public List<NavigationPermissionDto> Children { get; set; } = new List<NavigationPermissionDto>();
    }
}
