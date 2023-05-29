using Ocelot.Middleware;
using TicketAppApi.Extension;
using Microsoft.AspNetCore.Authentication;
using TicketAppApi.ServiceInject;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TicketAppApi.GlobalSettng;
using TicketAppApi.JwtExtension;
using TicketAppApi.MongoDbHelper;
using Microsoft.Extensions.FileProviders;
using System.Text.Json;
using TicketAppApi.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.InjectAppsettingConf();//����ȫ�������ļ�

builder.Configuration.AddCommandLine(args);
builder.WebHost.UseUrls(builder.Configuration.GetValue<string>("StartUrl"));
builder.Services.IdGenInit();

builder.Services.AddControllers().AddJsonOptions(
        options => {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Converters.Add(new NumberConverter());
        }); 


builder.Services.AddJwtService();
builder.Services.AddJWTAuthorizationService();
builder.Services.AddSignalR();
builder.Services.AddTransient<JWTAutoRefreshMiddleWare>();


builder.Services.AddCorsService();
builder.Services.DBInit();
builder.Services.LogInit();


var app = builder.Build();
///�м��λ��Ҫ�����ͷ���ſ�����������
app.UseMiddleware<ErrorHandExtension>();


app.UseRouting();
app.UseCorsService();

app.UseJWTService();
app.UseStaticFiles();
//app.UseOcelot();


app.UseEndpoints(op => {
    op.MapControllers();
    op.MapHub<SignalHubService>("/hubs/SignalHubService").RequireCors(t => t.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
    op.Map("/",()=> Results.Redirect("/staticfiles/404.html#")).RequireCors(t => t.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
});



app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
    RequestPath = "/StaticFiles"
});

app.UseMiddleware<JWTAutoRefreshMiddleWare>();
app.Hellomsg();

app.Run();

