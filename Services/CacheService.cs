using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services {
    public class CacheService {
        List<CurrencyRates> _currencyRatesCache;

        public CacheService() {
            _currencyRatesCache = new List<CurrencyRates>();
        }



    }
}
