﻿using System.Dynamic;
using Microsoft.Extensions.Configuration;

namespace android.Configuration
{
    public class AndroidConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public int PermanentCacheExpirationInDays { get; set; }
        public int InMemoryCacheExpirationInHours { get; set; }
        public int ApiTimeoutInSeconds { get; set; }
        public string CacheDatabaseName { get; set; }
        public string ServiceGuaranteeUrl { get; set; }
        public int EligibilityDaysAvailable { get; set; }
    }
}