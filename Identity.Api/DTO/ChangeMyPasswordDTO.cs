namespace Identity.Api.DTO
{
    public class ChangeMyPasswordDTO
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
