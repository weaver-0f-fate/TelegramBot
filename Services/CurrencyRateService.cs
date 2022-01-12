using Models;
using Services.Properties;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services {
    public static class CurrencyRateService {

        /// <summary>
        /// Get either information about asked currency rate related to UAH, or Exception message
        /// </summary>
        /// <param name="inputMessage">Message which contains 2 strings, target currency and date (example: "USD 13.01.2020")</param>
        /// <returns>information about asked currency rate or ExceptionMessage</returns>
        public static async Task<string> GetCurrencyRate(string inputMessage) {
            try {
                var currencyCode = DeserializeInput(inputMessage, out var date);
                var currencyRate = await GetRequiredCurrencyRate(currencyCode, date);
                var data = GetAverageCurrencyRate(currencyRate);

                return $"{date.ToShortDateString()}: 1 {currencyCode} = {data} UAH";
            }
            catch(Exception ex) {
                return ex.Message;
            }
        }

        private static string DeserializeInput(string inputMessage, out DateTime date) {
            var strings = inputMessage.Split(' ');

            if (strings.Length < 2) {
                throw new Exception($"{Resources.InvalidInputException}. {Resources.InputPatternMessage}.");
            }

            if (DateTime.TryParse(strings[1], out date)) {
                return strings[0].ToUpper();
            }
            else {
                throw new Exception($"{Resources.InvalidDateExceptionMessage}. {Resources.InputPatternMessage}.");
            }
        }

        private static async Task<CurrencyRate> GetRequiredCurrencyRate(string currencyCode, DateTime date) {
            var currencyRates = await GetUAHRatesOnSpicifiedDate(date);

            var currencyRate = currencyRates
                .ExchangeRates
                .Where(x => x.Currency == currencyCode)
                .FirstOrDefault();

            if (currencyRate is null) {
                throw new Exception($"{Resources.NoDataExceptionMessage}.");
            }
            return currencyRate;
        }

        private static async Task<CurrencyRates> GetUAHRatesOnSpicifiedDate(DateTime date) {
            var client = new HttpClient();
            var uri = BuildUri(date);
            var streamTask = client.GetStreamAsync(uri);
            return await JsonSerializer.DeserializeAsync<CurrencyRates>(await streamTask);
        }

        private static Uri BuildUri(DateTime date) {
            var stringDate = $"&date={date.Day}.{date.Month}.{date.Year}";
            return new Uri(Resources.ApiURL + stringDate);
        }

        private static double GetAverageCurrencyRate(CurrencyRate rate) {
            return (rate.PurchaseRateNB + rate.SaleRateNB) / 2;
        }
    }
}
