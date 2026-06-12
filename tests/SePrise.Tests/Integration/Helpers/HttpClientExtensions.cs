namespace SePrise.Tests.Integration.Helpers;

using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

/// <summary>
/// Métodos extensores para facilitar la interacción con HttpClient en las pruebas.
/// </summary>
public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<HttpResponseMessage> PostJsonAsync<T>(this HttpClient client, string url, T body)
    {
        return await client.PostAsJsonAsync(url, body);
    }

    public static async Task<HttpResponseMessage> PatchJsonAsync<T>(this HttpClient client, string url, T body)
    {
        // En .NET 6+ se puede usar PatchAsJsonAsync del paquete System.Net.Http.Json si se inyecta o configurar a mano
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        return await client.PatchAsync(url, content);
    }

    public static async Task<T?> ReadAsAsync<T>(this HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"La respuesta indica un error ({response.StatusCode}): {errorContent}");
        }

        var contentStream = await response.Content.ReadAsStreamAsync();
        if (contentStream.Length == 0)
        {
            return default;
        }

        return await JsonSerializer.DeserializeAsync<T>(contentStream, JsonOptions);
    }

    public static async Task<string> ReadErrorMessageAsync(this HttpResponseMessage response)
    {
        return await response.Content.ReadAsStringAsync();
    }
}
