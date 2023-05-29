using Microsoft.Extensions.Configuration;
using TicketAppApi.GlobalSettng;

namespace TicketAppApi.ServiceInject
{
    public static class AppsettingService
    {
        public static void InjectAppsettingConf(this IServiceCollection services)
        {
            IConfiguration configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            services.AddSingleton(new Appsettings(configuration));
        }
    }
}
