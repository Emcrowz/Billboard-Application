using System.Text.Json;

namespace Billboard_FrontEnd.Services
{
    public class WebAPIService : IWebAPIService
    {
        private readonly string _apiBase;
        private readonly HttpClient _httpClient; // Allows HTTP communication via API
        private readonly JsonSerializerOptions _options;

        public WebAPIService(string apiBase)
        {
            _apiBase = apiBase;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_apiBase);

            _options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
        }
    }
}
