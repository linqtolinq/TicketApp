using IdGen;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TicketAppApi.Controllers.ApiFormModel;
using TicketAppApi.Excepsion;
using TicketAppApi.Extension;
using TicketAppApi.GlobalSettng;
using TicketAppApi.JwtExtension;
using TicketAppApi.Model;
using TicketAppApi.MongoDbHelper;
using TicketAppApi.ServiceInject;
using static MongoDB.Driver.WriteConcern;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TicketAppApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize(Policy = JWTService.PolicyName, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class UserEntityController : ControllerBase
    {
        private readonly JWTTokenOptions jwtoptions;
        private readonly MemoryCache memoryCache;
        private readonly JwtInvorker jwtInvorker;
        private readonly IMongoDatabase mongo;
        private readonly IdGenerator Idgen;

        public UserEntityController(IOptions<JWTTokenOptions> jwtoptions,
            MemoryCache memoryCache,
            JwtInvorker jwtInvorker,
            IMongoDatabase mongo,
            IdGenerator Idgen)
        {
            this.jwtoptions = jwtoptions.Value;
            this.memoryCache = memoryCache;
            this.jwtInvorker = jwtInvorker;
            this.mongo = mongo;
            this.Idgen = Idgen;
        }
        // GET: api/<UserEntryController>
        [HttpGet]
        [PermissionFilter(Role.Admin)]
        public async Task<Result> GetUserLists(int pagenumber,int pagesize,string keywords)
        {
            if (keywords == "#")
            {
                var user = await mongo.DbGetCollection<FinancialStaff>().Find(s => s.Id == HttpContext.GetUserId()).FirstAsync();
                if (user.Role != Role.Admin)
                {
                    return Result.SuccessError("权限不足");
                }
                var data =await mongo.DbGetCollection<FinancialStaff>().Find(s => s.IsDeleted == false).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                var len =await mongo.DbGetCollection<FinancialStaff>().Find(s => s.IsDeleted == false).CountDocumentsAsync();
                return Result.Success().SetData(new {
                    tables = data.Select(s => {
                        s.Password = "";
                        return s;
                    }),
                    counts = len
                });
            }
            else if (!string.IsNullOrEmpty(keywords))
            {
                var user = await mongo.DbGetCollection<FinancialStaff>().Find(s => s.Id == HttpContext.GetUserId()).FirstAsync();
                if (user.Role != Role.Admin)
                {
                    return Result.SuccessError("权限不足");
                }
                var data =await mongo.DbGetCollection<FinancialStaff>().Find(s => s.Name.Contains(keywords) && s.IsDeleted == false).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                var len =await mongo.DbGetCollection<FinancialStaff>().Find(s => s.Name.Contains(keywords) && s.IsDeleted == false).CountDocumentsAsync();
                return Result.Success().SetData(new
                {
                    tables = data.Select(s => {
                        s.Password = "不告诉你";
                        return s;
                    }),
                    counts = len
                });
            }
            else
            {
                return Result.SuccessError("参数解析错误");
            }
           
        }
        /// <summary>
        /// 获取自己的账户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<Result> GetOwnInfo()
        {
            var user = await mongo.DbGetCollection<FinancialStaff>().Find(s => s.Id == HttpContext.GetUserId()).FirstAsync();
            user.Password = "不告诉你";
            return Result.Success().SetData(user);
        }
        // GET api/<UserEntryController>/5
        [HttpGet]
        public string TestAuth()
        {
            var str = HttpContext.User.FindFirstValue(ClaimTypes.Email) ?? "";
            //str += HttpContext.User.FindFirstValue(ClaimTypes.Role);
            str += HttpContext.User.FindFirstValue(ClaimTypes.Name);

            return str;
        }

        // POST api/<UserEntryController>
        [HttpGet]
        [AllowAnonymous]
        public string AllowAnonymous()
        {
            return "ok";
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<Result> Login([FromBody]EntityLoginDto entityLoginDto)
        {
            if (!ModelState.IsValid)
            {
                return Result.SuccessError("信息填写不完整");
            }
            var is_exit = await mongo.DbGetCollection<FinancialStaff>().Find(s => s.UserName == entityLoginDto.UserName && s.IsDeleted == false).AnyAsync();
            if (is_exit)
            {
                var user = await mongo.DbGetCollection<FinancialStaff>().Find(s => s.UserName == entityLoginDto.UserName && s.IsDeleted == false).FirstAsync();
                if (!user.JudgePassword(entityLoginDto.PassWord))
                {
                    return Result.SuccessError("账号密码错误");
                }
                var expiretime = Int64.Parse(Appsettings.app("JwtAuthorize:Expiration") ?? "0");
                var ip = HttpContext.GetClientIp();

                var token = jwtInvorker.GetTokenStr(new FinancialStaff()
                {
                    Name = user.Name,
                    Email = user.Email,
                    Id = user.Id,
                    Role = user.Role,
                }, s => new List<Claim>() { new Claim(ClaimTypes.Sid, s.Id.ToString()), new Claim(ClaimTypes.Name, s.Name), new Claim(ClaimTypes.Email, s.Email) ,new Claim(ClaimTypes.Role,$"{user.Role}")});
                HttpContext.Response.Headers.Add("jwt_token",token);
                HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "jwt_token");
                JWTService.Cache.Set("ticket_user_" + ip, new OnlineUser
                {
                    RemoteIp = ip,
                    LastRefreshTime = DateTime.Now
                }, TimeSpan.FromSeconds(expiretime + 10));
                LogHelper.LogInfo(user.Id, "登录管理平台");


                var filter = Builders<FinancialStaff>.Filter.Eq(s => s.Id, user.Id);
                var update = Builders<FinancialStaff>.Update
                    .Set(s => s.Ip, HttpContext.GetClientIp());
                await mongo.DbGetCollection<FinancialStaff>().UpdateOneAsync(filter,update);


                return Result.Success("登录成功").SetData(token);
            }
            return Result.SuccessError("该账号不存在");
        }
        [HttpPost]
        [PermissionFilter(Role.Admin)]
        public async Task<Result> AddUser(EntityAddDto entityAddDto)
        {
            if (!ModelState.IsValid)
            {
                return Result.SuccessError("信息填写不完整");
            }
            var user = await mongo.DbGetCollection<FinancialStaff>().Find(s => s.UserName == entityAddDto.UserName).AnyAsync();
            if (user)
            {
                var u = await mongo.DbGetCollection<FinancialStaff>().Find(s => s.UserName == entityAddDto.UserName).FirstAsync();
                if (u.IsDeleted ==false)
                {
                    return Result.SuccessError("该账号已存在");
                }
                else
                {
                    await mongo.DbGetCollection<FinancialStaff>().DeleteOneAsync(s=>s.Id == u.Id);
                    u.Ip = HttpContext.GetClientIp();
                    u.Email = entityAddDto.Email;
                    u.Name = entityAddDto.Name;
                    u.UserName = entityAddDto.UserName;
                    u.BuildPassword(entityAddDto.Password);
                    u.Nick = entityAddDto.Nick;
                    u.Remark = entityAddDto.Remark;
                    u.Address = entityAddDto.Address;
                    u.Role = entityAddDto.Role;
                    u.Id = Idgen.CreateId();
                    u.IsDeleted = false;
                    await mongo.DbGetCollection<FinancialStaff>().InsertOneAsync(u);


                    return Result.Success("操作成功");
                }
            }
            else
            {
                try
                {
                    var newuser = new FinancialStaff();
                    newuser.Ip = HttpContext.GetClientIp();
                    newuser.Email = entityAddDto.Email;
                    newuser.Name = entityAddDto.Name;
                    newuser.UserName = entityAddDto.UserName;
                    newuser.BuildPassword(entityAddDto.Password);
                    newuser.Nick = entityAddDto.Nick;
                    newuser.Remark = entityAddDto.Remark;
                    newuser.Address = entityAddDto.Address;
                    newuser.Role = entityAddDto.Role;
                    newuser.Id = Idgen.CreateId();


                    await mongo.DbGetCollection<FinancialStaff>().InsertOneAsync(newuser);
                    return Result.Success("操作成功");
                }
                catch (Exception)
                {
                    throw new Exception("添加用户失败");
                }
            }
        }
        /// <summary>
        /// 更新自己的账户信息
        /// </summary>
        /// <param name="entityupdateDto"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        public async Task<Result> UpdateOwnAccountInfo(EntityUpdateDto entityupdateDto)
        {
            if (!ModelState.IsValid)
            {
                return Result.SuccessError("信息填写不完整");
            }
            var user = await mongo.DbGetCollection<FinancialStaff>().Find(s => s.Id == HttpContext.GetUserId()).FirstAsync();
            try
            {
                var filter = Builders<FinancialStaff>.Filter.Eq(s=>s.Id, HttpContext.GetUserId());
                var update = Builders<FinancialStaff>.Update
                    .Set(s => s.Ip, HttpContext.GetClientIp())
                    .Set(s=>s.Email, entityupdateDto.Email)
                    .Set(s=>s.Name, entityupdateDto.Name)
                    .Set(s=>s.Nick, entityupdateDto.Nick)
                    .Set(s=>s.Remark, entityupdateDto.Remark)
                    .Set(s=>s.Address, entityupdateDto.Address);
                await mongo.DbGetCollection<FinancialStaff>().UpdateOneAsync(filter, update);
                return Result.Success("操作成功");
            }
            catch (Exception)
            {
                throw new Exception("添加用户失败");
            }

        }

        /// <summary>
        /// 注销自己的账户
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpDelete]
        public async Task<Result> SignOutOwnAccount()
        {
            if (!ModelState.IsValid)
            {
                return Result.SuccessError("信息填写不完整");
            }
            try
            {
                var filter = Builders<FinancialStaff>.Filter.Eq(s => s.Id, HttpContext.GetUserId());
                var update = Builders<FinancialStaff>.Update.Set(s => s.IsDeleted, true);
                await mongo.DbGetCollection<FinancialStaff>().UpdateOneAsync(filter, update);
                JWTService.Cache.Remove("ticket_user_" + HttpContext.GetClientIp());
                return Result.Success("操作成功");
            }
            catch (Exception)
            {
                throw new Exception("添加用户失败");
            }

        }
        [HttpDelete]
        public async Task<Result> AdminSignOutOwnAccount(long id)
        {
            if (!ModelState.IsValid)
            {
                return Result.SuccessError("信息填写不完整");
            }
            try
            {
                var filter = Builders<FinancialStaff>.Filter.Eq(s => s.Id, id);
                var update = Builders<FinancialStaff>.Update.Set(s => s.IsDeleted, true);
                await mongo.DbGetCollection<FinancialStaff>().UpdateOneAsync(filter, update);
                JWTService.Cache.Remove("ticket_user_" + HttpContext.GetClientIp());
                return Result.Success("操作成功");
            }
            catch (Exception)
            {
                throw new Exception("添加用户失败");
            }

        }
        /// <summary>
        /// 修改账号密码
        /// </summary>
        /// <param name="entityupdateDto"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        public async Task<Result> UpdateOwnAccountPwd(EntityUpdatePwdDto entityupdateDto)
        {
            if (!ModelState.IsValid)
            {
                return Result.SuccessError("信息填写不完整");
            }
            if (entityupdateDto.NewPwd != entityupdateDto.NewConfirmPwd)
            {
                return Result.SuccessError("新密码不一致");
            }
            var user = await mongo.DbGetCollection<FinancialStaff>().Find(s => s.Id == HttpContext.GetUserId()).FirstAsync();
            if (user == null || user.JudgePassword(entityupdateDto.OldPwd) == false)
            {
                return Result.SuccessError("旧密码错误");
            }
            user.BuildPassword(entityupdateDto.NewPwd);
            try
            {
                var filter = Builders<FinancialStaff>.Filter.Eq(s => s.Id, HttpContext.GetUserId());
                var update = Builders<FinancialStaff>.Update
                    .Set(s => s.Ip, HttpContext.GetClientIp())
                    .Set(s => s.Password, user.Password).Set(s=>s.Salt,user.Salt);
                await mongo.DbGetCollection<FinancialStaff>().UpdateOneAsync(filter, update);
                return Result.Success("操作成功");
            }
            catch (Exception)
            {
                throw new Exception("修改失败");
            }

        }
    }
}
