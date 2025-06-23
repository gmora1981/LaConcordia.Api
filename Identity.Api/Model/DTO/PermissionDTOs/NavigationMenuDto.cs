namespace Identity.Api.Model.DTO.PermissionDTOs
{
    // DTO para el menú de navegación con permisos
    public class NavigationMenuDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? Icon { get; set; }
        public int Order { get; set; }
        public bool HasAccess { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public List<NavigationMenuDto> Children { get; set; } = new List<NavigationMenuDto>();
    }
}
