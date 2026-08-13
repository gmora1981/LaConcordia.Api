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

            query = query.Trim();

            // Caso interseccion ("Colombia y Manuel de Villavicencio" / "Colombia, Manuel de
            // Villavicencio"): Nominatim no resuelve cruces de calles directamente. Si el texto
            // trae dos nombres separados, se geocodifica cada calle por separado y se devuelve
            // el punto medio entre las coincidencias mas cercanas entre si, como aproximacion
            // de la esquina.
            var partes = DividirEnDosCalles(query);
            if (partes != null)
            {
                var interseccion = await BuscarInterseccion(partes.Value.calle1, partes.Value.calle2);
                if (interseccion != null)
                    return new List<GeocodingResultDTO> { interseccion };
            }

            // Busqueda estructurada (calle separada de ciudad/pais): evita que Nominatim confunda
            // un nombre de calle con el nombre de un pais real (ej. la calle "Colombia" en
            // Guayaquil se interpretaba como el pais Colombia y, al chocar con el filtro de
            // Ecuador, no devolvia nada).
            var resultados = await BuscarEstructurado(query);
            if (resultados.Count > 0)
                return resultados;

            // Ultimo intento: busqueda libre tal cual la escribio el usuario (con pista de ciudad
            // si no la trae), por si la estructurada no encontro nada.
            var queryConContexto = query.Contains("Ecuador", StringComparison.OrdinalIgnoreCase)
                ? query
                : $"{query}, Guayaquil, Ecuador";

            resultados = await BuscarLibre(queryConContexto);
            if (resultados.Count == 0 && queryConContexto != query)
            {
                resultados = await BuscarLibre(query);
            }

            return resultados;
        }

        // Intenta partir "Calle1 y Calle2" / "Calle1, Calle2" en sus dos nombres de calle.
        private static (string calle1, string calle2)? DividirEnDosCalles(string query)
        {
            string[] separadores = { " y ", " esquina ", " esq. ", "," };
            foreach (var sep in separadores)
            {
                var idx = query.IndexOf(sep, StringComparison.OrdinalIgnoreCase);
                if (idx > 0 && idx < query.Length - sep.Length)
                {
                    var calle1 = query.Substring(0, idx).Trim();
                    var calle2 = query.Substring(idx + sep.Length).Trim();
                    // Se exige un minimo de caracteres en la segunda calle para no disparar
                    // busquedas de interseccion en cada tecla mientras el usuario aun esta escribiendo
                    // (el frontend ya llama a Buscar con debounce, pero esto evita llamadas de mas).
                    if (calle1.Length > 0 && calle2.Length >= 3)
                        return (calle1, calle2);
                }
            }
            return null;
        }

        private async Task<GeocodingResultDTO?> BuscarInterseccion(string calle1, string calle2)
        {
            var resultados1 = await BuscarEstructurado(calle1);
            await Task.Delay(250); // respeta el limite de 1 solicitud/segundo de Nominatim
            var resultados2 = await BuscarEstructurado(calle2);

            if (resultados1.Count == 0 || resultados2.Count == 0)
                return null;

            GeocodingResultDTO? mejor1 = null, mejor2 = null;
            var mejorDistancia = double.MaxValue;

            foreach (var r1 in resultados1)
            {
                foreach (var r2 in resultados2)
                {
                    var distancia = DistanciaMetros(r1.Lat, r1.Lon, r2.Lat, r2.Lon);
                    if (distancia < mejorDistancia)
                    {
                        mejorDistancia = distancia;
                        mejor1 = r1;
                        mejor2 = r2;
                    }
                }
            }

            // Si el par mas cercano entre ambas calles esta a mas de ~2km, probablemente no se
            // cruzan (son calles con el mismo nombre en otra zona de la ciudad) y no conviene
            // devolver un punto enganoso.
            if (mejor1 == null || mejor2 == null || mejorDistancia > 2000)
                return null;

            return new GeocodingResultDTO
            {
                DisplayName = $"{calle1} y {calle2} (esquina aproximada)",
                Lat = (mejor1.Lat + mejor2.Lat) / 2,
                Lon = (mejor1.Lon + mejor2.Lon) / 2
            };
        }

        // Distancia aproximada en metros entre dos coordenadas, suficiente para comparar
        // cercania dentro de una misma ciudad (no hace falta la formula exacta de Haversine).
        private static double DistanciaMetros(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            const double metrosPorGradoLat = 111320;
            var mediaLatRad = (double)(lat1 + lat2) / 2 * Math.PI / 180;
            var metrosPorGradoLon = metrosPorGradoLat * Math.Cos(mediaLatRad);

            var dLat = (double)(lat1 - lat2) * metrosPorGradoLat;
            var dLon = (double)(lon1 - lon2) * metrosPorGradoLon;
            return Math.Sqrt(dLat * dLat + dLon * dLon);
        }

        // Busqueda estructurada: el texto del usuario va solo en "street", separado de ciudad/pais,
        // para que Nominatim no lo confunda con el nombre de un pais o de otra entidad.
        private async Task<List<GeocodingResultDTO>> BuscarEstructurado(string calleYNumero)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("LaConcordiaDespacho/1.0 (soporte@lconcordia.com)");

            var url = "https://nominatim.openstreetmap.org/search"
                + $"?street={Uri.EscapeDataString(calleYNumero)}"
                + "&city=Guayaquil&country=Ecuador"
                + "&format=json&limit=5&addressdetails=0";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<GeocodingResultDTO>();

            var json = await response.Content.ReadAsStringAsync();
            return ParsearResultados(json);
        }

        private async Task<List<GeocodingResultDTO>> BuscarLibre(string query)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("LaConcordiaDespacho/1.0 (soporte@lconcordia.com)");

            var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(query)}&format=json&limit=5&addressdetails=0&countrycodes=ec";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<GeocodingResultDTO>();

            var json = await response.Content.ReadAsStringAsync();
            return ParsearResultados(json);
        }

        private static List<GeocodingResultDTO> ParsearResultados(string json)
        {
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
