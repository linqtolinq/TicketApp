using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using TicketAppApi.Model;

namespace TicketAppApi.Excepsion
{
    public class PermissionFilter:ActionFilterAttribute
    {
        private Role role { get; set; }
        public PermissionFilter(Role role)
        {
            this.role = role;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //过滤非admin权限
            if (context.HttpContext.User.FindFirstValue(ClaimTypes.Role) != Role.Admin.ToString()
                && this.role == Role.Admin)
            {
                context.HttpContext.Response.StatusCode = 404;
            }
            else
            {
                base.OnActionExecuting(context);
            }
        }
        
    }
}
