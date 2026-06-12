using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Json;

namespace SePrise.WinForms.Services;

public class ApiClient
{
    private readonly HttpClient _client;
    private readonly string _baseUrl = "http://localhost:5293";
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClient()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri(_baseUrl);

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
        };
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        var response = await _client.GetAsync(endpoint);
        await HandleErrorAsync(response);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return default;
        return await DeserializeSafelyAsync<T>(response);
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        var response = await _client.PostAsJsonAsync(endpoint, data, _jsonOptions);
        await HandleErrorAsync(response);
        return await DeserializeSafelyAsync<T>(response);
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        var response = await _client.PutAsJsonAsync(endpoint, data, _jsonOptions);
        await HandleErrorAsync(response);
        return await DeserializeSafelyAsync<T>(response);
    }

    public async Task<T?> PatchAsync<T>(string endpoint, object data)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, endpoint)
        {
            Content = JsonContent.Create(data, null, _jsonOptions)
        };
        var response = await _client.SendAsync(request);
        await HandleErrorAsync(response);
        return await DeserializeSafelyAsync<T>(response);
    }

    public async Task DeleteAsync(string endpoint)
    {
        var response = await _client.DeleteAsync(endpoint);
        await HandleErrorAsync(response);
    }

    private async Task<T?> DeserializeSafelyAsync<T>(HttpResponseMessage response)
    {
        try
        {
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (JsonException ex)
        {
            throw new Exception($"⚠️ Desincronización detectada entre Front y API (Contrato roto):\nNo se pudo leer la respuesta correctamente.\nDetalle técnico: {ex.Message}");
        }
    }

    private async Task HandleErrorAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return; 
            }

            string content = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error de API: {response.StatusCode}. Detalle: {content}");
        }
    }
}
