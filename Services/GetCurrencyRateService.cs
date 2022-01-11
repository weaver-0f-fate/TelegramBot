using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Services {
    public class GetCurrencyRateService {
        private static readonly HttpClient client = new HttpClient();

        public async Task<string> GetData(string input) {
            if (string.IsNullOrEmpty(input)) {
                throw new Exception();
            }


            var x = await ProcessRepositories(BuildUri("test")); 


            return x;
        }

        private static async Task<string> ProcessRepositories(string URI) {
            client.DefaultRequestHeaders.Accept.Clear();

            var stringTask = client.GetStringAsync(URI);

            var msg = await stringTask;
            return msg;
        }

        private string BuildUri(string userInput) {
            if (string.IsNullOrEmpty(userInput)) {
                throw new Exception();
            }
            if (CurrencyCodeIsInvalid()) {
                throw new Exception();
            }
            if (DateIsInvalid()) {
                throw new Exception();
            }


            return "https://api.privatbank.ua/p24api/exchange_rates?json&date=01.12.2014";
        }

        private bool CurrencyCodeIsInvalid() {
            return false;
        }

        private bool DateIsInvalid() {
            return false;
        }
    }
}
