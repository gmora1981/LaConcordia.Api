namespace Identity.Api.DTO
{
    public class GeocodingResultDTO
    {
        public string DisplayName { get; set; } = null!;
        public decimal Lat { get; set; }
        public decimal Lon { get; set; }
    }

    // Prediccion de Google Places Autocomplete: todavia no trae coordenadas, solo el texto
    // sugerido y el PlaceId necesario para resolverlas despues (Place Details).
    public class PlacePredictionDTO
    {
        public string PlaceId { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
