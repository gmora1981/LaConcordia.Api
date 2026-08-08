using Identity.Api.DTO;

namespace Identity.Api.Interfaces
{
    public interface IGeocoding
    {
        Task<List<GeocodingResultDTO>> Buscar(string query);

        // Geocodificacion inversa: coordenadas -> texto de direccion legible.
        Task<string?> BuscarDireccionPorCoordenadas(decimal lat, decimal lon);
    }
}
