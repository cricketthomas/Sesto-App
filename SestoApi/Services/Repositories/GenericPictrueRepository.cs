using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PexelsDotNetSDK.Api;
using PexelsDotNetSDK.Models;
using sesto.api.Services.Interfaces;
using System.Linq;
using System.Collections.Generic;
using sesto.api.Static;
using System.IO;
using System.Text.RegularExpressions;

namespace sesto.api.Services.Repositories
{
    public class GenericPictrueRepository : IGenericPictrueRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GenericPictrueRepository> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClient;
        public GenericPictrueRepository(ILogger<GenericPictrueRepository> logger, IConfiguration configuration, IMemoryCache memoryCache, IHttpClientFactory httpClient)
        {
            _logger = logger;
            _configuration = configuration;
            _memoryCache = memoryCache;
            _httpClient = httpClient;

        }

        public string _pexelsApiKey => _configuration["PexelApiKey"];
        public PexelsClient pexelsClient => new PexelsClient(_pexelsApiKey);

        //https://www.pexels.com/api/documentation/#photos-search
        private async Task<string> GetPexelImage(string query)
        {

            try
            {
                return await _memoryCache.GetOrCreateAsync($"{query}_pexel", async (f) =>
                {
                    f.SlidingExpiration = TimeSpan.FromHours(1);
                    try
                    {
                        var result = await pexelsClient.SearchPhotosAsync(query);
                        if (result.photos.Count < 1)
                            return null;

                        var orginialImg = result.photos.First().source.original;
                        return orginialImg;
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogWarning($"{query} was not found in pexels. {Ex}");
                        return null;
                    }

                });

            }

            catch (Exception Ex)
            {
                _logger.LogWarning($"{query} was not found in pexels. {Ex}");
                return null;
            }

        }

        public async Task<string> GetFirstPexelImage(string[] searchTypes)
        {
            string imageResult = null;
            int count = 0;
            while (imageResult == null && count <= searchTypes.Length)
            {
                imageResult = await GetPexelImage(searchTypes[count]);
                count++;
            }
            return imageResult;
        }


        /// <summary>
        /// this fetches the correspoding emoji to render on the ui
        /// </summary>
        public Emoji GetEmoji(string name, string placeType)
        {
            var json = _memoryCache.GetOrCreate("json", (f) =>
            {
                f.SlidingExpiration = TimeSpan.FromHours(1);
                var _json = JsonSerializer.Deserialize<IList<Emoji>>(File.ReadAllText("./static/emoji.json"));
                return _json;
            });

            var jsonAliases = json.Where(props => props.Aliases != null && props.Tags != null);
            var aliasEmoji = jsonAliases.Where(alias =>
                    alias.Aliases.Any(a => a.Equals(name)) || alias.Tags.Any(t => t.Equals(name)) ||
                    alias.Aliases.Any(a => a.Equals(placeType)) || alias.Tags.Any(t => t.Equals(placeType))).FirstOrDefault();
            var firstMatchingName = json.Where(slug => name.Contains(slug.Description, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if (firstMatchingName != null)
                return firstMatchingName;

            if (aliasEmoji != null)
                return aliasEmoji;

            var defaultEmoji = json.Where(slug => slug.Description.ToLowerInvariant().Equals(placeType.ToLowerInvariant())).SingleOrDefault();
            return defaultEmoji;

        }

    }
}
