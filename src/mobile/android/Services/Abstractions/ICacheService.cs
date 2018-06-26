using System.Collections.Generic;
using System.Threading.Tasks;

namespace android.Services.Abstractions
{
    public interface ICacheService
    {
        IEnumerable<T> GetEntitiesFromInMemoryCache<T>(string key);
        void AddEntitiesToInMemoryCache<T>(string key, IEnumerable<T> entities);
        void AddEntitiesToPersistentCache<T>(string key, IEnumerable<T> entities);
        Task<IEnumerable<T>> GetEntitiesFromPersistentCache<T>(string key);
        void AddObjectToUserCache<T>(string key, T obj);
        Task<T> GetObjectFromUserCacheAsync<T>(string key) where T : class;
    }
}