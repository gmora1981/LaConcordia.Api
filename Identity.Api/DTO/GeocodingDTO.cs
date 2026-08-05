namespace Identity.Api.DTO
{
    public class GeocodingResultDTO
    {
        public string DisplayName { get; set; } = null!;
        public decimal Lat { get; set; }
        public decimal Lon { get; set; }
    }
}
