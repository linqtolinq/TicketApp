using Amazon.Auth.AccessControlPolicy;
using IdGen;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MQTTDashWebApi.Extension;
using System.ComponentModel.Design;
using System.Diagnostics.Metrics;
using TicketAppApi.Controllers.ApiFormModel;
using TicketAppApi.Excepsion;
using TicketAppApi.Extension;
using TicketAppApi.JwtExtension;
using TicketAppApi.Model;
using TicketAppApi.MongoDbHelper;
using TicketAppApi.ServiceInject;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static MongoDB.Driver.WriteConcern;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TicketAppApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Policy = JWTService.PolicyName, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CompanyController : ControllerBase
    {
        public CompanyController(IOptions<JWTTokenOptions> jwtoptions, MemoryCache memoryCache, JwtInvorker jwtInvorker, IMongoDatabase mongo, IdGenerator idgen)
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
        /// 获取公司列表集合
        /// </summary>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        [HttpGet]
        [PermissionFilter(Role.Admin)]
        public async Task<Result> GetCompanyListByName(int pagenumber, int pagesize, string keywords)
        {
            if (keywords == "#")
            {
                long len = await mongo.DbGetCollection<Company>().Find(s => true).CountDocumentsAsync();
                var data =await mongo.DbGetCollection<Company>().Find(s => true).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                return Result.Success().SetData(new { 
                    tables= data,
                    counts = len
                });
            }
            else if (!string.IsNullOrEmpty(keywords))
            {
                long len = await mongo.DbGetCollection<Company>().Find(s => true).CountDocumentsAsync();
                var data =await mongo.DbGetCollection<Company>().Find(s => s.Name.Contains(keywords)).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
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
        [HttpGet]
        [PermissionFilter(Role.Admin)]
        public async Task<Result> GetCompanyListByManagerName(int pagenumber, int pagesize, string keywords)
        {
            if (keywords == "#")
            {
                long len = await mongo.DbGetCollection<Company>().Find(s => true).CountDocumentsAsync();
                var data = await mongo.DbGetCollection<Company>().Find(s => true).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();

                return Result.Success().SetData(new
                {
                    tables = data,
                    counts = len
                });
            }
            else if (!string.IsNullOrEmpty(keywords))
            {
                long len = await mongo.DbGetCollection<Company>().Find(s => true).CountDocumentsAsync();
                var data = await mongo.DbGetCollection<Company>().Find(s => s.ManagerName.Contains(keywords)).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
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
        // GET api/<CompanyController>/5
        /// <summary>
        /// 获取公司详细信息（包含业务信息和project信息）
        /// </summary>
        /// <param name="companyid"></param>
        /// <returns></returns>
        [HttpGet]
        [PermissionFilter(Role.Admin)]
        public async Task<Result> GetOneCompanyAllInfo(long companyid)
        {
            var company =await mongo.DbGetCollection<Company>().Find(s => s.Id == companyid).FirstAsync();
            var business = await mongo.DbGetCollection<BusinessDepartment>().Find(s => s.CompanyId == companyid).ToListAsync();
            var data = new { 
                Company=company,
                Business =new List<object>(),
            };
            if (business.Count > 0)
            {
                foreach (var item in business)
                {
                    var busi = new
                    {
                        Business = item,
                        Projects = (await mongo.DbGetCollection<Project>().Find(s => s.BusinessDepartmentId == item.Id).ToListAsync())
                    };
                    data.Business.Add(busi);
                }
            }
            return Result.Success().SetData(data);
        }


        //[HttpGet]
        //[PermissionFilter(Role.Admin)]
        //public async Task<Result> GetOneCompanyAllInfo(string comids, string keywords,int counts)
        //{
        //    if (string.IsNullOrEmpty(comids))
        //    {
        //        return Result.SuccessError("参数解析错误");
        //    }
        //    if (comids == "[]" || comids == "[0]" || comids == "['0']" || comids == "[\"0\"]")
        //    {
        //        var business = await mongo.DbGetCollection<BusinessDepartment>().Find(s => s.Name.Contains(keywords)).Limit(counts).ToListAsync();
        //        return Result.Success().SetData(business);
        //    }
        //    else
        //    {
        //        try
        //        {
        //            List<FilterDefinition<BusinessDepartment>> filters = new List<FilterDefinition<BusinessDepartment>>();
        //            //filters.Add(Builders<BusinessDepartment>.Filter.Regex(s => s.Name, new BsonRegularExpression($".*{keywords}.*", "i")));
        //            filters.AddRange(comids.GetModelFromJsonString<long[]>().Select(s => Builders<BusinessDepartment>.Filter.Eq(s1 => s1.CompanyId, s)));
        //            var filter = Builders<BusinessDepartment>.Filter.Or(filters);
        //            var data = await mongo.DbGetCollection<BusinessDepartment>().Find(filter).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
        //            return Result.Success().SetData(data);
        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //        }
        //    }
           
        //}

        /// <summary>
        /// 获取公司的信息（不包含其业务信息和project信息）
        /// </summary>
        /// <param name="companyid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Result> GetOneCompanyInfo(long companyid)
        {
            var company = await mongo.DbGetCollection<Company>().Find(s => s.Id == companyid).FirstAsync();
            return Result.Success().SetData(company);
        }
        // POST api/<CompanyController>
        [HttpPost]
        public async Task<Result> AddCompany([FromBody] CompanyAddDto company)
        {
            if (ModelState.IsValid)
            {
                var com = await mongo.DbGetCollection<Company>().Find(s => s.Name == company.Name).AnyAsync();
                if (com)
                {
                    return Result.SuccessError("当前公司已注册");
                }
                else
                {
                    var newcom = new Company();
                    newcom.Name = company.Name;
                    newcom.Remark = company.Remark;
                    newcom.RegisteredLocation = company.RegisteredLocation;
                    newcom.ManagerName = company.ManagerName;
                    newcom.PhoneNumber = company.PhoneNumber;
                    newcom.Id = Idgen.CreateId();
                    await mongo.DbGetCollection<Company>().InsertOneAsync(newcom);

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
        public async Task<Result> UpdateCompany([FromBody] CompanyUpdateDto company)
        {
            if (ModelState.IsValid)
            {
               var com =  await mongo.DbGetCollection<Company>().AsQueryable().Where(s => s.Id == company.Id).AnyAsync();
                var coms = await mongo.DbGetCollection<Company>().AsQueryable().Where(s => true).ToListAsync();
                if (!com)
                {
                    return Result.SuccessError("参数解析错误");
                }
                else
                {
                    var filter = Builders<Company>.Filter.Eq(s => s.Id, company.Id);
                    var update = Builders<Company>.Update
                        .Set(s => s.Name, company.Name)
                        .Set(s => s.Remark, company.Remark)
                        .Set(s => s.RegisteredLocation, company.RegisteredLocation)
                        .Set(s => s.PhoneNumber, company.PhoneNumber)
                        .Set(s => s.ManagerName, company.ManagerName);
                    var re = await mongo.DbGetCollection<Company>().UpdateOneAsync(filter,update);
                    if (re.IsAcknowledged)
                    {
                        return Result.Success("修改成功");
                    }
                    else
                    {
                        return Result.SuccessError("修改失败");
                    }
                }
            }
            else
            {
                return Result.SuccessError("参数填写不全");
            }
        }

        // DELETE api/<CompanyController>/5
        [HttpDelete]
        [PermissionFilter(Role.Admin)]
        public async Task<Result> DeleteCompany(long companyid)
        {
            var company = await mongo.DbGetCollection<Company>().Find(s => s.Id == companyid).FirstAsync();
            if (company != null)
            {
                var filter = Builders<Company>.Filter.Eq(r => r.Id, companyid);
                var re =  await mongo.DbGetCollection<Company>().DeleteOneAsync(filter);
                return re.IsAcknowledged ? Result.Success("操作成功") : Result.SuccessError("处理错误");
            }
            else
            {
                return Result.SuccessError("参数解析错误");
            }
        }
    }
}
