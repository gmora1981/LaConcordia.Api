using Identity.Api.DTO;
using Identity.Api.Interfaces;
using System.Globalization;
using System.Text.Json;

namespace Identity.Api.Services
{
    // Proxy del servidor hacia la API de Google Maps (Places Autocomplete + Place Details +
    // Geocoding). Se llama desde el backend para no exponer la API Key en el navegador: el
    // frontend solo habla con nuestra API, nunca directo con Google.
    public class GeocodingServices : IGeocoding
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;

        // Guayaquil, Ecuador: centro de la ciudad donde opera la cooperativa, usado para
        // priorizar (no restringir) los resultados de autocompletado hacia esta zona.
        private const string LocationBias = "-2.1709979,-79.9223592";
        private const int RadioMetros = 30000;

        public GeocodingServices(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiKey = configuration["GoogleMaps:ApiKey"] ?? string.Empty;
        }

        public async Task<List<PlacePredictionDTO>> BuscarPredicciones(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || string.IsNullOrEmpty(_apiKey))
                return new List<PlacePredictionDTO>();

            var client = _httpClientFactory.CreateClient();
            var url = "https://maps.googleapis.com/maps/api/place/autocomplete/json"
                + $"?input={Uri.EscapeDataString(query)}"
                + $"&location={LocationBias}&radius={RadioMetros}"
                + "&components=country:ec&language=es"
                + $"&key={_apiKey}";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<PlacePredictionDTO>();

            var json = await response.Content.ReadAsStringAsync();
            var resultados = new List<PlacePredictionDTO>();

            using var doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("predictions", out var predicciones))
                return resultados;

            foreach (var item in predicciones.EnumerateArray())
            {
                var placeId = item.GetProperty("place_id").GetString();
                var descripcion = item.GetProperty("description").GetString();

                if (!string.IsNullOrEmpty(placeId) && !string.IsNullOrEmpty(descripcion))
                {
                    resultados.Add(new PlacePredictionDTO
                    {
                        PlaceId = placeId,
                        Description = descripcion
                    });
                }
            }

            return resultados;
        }

        public async Task<GeocodingResultDTO?> ObtenerCoordenadasPorPlaceId(string placeId)
        {
            if (string.IsNullOrWhiteSpace(placeId) || string.IsNullOrEmpty(_apiKey))
                return null;

            var client = _httpClientFactory.CreateClient();
            var url = "https://maps.googleapis.com/maps/api/place/details/json"
                + $"?place_id={Uri.EscapeDataString(placeId)}"
                + "&fields=geometry,formatted_address&language=es"
                + $"&key={_apiKey}";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("result", out var resultado))
                return null;

            if (!resultado.TryGetProperty("geometry", out var geometry) ||
                !geometry.TryGetProperty("location", out var location))
                return null;

            var lat = location.GetProperty("lat").GetDecimal();
            var lng = location.GetProperty("lng").GetDecimal();
            var direccion = resultado.TryGetProperty("formatted_address", out var faEl)
                ? faEl.GetString() ?? string.Empty
                : string.Empty;

            return new GeocodingResultDTO
            {
                DisplayName = direccion,
                Lat = lat,
                Lon = lng
            };
        }

        public async Task<string?> BuscarDireccionPorCoordenadas(decimal lat, decimal lon)
        {
            if (string.IsNullOrEmpty(_apiKey))
                return null;

            var client = _httpClientFactory.CreateClient();
            var latStr = lat.ToString(CultureInfo.InvariantCulture);
            var lonStr = lon.ToString(CultureInfo.InvariantCulture);
            var url = "https://maps.googleapis.com/maps/api/geocode/json"
                + $"?latlng={latStr},{lonStr}&language=es&key={_apiKey}";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("results", out var resultados) &&
                resultados.GetArrayLength() > 0 &&
                resultados[0].TryGetProperty("formatted_address", out var direccionEl))
            {
                return direccionEl.GetString();
            }

            return null;
        }
    }
}
