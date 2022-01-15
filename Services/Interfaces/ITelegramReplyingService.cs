using System.Threading.Tasks;

namespace Services.Interfaces {
    public interface ITelegramReplyingService {
        public Task StartListeningAsync();
    }
}
