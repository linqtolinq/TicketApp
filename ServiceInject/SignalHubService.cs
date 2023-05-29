using Microsoft.AspNetCore.SignalR;
using ServiceStack.Logging;
using TicketAppApi.Extension;

namespace TicketAppApi.ServiceInject
{
    public class SignalHubService : Hub<IClock>
    {
        public SignalHubService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SignalHubService>();
        }

        private readonly ILogger<SignalHubService> _logger;

        public override System.Threading.Tasks.Task OnConnectedAsync()
        {
            _logger.LogInformation($"one client connected with ip:[{Context.GetHttpContext().Connection.RemoteIpAddress}]");

            return base.OnConnectedAsync();
        }
        public override System.Threading.Tasks.Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"one client disconnect with ip:[{Context.GetHttpContext().Connection.RemoteIpAddress}]");
            return base.OnDisconnectedAsync(exception);
        }
        public string Start(string id)
        {
            return $"Welcome [ {id} ] to connect!";
        }
    }
    public interface IClock
    {
        System.Threading.Tasks.Task ShowTime(DateTime currentTime);
        System.Threading.Tasks.Task DeviceSync(string info, DateTime currentTime);
        System.Threading.Tasks.Task UnitSync(string info, DateTime currentTime);
        System.Threading.Tasks.Task GroupSync(string info, DateTime currentTime);
        System.Threading.Tasks.Task PushMessageSync(string info, int id, DateTime currentTime);
        System.Threading.Tasks.Task UserAccountInfoSync(string info, DateTime currentTime);
        System.Threading.Tasks.Task EmqxDataStreamSync(string info, DateTime currentTime);
    }

}
