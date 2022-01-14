using Models;
using Services.Properties;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services {
    public class CurrencyRateService {
        private CacheService _cacheService;
        private HttpClient _client;


        public CurrencyRateService(CacheService cacheService) {
            _cacheService = cacheService;
            _client = new HttpClient();
        }

        /// <summary>
        /// Get either information about asked currency rate related to UAH, or Exception message
        /// </summary>
        /// <param name="inputMessage">Message which contains 2 strings, target currency and date (example: "USD 13.01.2020")</param>
        /// <returns>information about asked currency rate or ExceptionMessage</returns>
        public async Task<string> GetCurrencyRateAsync(string inputMessage) {
            try {
                TryDeserializeInput(inputMessage, out var currencyCode, out var date);

                var currencyRates = await _cacheService.GetOrCreateAsync(date, GetUAHRatesOnSpicifiedDateAsync);

                var currencyRate = GetRequestedCurrencyRate(currencyRates, currencyCode);
                var averageCurrencyRate = GetAverageCurrencyRate(currencyRate);

                return $"{date.ToShortDateString()}: 1 {currencyCode} = {averageCurrencyRate} UAH";
            }
            catch(Exception ex) {
                return ex.Message;
            }
        }

        private bool TryDeserializeInput(string inputMessage, out string currencyCode, out DateTime date) {
            const int MinimalNumberOfArguments = 2;
            var strings = inputMessage.Split(' ');

            //according to input pattern first string should be currency code and second date.
            if(strings.Length >= MinimalNumberOfArguments && DateTime.TryParse(strings[1], out date)) {
                currencyCode = strings[0].ToUpper();
                return true;
            }
            throw new Exception($"{Resources.InvalidInputException}. {Resources.InputPatternMessage}."); 
        }

        private CurrencyRate GetRequestedCurrencyRate(CurrencyRates currencyRates, string currencyCode) {
            var currencyRate = currencyRates
                .ExchangeRates
                .Where(x => x.Currency == currencyCode)
                .FirstOrDefault();

            if (currencyRate is null) {
                throw new Exception($"{Resources.NoDataExceptionMessage}.");
            }
            return currencyRate;
        }

        private async Task<CurrencyRates> GetUAHRatesOnSpicifiedDateAsync(DateTime date) {
            var stringDate = $"&date={date.Day}.{date.Month}.{date.Year}";
            var uri = new Uri(Resources.ApiURL + stringDate);
            var streamTask = _client.GetStreamAsync(uri);

            return await JsonSerializer.DeserializeAsync<CurrencyRates>(await streamTask);
        }

        private double GetAverageCurrencyRate(CurrencyRate rate) {
            return (rate.PurchaseRateNB + rate.SaleRateNB) / 2; 
        }
    }
}
