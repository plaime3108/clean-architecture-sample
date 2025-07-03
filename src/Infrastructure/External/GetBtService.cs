using System.Text;
using System.Text.Json;
using Application.Interfaces.Cache;
using Microsoft.Extensions.Options;
using Application.Interfaces.Utils;
using Infrastructure.Configurations;
using Application.Interfaces.External;
using Application.Contracts.BtServices.Token;

namespace Infrastructure.External
{
    public class GetBtService : IExternalApiClient
    {
        private readonly ICacheService _cacheService;
        private readonly IOptions<BtServicesConfig> _options;
        private readonly IHttpClientFactory _client;
        private readonly IBtinreqProvider _btinreqProvider;
        public GetBtService(IOptions<BtServicesConfig> options, IHttpClientFactory client, ICacheService cacheService, IBtinreqProvider btinreqProvider)
        {
            _options = options;
            _cacheService = cacheService;
            _client = client;
            _btinreqProvider = btinreqProvider;
        }

        public async Task<GetTokenResponse> SendAsync<GetTokenResponse>()
        {
            try
            {
                const string cacheKey = "token-btservice";
                var cachedData = await _cacheService.GetAsync<GetTokenResponse>(cacheKey);
                if (cachedData != null)
                    return cachedData;

                GetTokenResquest request = new()
                {
                    Btinreq = _btinreqProvider.GetBtinreqAsync(string.Empty),
                    UserId = _options.Value.Username,
                    UserPassword = _options.Value.Password,
                };

                using var httpClient = _client.CreateClient(_options.Value.HttpClientName);
                var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
                var result = await httpClient.PostAsync(_options.Value.GetTokenPath, content);
                var tokenResponse = JsonSerializer.Deserialize<GetTokenResponse>(await result.Content.ReadAsStringAsync());
                await _cacheService.SetAsync(cacheKey, () => Task.FromResult(tokenResponse), TimeSpan.FromHours(20));
                return tokenResponse!;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, string path)
        {
            try
            {
                var requestUri = _options.Value.GetType().GetProperty(path)!.GetValue(_options.Value)?.ToString();
                if (string.IsNullOrEmpty(requestUri))
                {
                    throw new ArgumentException($"The path '{path}' is not valid in the configuration.", nameof(path));
                }
                using var httpClient = _client.CreateClient(_options.Value.HttpClientName);
                var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
                var result = await httpClient.PostAsync(requestUri, content);
                var response = JsonSerializer.Deserialize<TResponse>(await result.Content.ReadAsStringAsync());
                return response!;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
