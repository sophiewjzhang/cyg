using Akavache;
using android.Services.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;

//[assembly: Xamarin.Forms.Dependency (typeof (CacheService))]
namespace android.Services
{
    public class CacheService : ICacheService
    {
        private IMemoryCache inMemoryCache;
        private IObjectBlobCache persistentCache;
        private int inMemoryCacheExpirationInHours;
        private int permanentCacheExpirationInDays;

        public CacheService(IMemoryCache inMemoryCache, IObjectBlobCache persistentCache, int inMemoryExpirationInHours, int permanentExpirationInDays)
        {
            this.inMemoryCache = inMemoryCache;
            this.persistentCache = persistentCache;
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
            persistentCache.InsertObject(key, entities, TimeSpan.FromDays(permanentCacheExpirationInDays));
        }

        public async Task<IEnumerable<T>> GetEntitiesFromPersistentCache<T>(string key)
        {
            return await persistentCache.GetObject<IEnumerable<T>>(key).FirstOrDefaultAsync();


        }
    }
}