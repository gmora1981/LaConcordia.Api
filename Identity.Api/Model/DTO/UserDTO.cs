namespace Identity.Api.Model.DTO
{
    public class UserDTO
    {
        public required string UserId { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }

        public required string LastName { get; set; }
        // public string Password { get; set; }
    }
}
