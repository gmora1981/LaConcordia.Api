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

            // Nominatim, a diferencia de Google Maps, no infiere la ciudad del usuario: una
            // busqueda local como "Colombia 1017" (calle + numero, sin ciudad) no devuelve nada
            // porque es ambigua a nivel mundial. Como los pedidos son siempre dentro de Guayaquil,
            // se agrega esa pista a la consulta; si aun asi no hay resultados, se reintenta con el
            // texto tal cual lo escribio el usuario (por si ya incluia ciudad/pais o era otro lugar).
            var queryConContexto = query.Contains("Ecuador", StringComparison.OrdinalIgnoreCase)
                ? query
                : $"{query}, Guayaquil, Ecuador";

            var resultados = await BuscarEnNominatim(queryConContexto);
            if (resultados.Count == 0 && queryConContexto != query)
            {
                resultados = await BuscarEnNominatim(query);
            }

            return resultados;
        }

        private async Task<List<GeocodingResultDTO>> BuscarEnNominatim(string query)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("LaConcordiaDespacho/1.0 (soporte@lconcordia.com)");

            var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(query)}&format=json&limit=5&addressdetails=0&countrycodes=ec";

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
