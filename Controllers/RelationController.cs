using Amazon.Auth.AccessControlPolicy;
using IdGen;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MQTTDashWebApi.Extension;
using TicketAppApi.Extension;
using TicketAppApi.JwtExtension;
using TicketAppApi.Model;
using TicketAppApi.MongoDbHelper;
using TicketAppApi.ServiceInject;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TicketAppApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Policy = JWTService.PolicyName, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RelationController : ControllerBase
    {
        public RelationController(IOptions<JWTTokenOptions> jwtoptions, MemoryCache memoryCache, JwtInvorker jwtInvorker, IMongoDatabase mongo, IdGenerator idgen)
        {
            this.jwtoptions = jwtoptions.Value;
            this.memoryCache = memoryCache;
            this.jwtInvorker = jwtInvorker;
            this.mongo = mongo;
            Idgen = idgen;
        }
        private readonly JWTTokenOptions jwtoptions;
        private readonly MemoryCache memoryCache;
        private readonly JwtInvorker jwtInvorker;
        private readonly IMongoDatabase mongo;
        private readonly IdGenerator Idgen;

        // GET api/<RelationController>/5
        /// <summary>
        /// 获取一个用户的所有负责业务部门
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Result> GetAuserAllBusinessDepartment(long id)
        {
            var is_user_exist = await mongo.DbGetCollection<FinancialStaff>().Find(s => s.Id == id).AnyAsync();
            if (!is_user_exist)
            {
                return Result.SuccessError("无数据");
            }
            else
            {
                var res = await mongo.DbGetCollection<Relation>().Find(s => s.FinancialStaffId == id).ToListAsync();
                if (res.Count > 0)
                {
                    var filter = Builders<BusinessDepartment>.Filter.Or(res.Select(s => Builders<BusinessDepartment>.Filter.Eq(s1 => s1.Id, s.BusinessDepartmentId)));
                    return Result.Success("获取成功").SetData(await mongo.DbGetCollection<BusinessDepartment>().Find(filter).ToListAsync());
                }
                else
                {
                    return Result.Success("无数据");
                }
            }
        }
        [HttpGet]
        public async Task<Result> getAuserOwnAllBusinessDepartment(string keywords,int pagepagenumber,int pagesize)
        {
            var id = HttpContext.GetUserId();
            if (id == 0)
            {
                return Result.SuccessError("解析数据错误");
            }
            var is_user_exist = await mongo.DbGetCollection<FinancialStaff>().Find(s => s.Id == id).AnyAsync();
            if (!is_user_exist)
            {
                return Result.SuccessError("无数据");
            }
            else
            {
                var res = await mongo.DbGetCollection<Relation>().Find(s => s.FinancialStaffId == id).ToListAsync();
                if (res.Count > 0)
                {
                    var filter = Builders<BusinessDepartment>.Filter.Or(res.Select(s => Builders<BusinessDepartment>.Filter.Eq(s1 => s1.Id, s.BusinessDepartmentId)));

                    if (keywords != "#")
                    {
                        var data = await mongo.DbGetCollection<BusinessDepartment>().Find(

                           Builders<BusinessDepartment>.Filter.And(
                               Builders<BusinessDepartment>.Filter.Regex(s => s.Name, new BsonRegularExpression($".*{keywords}.*", "i")),
                               filter
                               )).Skip(pagesize * (pagepagenumber - 1)).Limit(pagesize).ToListAsync();

                        var len = await mongo.DbGetCollection<BusinessDepartment>().Find(

                           Builders<BusinessDepartment>.Filter.And(
                               Builders<BusinessDepartment>.Filter.Regex(s => s.Name, new BsonRegularExpression($".*{keywords}.*", "i")),
                               filter
                               )).Skip(pagesize * (pagepagenumber - 1)).Limit(pagesize).CountDocumentsAsync();
                        return Result.Success("获取成功").SetData(new { tables = data,counts = len});
                    }
                    else
                    {
                        var data = await mongo.DbGetCollection<BusinessDepartment>().Find(filter).Skip(pagesize * (pagepagenumber - 1)).Limit(pagesize).ToListAsync();
                        var len = await mongo.DbGetCollection<BusinessDepartment>().Find(filter).Skip(pagesize * (pagepagenumber - 1)).Limit(pagesize).CountDocumentsAsync();
                        return Result.Success("获取成功").SetData(new { tables = data, counts = len });
                    }
                  
                }
                else
                {
                    return Result.Success("无数据");
                }
            }
        }
        
        // POST api/<RelationController>
        [HttpPost]
        public async Task<Result> UpdateAuserAllBusinessDepartment(long uid,string newids)
        {
            var is_user_exist = await mongo.DbGetCollection<FinancialStaff>().Find(s => s.Id == uid).AnyAsync();
            if (!is_user_exist)
            {
                return Result.SuccessError("无数据");
            }
            else
            {
                var res = await mongo.DbGetCollection<Relation>().DeleteManyAsync(s => s.FinancialStaffId == uid);

                if (newids != "[]" && newids != "[0]" && newids != "['0']" && newids != "[\"0\"]")
                {
                    try
                    {
                        var relatios = new List<Relation>();

                        foreach (var item in newids.GetModelFromJsonString<long[]>())
                        {
                            relatios.Add(new Relation { 
                                Id = Idgen.CreateId(),
                                FinancialStaffId = uid,
                                BusinessDepartmentId = item
                            });
                        }
                        await mongo.DbGetCollection<Relation>().InsertManyAsync(relatios);
                        return res.IsAcknowledged ?  Result.Success("处理成功"): Result.SuccessError("处理失败");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"出错了，{ex.Message}，发声位置：{ex.StackTrace}");
                        return Result.SuccessError("处理失败");
                    }
                   
                }
                else
                {
                    return res.IsAcknowledged ? Result.Success("处理成功") : Result.SuccessError("处理失败");
                }
            }
        }
        [HttpDelete]
        public async Task<Result> DeletAuserAllBusinessDepartment(long uid, string businessids)
        {
            var is_user_exist = await mongo.DbGetCollection<FinancialStaff>().Find(s => s.Id == uid).AnyAsync();

            if (!is_user_exist)
            {
                return Result.SuccessError("参数解析异常");
            }
            else
            {
                if (businessids == "[]" || businessids == "[0]" || businessids == "['0']" || businessids == "[\"0\"]")
                {
                    return Result.SuccessError("参数解析异常");
                }
                else
                {
                    try
                    {
                        var filter = Builders<Relation>.Filter.Or(businessids.GetModelFromJsonString<long[]>().Select(s =>
                        Builders<Relation>.Filter.And(
                        Builders<Relation>.Filter.Eq(s1 => s1.BusinessDepartmentId, s),
                         Builders<Relation>.Filter.Eq(s1 => s1.FinancialStaffId, uid)
                        )));
                        var res = await mongo.DbGetCollection<Relation>().DeleteManyAsync(filter);
                        return res.IsAcknowledged ? Result.Success("处理成功") : Result.SuccessError("处理失败");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"出错了，{ex.Message}，发声位置：{ex.StackTrace}");
                        return Result.SuccessError("处理失败");
                    }
                }
            }
        }
    }
}
