using Identity.Api.DTO;

namespace Identity.Api.Interfaces
{
    public interface IGeocoding
    {
        // Predicciones tipo autocompletar mientras el usuario escribe (Google Places Autocomplete).
        // Todavia no traen coordenadas.
        Task<List<PlacePredictionDTO>> BuscarPredicciones(string query);

        // Resuelve una prediccion ya seleccionada a sus coordenadas (Google Place Details).
        Task<GeocodingResultDTO?> ObtenerCoordenadasPorPlaceId(string placeId);

        // Geocodificacion inversa: coordenadas -> texto de direccion legible.
        Task<string?> BuscarDireccionPorCoordenadas(decimal lat, decimal lon);
    }
}
