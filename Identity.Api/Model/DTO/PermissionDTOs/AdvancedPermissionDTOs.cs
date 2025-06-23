using Identity.Api.Model.DTO.PermissionDTOs;

namespace Identity.Api.Model.DTOs
{
    public class PermissionAuditDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<PermissionChangeDto> UserPermissionChanges { get; set; } = new();
        public List<PermissionChangeDto> RolePermissionChanges { get; set; } = new();
    }

    public class PermissionChangeDto
    {
        public string EntityId { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public int NavigationItemId { get; set; }
        public string NavigationItemTitle { get; set; } = string.Empty;
        public DateTime GrantedAt { get; set; }
        public string GrantedBy { get; set; } = string.Empty;
    }

    public class PermissionMatrixDto
    {
        public List<NavigationItemSummaryDto> NavigationItems { get; set; } = new();
        public List<RoleSummaryDto> Roles { get; set; } = new();
        public List<PermissionMatrixEntryDto> Permissions { get; set; } = new();
    }

    public class NavigationItemSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int? ParentId { get; set; }
    }

    public class RoleSummaryDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class PermissionMatrixEntryDto
    {
        public string RoleId { get; set; } = string.Empty;
        public int NavigationItemId { get; set; }
        public bool CanView { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }

    public class PermissionTemplateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<NavigationPermissionDto> Permissions { get; set; } = new();
    }

    public class CopyPermissionsDto
    {
        public string SourceId { get; set; } = string.Empty;
        public string TargetId { get; set; } = string.Empty;
        public string EntityType { get; set; } = "User"; // User or Role
    }
}