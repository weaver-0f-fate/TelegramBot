using Services;
using System.Threading.Tasks;

namespace Task11 {
    public class Program {
        public static async Task Main(string[] args) {
            var bot = new TelegramReplyingService();

            await bot.StartListeningAsync();
        }
    }
}
