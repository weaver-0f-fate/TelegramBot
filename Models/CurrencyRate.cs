using System.Text.Json.Serialization;

namespace Models {
    public class CurrencyRate {
        [JsonPropertyName("baseCurrency")]
        public string BaseCurrency { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("saleRateNB")]
        public double SaleRateNB { get; set; }

        [JsonPropertyName("purchaseRateNB")]
        public double PurchaseRateNB { get; set; }
    }
}
