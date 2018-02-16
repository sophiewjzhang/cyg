﻿using System;
using System.Collections.Generic;
using android.Services.Abstractions;
using DTO;
using System.Threading.Tasks;
using android.Configuration;

namespace android.Services
{
    public class RouteDataService : BaseDataService, IRouteDataService
    {
        private const string AllRoutesCacheKey = "Routes";

        public RouteDataService(ICacheService cacheService, string baseUrl) : base(cacheService, baseUrl)
        {
        }

        public async Task<IEnumerable<Route>> GetRoutesAsync()
        {
            return await GetAsync<Route>(AllRoutesCacheKey, "route");
        }
    }
}