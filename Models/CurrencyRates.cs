using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Models {
    public class CurrencyRates {
        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("exchangeRate")]
        public List<CurrencyRate> ExchangeRates { get; set; }
    }
}
