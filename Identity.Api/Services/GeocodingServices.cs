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

            // Google devuelve HTTP 200 incluso cuando falla (API no habilitada, key
            // restringida, etc.); el error real viene en "status"/"error_message".
            ValidarStatusGoogle(doc.RootElement, "Places Autocomplete");

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

            ValidarStatusGoogle(doc.RootElement, "Place Details");

            if (!doc.RootElement.TryGetProperty("result", out var resultado))
                return null;

            if (!resultado.TryGetProperty("geometry", out var geometry) ||
                !geometry.TryGetProperty("location", out var location))
                return null;

            var lat = location.GetProperty("lat").GetDecimal();
            var lng = location.GetProperty("lng").GetDecimal();

            // Ninguna direccion real en Ecuador tiene lat o lng exactamente en 0 (0,0 es
            // "Null Island", en el Atlantico frente a Africa). Si aparece, es señal de una
            // respuesta invalida/incompleta: se rechaza explicitamente (con el JSON crudo de
            // Google en el mensaje, para poder diagnosticar) en vez de mover el marcador ahi.
            if (lat == 0 || lng == 0)
            {
                var jsonResumido = json.Length > 600 ? json.Substring(0, 600) + "…" : json;
                throw new Exception($"Google Place Details devolvió lat={lat}, lng={lng} para este lugar "
                    + $"(place_id={placeId}). Respuesta: {jsonResumido}");
            }

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

            ValidarStatusGoogle(doc.RootElement, "Geocoding (reverse)");

            if (doc.RootElement.TryGetProperty("results", out var resultados) &&
                resultados.GetArrayLength() > 0 &&
                resultados[0].TryGetProperty("formatted_address", out var direccionEl))
            {
                return direccionEl.GetString();
            }

            return null;
        }

        // Google Maps Platform responde HTTP 200 incluso cuando la solicitud falla (API no
        // habilitada, key con restricciones incorrectas, facturacion no activada, etc.); el
        // motivo real viene en el campo "status". Sin esto, esos errores quedaban en silencio
        // y el mapa simplemente no reaccionaba a la seleccion del usuario.
        private static void ValidarStatusGoogle(JsonElement root, string operacion)
        {
            if (!root.TryGetProperty("status", out var statusEl))
                return;

            var status = statusEl.GetString();
            if (status == "OK" || status == "ZERO_RESULTS")
                return;

            var mensaje = root.TryGetProperty("error_message", out var errEl)
                ? errEl.GetString()
                : null;

            throw new Exception($"Google Maps ({operacion}) devolvio '{status}'"
                + (string.IsNullOrEmpty(mensaje) ? "." : $": {mensaje}"));
        }
    }
}
