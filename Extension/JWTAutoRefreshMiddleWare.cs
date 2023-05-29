using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using Ocelot.Responses;
using System.Security.Claims;
using TicketAppApi.GlobalSettng;
using TicketAppApi.JwtExtension;
using TicketAppApi.Model;
using TicketAppApi.ServiceInject;

namespace TicketAppApi.Extension
{
    public class JWTAutoRefreshMiddleWare : IMiddleware
    {
        private readonly JwtInvorker jwtInvorker;

        public JWTAutoRefreshMiddleWare(JwtInvorker jwtInvorker)
        {
            this.jwtInvorker = jwtInvorker;
        }

        /// <summary>
        /// 自动刷新jwt颁发的token
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            var reexpiretime = Int64.Parse(Appsettings.app("JwtAuthorize:ReExpiration") ?? "0");
            var expiretime = Int64.Parse(Appsettings.app("JwtAuthorize:Expiration") ?? "0");
            var ip = context.GetClientIp();
            var user =  JWTService.Cache.Get<OnlineUser>("ticket_user_" + ip);
            if (user is not null && user.LastRefreshTime.AddSeconds(reexpiretime) < DateTime.Now /*toekn自动刷新*/)
            {
                context.Response.Headers.Add("jwt_token", jwtInvorker.GetTokenStr(new FinancialStaff()
                {
                    Name = context.User.FindFirstValue(ClaimTypes.Name),
                    Email = context.User.FindFirstValue(ClaimTypes.Email),
                    Id = Int64.Parse(context.User.FindFirstValue(ClaimTypes.Sid)??"0"),
                    Role = (Role)int.Parse(context.User.FindFirstValue(ClaimTypes.Role)) 
                },
            s => new List<Claim>() {
                new Claim(ClaimTypes.Sid, s.Id.ToString()),
                new Claim(ClaimTypes.Name, s.Name),
                new Claim(ClaimTypes.Email, s.Email),
                new Claim(ClaimTypes.Role, $"{s.Role}")
            }));
                context.Response.Headers.Add("Access-Control-Expose-Headers", "jwt_token");

                JWTService.Cache.Set("ticket_user_" + ip, new OnlineUser
                {
                    RemoteIp = ip,
                    LastRefreshTime = DateTime.Now
                }, TimeSpan.FromSeconds(expiretime+10));
            }
            else if (context.User.HasClaim(s=>s.Type == ClaimTypes.Sid) && context.User.HasClaim(s => s.Type == ClaimTypes.Sid) && context.User.HasClaim(s => s.Type == ClaimTypes.Sid) && context.User.HasClaim(s => s.Type == ClaimTypes.Role))
            {
                JWTService.Cache.Set("ticket_user_" + ip, new OnlineUser
                {
                    RemoteIp = ip,
                    LastRefreshTime = DateTime.Now
                }, TimeSpan.FromSeconds(expiretime+10));
            }
            await next(context);
        }

        public T? GetOnlineUser<T>(HttpContext context)
        {
            var t = JWTService.Cache.Get<T>(context.GetClientIp());
            return t;
        }

    }

    public class OnlineUser
    {
        public string RemoteIp { get; set; }
        /// <summary>
        /// 最后刷新时间
        /// </summary>
        public DateTime LastRefreshTime { get; set; }
    }
}
