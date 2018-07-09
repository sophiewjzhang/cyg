using Newtonsoft.Json;
using services.abstractions.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace services.abstractions
{
    public abstract class BaseDataService
    {
        private readonly ICacheService cacheService;
        protected readonly Uri baseUrl;
        protected readonly int apiTimeoutInSeconds;

        public BaseDataService(ICacheService cacheService, string baseUrl, int apiTimeoutInSeconds)
        {
            this.cacheService = cacheService;
            this.baseUrl = new Uri(baseUrl);
            this.apiTimeoutInSeconds = apiTimeoutInSeconds;
        }

        protected IEnumerable<T> GetEntitiesFromCache<T>(string key)
        {
            return cacheService.GetEntitiesFromInMemoryCache<T>(key);
        }

        protected async Task<IEnumerable<T>> GetEntitiesFromPersistentCache<T>(string key)
        {
            return await cacheService.GetEntitiesFromPersistentCache<T>(key);
        }

        protected async Task<IEnumerable<T>> GetEntitiesFromApiAsync<T>(Uri uri)
        {
            using (var client = new HttpClient(new HttpRetryMessageHandler(new HttpClientHandler())))
            {
                client.Timeout = TimeSpan.FromSeconds(apiTimeoutInSeconds);
                try
                {
                    var response = await client.GetAsync(uri);
                    using (var streamReader = new StreamReader(await response.Content.ReadAsStreamAsync()))
                    {
                        return JsonConvert.DeserializeObject<IEnumerable<T>>(await streamReader.ReadToEndAsync());
                    }
                }
                catch (HttpRequestException)
                {
                    throw new NoInternetConnectionException();
                }
                catch (TaskCanceledException)
                {
                    throw new ConnectionToServerLostException();
                }
                catch
                {
                    throw new GenericNoConnectionException();
                }
            }
        }

        protected async Task<T> GetEntityFromApiAsync<T>(Uri uri)
        {
            using (var client = new HttpClient(new HttpRetryMessageHandler(new HttpClientHandler())))
            {
                client.Timeout = TimeSpan.FromSeconds(apiTimeoutInSeconds);
                try
                {
                    var response = await client.GetAsync(uri);
                    using (var streamReader = new StreamReader(await response.Content.ReadAsStreamAsync()))
                    {
                        var result = await streamReader.ReadToEndAsync();
                        return JsonConvert.DeserializeObject<T>(result);
                    }
                }
                catch (HttpRequestException)
                {
                    throw new NoInternetConnectionException();
                }
                catch (TaskCanceledException)
                {
                    throw new ConnectionToServerLostException();
                }
                catch
                {
                    throw new GenericNoConnectionException();
                }
            }
        }

        public async Task<IEnumerable<T>> GetCachedAsync<T>(string relativeUrl)
        {
            var entities = GetEntitiesFromCache<T>(relativeUrl);
            if (entities == null || !entities.Any())
            {
                entities = await GetEntitiesFromPersistentCache<T>(relativeUrl);
                if (entities == null || !entities.Any())
                {
                    entities = await GetRemoteAsync<T>(relativeUrl);
                    cacheService.AddEntitiesToPersistentCache<T>(relativeUrl, entities);
                }
                cacheService.AddEntitiesToInMemoryCache<T>(relativeUrl, entities);
                return entities;
            }
            return entities;
        }

        public async Task<IEnumerable<T>> GetRemoteAsync<T>(string relativeUrl)
        {
            return await GetEntitiesFromApiAsync<T>(new Uri(baseUrl, relativeUrl));
        }
    }
}