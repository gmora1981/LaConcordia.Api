namespace Identity.Api.Model.DTO.PermissionDTOs
{
    // DTO para asignar permisos masivos
    public class BulkPermissionAssignmentDto
    {
        public List<int> NavigationItemIds { get; set; } = new List<int>();
        public bool CanView { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}
