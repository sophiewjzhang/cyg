using Akavache;
using Microsoft.Extensions.Caching.Memory;
using services.abstractions;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace android.Services
{
    public class CacheService : ICacheService
    {
        private IMemoryCache inMemoryCache;
        private int inMemoryCacheExpirationInHours;
        private int permanentCacheExpirationInDays;

        public CacheService(IMemoryCache inMemoryCache, int inMemoryExpirationInHours, int permanentExpirationInDays)
        {
            this.inMemoryCache = inMemoryCache;
            this.inMemoryCacheExpirationInHours = inMemoryExpirationInHours;
            this.permanentCacheExpirationInDays = permanentExpirationInDays;
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
    }
}