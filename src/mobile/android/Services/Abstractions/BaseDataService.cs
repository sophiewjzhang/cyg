using Akavache.Sqlite3;
using android.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace android.Services.Abstractions
{
    public abstract class BaseDataService
    {
        private readonly ICacheService cacheService;
        private readonly Uri baseUrl;

        public BaseDataService(ICacheService cacheService, string baseUrl)
        {
            this.cacheService = cacheService;
            this.baseUrl = new Uri(baseUrl);
        }

        private IEnumerable<T> GetEntitiesFromCache<T>(string key)
        {
            return cacheService.GetEntitiesFromInMemoryCache<T>(key);
        }

        private async Task<IEnumerable<T>> GetEntitiesFromPersistentCache<T>(string key)
        {
            return await cacheService.GetEntitiesFromPersistentCache<T>(key);
        }

        protected async Task<IEnumerable<T>> GetEntitiesFromApiAsync<T>(Uri uri)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(uri);
                using (var streamReader = new StreamReader(await response.Content.ReadAsStreamAsync()))
                {
                    return JsonConvert.DeserializeObject<IEnumerable<T>>(await streamReader.ReadToEndAsync());
                }
            }
        }

        public async Task<IEnumerable<T>> GetAsync<T>(string key, string relativeUrl)
        {
            var entities = GetEntitiesFromCache<T>(key);
            if (entities == null)
            {
                entities = await GetEntitiesFromPersistentCache<T>(key);
                if (entities == null)
                {
                    entities = await GetEntitiesFromApiAsync<T>(new Uri(baseUrl, relativeUrl));
                    cacheService.AddEntitiesToPersistentCache<T>(key, entities);
                }
                cacheService.AddEntitiesToInMemoryCache<T>(key, entities);
                return entities;
            }
            return entities;
        }
    }
}