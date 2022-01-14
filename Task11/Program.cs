using Services;
using System.Threading.Tasks;

namespace Task11 {
    public class Program {
        public static async Task Main(string[] args) {
            var cacheService = new CacheService();
            var currencyRateService = new CurrencyRateService(cacheService);
            var bot = new TelegramReplyingService(currencyRateService);

            await bot.StartListeningAsync();
        }
    }
}
