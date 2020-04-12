using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using TRUEbot.Bot;

namespace TRUEbot.Web.Services
{
    public class BotHostedService : IHostedService
    {
        private readonly IBotClient _botClient;

        public BotHostedService(IBotClient botClient)
        {
            _botClient = botClient;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _botClient.Start();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _botClient.Stop();
        }
    }
}
