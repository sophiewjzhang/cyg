using Akavache;
using Microsoft.Extensions.Caching.Memory;
using services.abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace android.Services
{
    public class CacheService : ICacheService
    {
        private IMemoryCache inMemoryCache;
        private int inMemoryCacheExpirationInHours;
        private int permanentCacheExpirationInDays;
        private string baseUrl;
        private int apiTimeoutInSeconds;

        public CacheService(IMemoryCache inMemoryCache, int inMemoryExpirationInHours, int permanentExpirationInDays, string baseUrl, int apiTimeoutInSeconds)
        {
            this.inMemoryCache = inMemoryCache;
            this.inMemoryCacheExpirationInHours = inMemoryExpirationInHours;
            this.permanentCacheExpirationInDays = permanentExpirationInDays;
            this.baseUrl = baseUrl;
            this.apiTimeoutInSeconds = apiTimeoutInSeconds;
        }

        public void AddEntitiesToInMemoryCache<T>(string key, IEnumerable<T> entities)
        {
            inMemoryCache.Set(key, entities, TimeSpan.FromHours(inMemoryCacheExpirationInHours));
        }

        public IEnumerable<T> GetEntitiesFromInMemoryCache<T>(string key)
        {
            return inMemoryCache.Get<IEnumerable<T>>(key);
        }

        public void AddEntitiesToPersistentCache<T>(string key, IEnumerable<T> entities)
        {
            BlobCache.LocalMachine.InsertObject(key, entities, TimeSpan.FromDays(permanentCacheExpirationInDays));
        }

        public async Task<IEnumerable<T>> GetEntitiesFromPersistentCache<T>(string key)
        {
            try
            {
                // TODO: redo with GetOrCreate - pass handler to load from API

                var cache = BlobCache.LocalMachine.GetObject<IEnumerable<T>>(key);
                return await cache.FirstOrDefaultAsync();
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void AddObjectToUserCache<T>(string key, T obj)
        {
            BlobCache.UserAccount.InsertObject(key, obj);
        }

        public async Task<T> GetObjectFromUserCacheAsync<T>(string key) where T : class
        {
            try
            {
                // TODO: redo with GetOrCreate - pass handler to load from API
                var cache = BlobCache.UserAccount.GetObject<T>(key);
                return await cache.FirstOrDefaultAsync();
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        public async Task UpdateCacheStatusIfRequired()
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(apiTimeoutInSeconds);
                try
                {
                    var response = await client.GetAsync(new Uri(new Uri(baseUrl), "cache/check"));
                    using (var streamReader = new StreamReader(await response.Content.ReadAsStreamAsync()))
                    {
                        string cacheStatus = await streamReader.ReadToEndAsync();
                        switch (cacheStatus)
                        {
                            case "cache":
                                await BlobCache.LocalMachine.InvalidateAll();
                                break;
                            case "settings":
                                await BlobCache.UserAccount.InvalidateAll();
                                break;
                            case "cache_settings":
                                await BlobCache.LocalMachine.InvalidateAll();
                                await BlobCache.UserAccount.InvalidateAll();
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                }
            }
        }

        public static void InvalidateCache()
        {
            BlobCache.LocalMachine.InvalidateAll();
        }
    }
}