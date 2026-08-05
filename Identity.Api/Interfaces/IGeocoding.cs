using Identity.Api.DTO;

namespace Identity.Api.Interfaces
{
    public interface IGeocoding
    {
        Task<List<GeocodingResultDTO>> Buscar(string query);
    }
}
