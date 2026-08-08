using Identity.Api.DTO;
using Identity.Api.Interfaces;
using System.Globalization;
using System.Text.Json;

namespace Identity.Api.Services
{
    // Proxy del servidor hacia Nominatim (geocodificacion de OpenStreetMap, gratuita).
    // Se llama desde el backend (no desde el navegador) para fijar un User-Agent valido,
    // segun exige la politica de uso de Nominatim, y para no depender de CORS en el cliente.
    public class GeocodingServices : IGeocoding
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public GeocodingServices(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<GeocodingResultDTO>> Buscar(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<GeocodingResultDTO>();

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("LaConcordiaDespacho/1.0 (soporte@lconcordia.com)");

            var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(query)}&format=json&limit=5&addressdetails=0";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<GeocodingResultDTO>();

            var json = await response.Content.ReadAsStringAsync();
            var resultados = new List<GeocodingResultDTO>();

            using var doc = JsonDocument.Parse(json);
            foreach (var item in doc.RootElement.EnumerateArray())
            {
                var lat = decimal.Parse(item.GetProperty("lat").GetString()!, CultureInfo.InvariantCulture);
                var lon = decimal.Parse(item.GetProperty("lon").GetString()!, CultureInfo.InvariantCulture);
                var displayName = item.GetProperty("display_name").GetString() ?? "";

                resultados.Add(new GeocodingResultDTO
                {
                    DisplayName = displayName,
                    Lat = lat,
                    Lon = lon
                });
            }

            return resultados;
        }

        public async Task<string?> BuscarDireccionPorCoordenadas(decimal lat, decimal lon)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("LaConcordiaDespacho/1.0 (soporte@lconcordia.com)");

            var latStr = lat.ToString(CultureInfo.InvariantCulture);
            var lonStr = lon.ToString(CultureInfo.InvariantCulture);
            var url = $"https://nominatim.openstreetmap.org/reverse?lat={latStr}&lon={lonStr}&format=json";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("display_name", out var displayNameEl))
                return displayNameEl.GetString();

            return null;
        }
    }
}
