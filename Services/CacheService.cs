using Microsoft.Extensions.Caching.Memory;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services {
    public class CacheService {
        private MemoryCache _cache;

        public CacheService() {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }
 
        public async Task<CurrencyRates> GetOrCreateAsync(DateTime key, Func<DateTime, Task<CurrencyRates>> createItem) {
            const int SecondsToExpiration = 30;
            CurrencyRates cacheEntry;


            if (!_cache.TryGetValue(key, out cacheEntry)) {
                cacheEntry = await createItem(key);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(SecondsToExpiration));

                // Save data in cache.
                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }
            return cacheEntry;
        }


        
    }
}
