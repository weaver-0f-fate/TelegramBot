using System.Threading.Tasks;

namespace Services.Interfaces {
    public interface ICurrencyRateService {
        public Task<string> GetCurrencyRateAsync(string inputMessage);
    }
}
