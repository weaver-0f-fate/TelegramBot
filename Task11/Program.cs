using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;
using Services.Interfaces;
using System.Threading.Tasks;

namespace Task11 {
    public class Program {
        public static async Task Main(string[] args) {

            using var host = CreateHostBuilder(args).Build();
            var bot = host.Services.GetRequiredService<ITelegramReplyingService>();

            await bot.StartListeningAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    services.AddTransient<ICacheService, CacheService>()
                            .AddTransient<ICurrencyRateService, CurrencyRateService>()
                            .AddTransient<ITelegramReplyingService, TelegramReplyingService>());
        }
    }
}
