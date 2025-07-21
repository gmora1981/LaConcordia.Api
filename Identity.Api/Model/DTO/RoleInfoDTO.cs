namespace Identity.Api.Model.DTO
{
    public class RoleInfoDTO
    {
        public required string RoleId { get; set; }
        public required string RoleName { get; set; }
        public string? Description { get; set; }
        public int UserCount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}