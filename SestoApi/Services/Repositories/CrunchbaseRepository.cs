using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using sesto.api.Models.Crunchbase;
using sesto.api.Services.Interfaces;

namespace sesto.api.Services.Repositories
{
    public class CrunchbaseRepository : ICrunchbaseRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CrunchbaseRepository> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClient;

        public CrunchbaseRepository(ILogger<CrunchbaseRepository> logger, IConfiguration configuration, IMemoryCache memoryCache, IHttpClientFactory httpClient)
        {
            _logger = logger;
            _configuration = configuration;
            _memoryCache = memoryCache;
            _httpClient = httpClient;

        }
        /// <summary>
        /// returns the domain from crunchbase or null if it is not found.
        /// </summary>
        /// <param name="name">the name of the place or company</param>
        /// <returns></returns>
        public async Task<string> GetDomain(string name)
        {
            var client = _httpClient.CreateClient();
            var crunchbaseKey = _configuration["CrunchbaseAPIKey"];
            var baseUrl = _configuration["CrunchbaseBaseUrl"];
            return await _memoryCache.GetOrCreateAsync($"{name}_crunchbase", async (f) =>
            {
                f.SlidingExpiration = TimeSpan.FromHours(1);
                var responseMessage = await client.GetAsync($"{baseUrl}?name={name}&user_key={crunchbaseKey}&page=1");
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var domainData = await JsonSerializer.DeserializeAsync<CrunchbaseRoot>(await responseMessage.Content.ReadAsStreamAsync());
                    if (domainData.Data.Items.Count == 0)
                    {
                        _logger.LogInformation($"{name} was not found in crunchbase");
                        return null;
                    }
                    try
                    {
                        var match = domainData.Data.Items.Where(i => i.Properties.Name.StartsWith(name)).First();
                        if (match == null)
                        {
                            _logger.LogInformation($"{name} was not found in crunchbase");
                            return null;
                        }

                        var regex = new Regex("/[^/]*$");
                        return regex.Replace(match.Properties.Domain, string.Empty);
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogWarning($"{name} was not found in crunchbase. {Ex}");
                        return null;
                    }

                }
                _logger.LogError($"{name} caused a 400 bad request.");
                return null;
            });
        }

    }
}
