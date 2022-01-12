using Models;
using Services.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services {
    public class GetCurrencyRateService {

        /// <summary>
        /// Get either information about asked currency rate related to UAH, or Exception message
        /// </summary>
        /// <param name="inputMessage">Message which contains 2 strings, target currency and date (example: "USD 13.01.2020")</param>
        /// <returns>information about asked currency rate or ExceptionMessage</returns>
        public async Task<string> GetData(string inputMessage) {
            try {
                DeserializeInput(inputMessage, out var currencyCode, out var date);

                var currencyRates = await GetUAHRateOnSpicifiedDate(date);
                var currencyRate = GetRequiredCurrencyRate(currencyRates, currencyCode);
                var data = GetAverageCurrencyRate(currencyRate);

                return $"UAH to {currencyCode} rate:\n{data}";
            }
            catch(Exception ex) {
                return ex.Message;
            }
        }

        private void DeserializeInput(string inputMessage, out string currencyCode, out DateTime date) {
            var strings = inputMessage.Split(' ');

            if (strings.Length < 2) {
                throw new Exception($"{Resources.InvalidInputException}. {Resources.InputPatternMessage}.");
            }

            if (DateTime.TryParse(strings[1], out date)) {
                currencyCode = strings[0];
            }
            else {
                throw new Exception($"{Resources.InvalidDateExceptionMessage}. {Resources.InputPatternMessage}.");
            }
        }

        private CurrencyRate GetRequiredCurrencyRate(CurrencyRates currencyRates, string currencyCode) {
            var currencyRate = currencyRates
                .ExchangeRates
                .Where(x => x.Currency == currencyCode)
                .FirstOrDefault();

            if (currencyRate is null) {
                throw new Exception($"{Resources.NoDataExceptionMessage}.");
            }

            return currencyRate;
        }

        private async Task<CurrencyRates> GetUAHRateOnSpicifiedDate(DateTime date) {
            var client = new HttpClient();
            var uri = BuildUri(date);
            var streamTask = client.GetStreamAsync(uri);
            var currencyRates = await JsonSerializer.DeserializeAsync<CurrencyRates>(await streamTask);

            return currencyRates;
        }

        private Uri BuildUri(DateTime date) {
            var stringDate = $"&date={date.Day}.{date.Month}.{date.Year}";
            return new Uri(Resources.ApiURL + stringDate);
        }

        private double GetAverageCurrencyRate(CurrencyRate rate) {
            return (rate.PurchaseRateNB + rate.SaleRateNB) / 2;
        }
    }
}
