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
using Newtonsoft.Json;
using System;
using TicketAppApi.Controllers.ApiFormModel;
using TicketAppApi.Excepsion;
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
    public class BusinessDepartmentController : ControllerBase
    {
        public BusinessDepartmentController(IOptions<JWTTokenOptions> jwtoptions, MemoryCache memoryCache, JwtInvorker jwtInvorker, IMongoDatabase mongo, IdGenerator idgen)
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
        // GET: api/<CompanyController>
        /// <summary>
        /// 获取业务部列表集合
        /// </summary>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        [HttpGet]
        //[PermissionFilter(Role.Admin)]
        public async Task<Result> GetBusinessDepartmentList(string comids, int pagenumber, int pagesize, string keywords)
        {
            try
            {
                if (comids == "[]" || comids == "[0]" || comids == "['0']" || comids == "[\"0\"]")
                {
                    if (keywords == "#")
                    {
                        var data = await mongo.DbGetCollection<BusinessDepartment>().Find(s => true).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                        var len = await mongo.DbGetCollection<BusinessDepartment>().Find(s => true).CountDocumentsAsync();
                        return Result.Success().SetData(new
                        {
                            tables = data,
                            counts = len
                        });
                    }
                    else if (!string.IsNullOrEmpty(keywords))
                    {
                        var data = await mongo.DbGetCollection<BusinessDepartment>().Find(s => s.Name.Contains(keywords)).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                        var len = await mongo.DbGetCollection<BusinessDepartment>().Find(s=>s.Name.Contains(keywords)).CountDocumentsAsync();
                        return Result.Success().SetData(new
                        {
                            tables = data,
                            counts = len
                        });
                    }
                    else
                    {
                        return Result.SuccessError("参数解析错误");
                    }
                }
                else
                {
                    if (keywords == "#")
                    {
                        List<FilterDefinition<BusinessDepartment>> filters = new List<FilterDefinition<BusinessDepartment>>();
                        //filters.Add(Builders<BusinessDepartment>.Filter.Regex(s => s.Name, new BsonRegularExpression($".*{keywords}.*", "i")));
                        filters.AddRange(comids.GetModelFromJsonString<long[]>().Select(s => Builders<BusinessDepartment>.Filter.Eq(s1 => s1.CompanyId, s)));
                        var filter = Builders<BusinessDepartment>.Filter.Or(filters);
                        var data = await mongo.DbGetCollection<BusinessDepartment>().Find(filter).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();

                        var len = await mongo.DbGetCollection<BusinessDepartment>().Find(filter).CountDocumentsAsync();
                        return Result.Success().SetData(new { tables = data,counts = len});
                    }
                    else
                    {
                        List <FilterDefinition<BusinessDepartment>> filters = new List<FilterDefinition<BusinessDepartment>>();
                        filters.AddRange(comids.GetModelFromJsonString<long[]>().Select(s => Builders<BusinessDepartment>.Filter.Eq(s1 => s1.CompanyId, s)));

                        var filtersABCD = Builders<BusinessDepartment>.Filter.And(
                            Builders<BusinessDepartment>.Filter.Or(filters),
                             Builders<BusinessDepartment>.Filter.Regex(s => s.Name, new BsonRegularExpression($".*{keywords}.*", "i"))
                        );
                        var len = await mongo.DbGetCollection<BusinessDepartment>().Find(filtersABCD).CountDocumentsAsync();
                        var data = await mongo.DbGetCollection<BusinessDepartment>().Find(filtersABCD).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                        return Result.Success().SetData(new { tables = data, counts = len });
                    }

                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误{ex.Message}+发生位置:{ex.StackTrace}");
                return Result.SuccessError("参数解析错误");
            }

        }
        // GET api/<CompanyController>/5
        /// <summary>
        /// 获取业务部详细信息（包含project信息）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        //[PermissionFilter(Role.Admin)]
        public async Task<Result> GetOneBusinessDepartmentAllInfo(long id)
        {

            var business = await mongo.DbGetCollection<BusinessDepartment>().Find(s => s.Id == id).FirstAsync();

            Company company = new Company();

            if (business is not null)
            {
                company = await mongo.DbGetCollection<Company>().Find(s => s.Id == business.CompanyId).FirstAsync();
                var busi = new
                {
                    Company = company,
                    Business = business,
                    Projects = (await mongo.DbGetCollection<Project>().Find(s => s.BusinessDepartmentId == business.Id).ToListAsync())
                };
                return Result.Success().SetData(busi);
            }
            return Result.SuccessError("无相关信息");
        }
        /// <summary>
        /// 获取业务的信息（不包含project信息）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Result> GetOneBusinessDepartmentInfo(long id)
        {
            var bu = await mongo.DbGetCollection<BusinessDepartment>().Find(s => s.Id == id).FirstAsync();
            return Result.Success().SetData(bu);
        }
        /// <summary>
        /// 获取一个公司的业务列表
        /// </summary>
        /// <param name="companyid"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Result> GetCompanyBusinessDepartmentInfo(long companyid, string keywords)
        {
            if (string.IsNullOrEmpty(keywords))
            {
                return Result.SuccessError("参数解析错误");
            }
            if (keywords == "#")
            {
                var bu = await mongo.DbGetCollection<BusinessDepartment>().Find(s => s.CompanyId == companyid).ToListAsync();
                var data = new List<object>();
                foreach (var item in bu)
                {
                    var busi = new
                    {
                        Business = item,
                        Projects = (await mongo.DbGetCollection<Project>().Find(s => s.BusinessDepartmentId == item.Id).ToListAsync())
                    };
                    data.Add(busi);
                }
                return Result.Success().SetData(data);
            }
            else
            {
                var bu = await mongo.DbGetCollection<BusinessDepartment>().Find(s => s.CompanyId == companyid && s.Name.Contains(keywords)).ToListAsync();
                var data = new List<object>();
                foreach (var item in bu)
                {
                    var busi = new
                    {
                        Business = item,
                        Projects = (await mongo.DbGetCollection<Project>().Find(s => s.BusinessDepartmentId == item.Id).ToListAsync())
                    };
                    data.Add(busi);
                }
                return Result.Success().SetData(data);
            }
        }


        // POST api/<CompanyController>
        [HttpPost]
        public async Task<Result> AddBusinessDepartment([FromBody] BusinessDepartmentAddDto business)
        {
            if (ModelState.IsValid)
            {
                var com = await mongo.DbGetCollection<BusinessDepartment>().Find(s => s.Name == business.Name).AnyAsync();
                if (com)
                {
                    return Result.SuccessError("当前业务部门已注册");
                }
                else
                {
                    var newcom = new BusinessDepartment();
                    newcom.Name = business.Name;
                    newcom.Remark = business.Remark;
                    newcom.ManagerName = business.ManagerName;
                    newcom.Id = Idgen.CreateId();
                    newcom.CompanyId = business.CompanyId;
                    await mongo.DbGetCollection<BusinessDepartment>().InsertOneAsync(newcom);
                    return Result.Success("添加成功");
                }
            }
            else
            {
                return Result.SuccessError("信息填写不全");
            }
        }

        // PUT api/<CompanyController>/5
        [HttpPost]
        //[PermissionFilter(Role.Admin)]
        public async Task<Result> UpdateBusinessDepartment([FromBody] BusinessDepartmentUpdateDto business)
        {
            if (ModelState.IsValid)
            {
                var com = await mongo.DbGetCollection<BusinessDepartment>().Find(s => s.Id == business.Id).AnyAsync();
                if (!com)
                {
                    return Result.SuccessError("参数解析错误");
                }
                else
                {
                    var filter = Builders<BusinessDepartment>.Filter.Eq(s => s.Id, business.Id);
                    var update = Builders<BusinessDepartment>.Update
                        .Set(s => s.Name, business.Name)
                        .Set(s => s.Remark, business.Remark)
                        .Set(s => s.ManagerName, business.ManagerName);
                    var re = await mongo.DbGetCollection<BusinessDepartment>().UpdateOneAsync(filter, update);
                    return re.IsAcknowledged ? Result.Success("修改成功") : Result.SuccessError("修改失败");
                }
            }
            else
            {
                return Result.SuccessError("参数填写不全");
            }
        }

        // DELETE api/<CompanyController>/5
        [HttpDelete]
        //[PermissionFilter(Role.Admin)]
        public async Task<Result> DeleteBusinessDepartment(long id)
        {
            var bu = await mongo.DbGetCollection<BusinessDepartment>().Find(s => s.Id == id).AnyAsync();
            if (bu)
            {
                var filter = Builders<BusinessDepartment>.Filter.Eq(r => r.Id, id);
                var re = await mongo.DbGetCollection<BusinessDepartment>().DeleteOneAsync(filter);
                return re.IsAcknowledged ? Result.Success("操作成功") : Result.SuccessError("处理错误");
            }
            else
            {
                return Result.SuccessError("参数解析错误");
            }
        }
    }
}
