using Models;
using System;
using System.Threading.Tasks;

namespace Services.Interfaces {
    public interface ICacheService {
        public Task<CurrencyRates> GetOrCreateAsync(DateTime key, Func<DateTime, Task<CurrencyRates>> createItem);
    }
}
