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
using System;
using System.Drawing;
using System.Security.Claims;
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
    public class ProjectController : ControllerBase
    {
        public ProjectController(IOptions<JWTTokenOptions> jwtoptions, MemoryCache memoryCache, JwtInvorker jwtInvorker, IMongoDatabase mongo, IdGenerator idgen)
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
        public async Task<Result> GetProjectList(int pagenumber, int pagesize, string keywords, string businessids)
        {
            try
            {
                var userrole = (HttpContext.User.FindFirstValue(ClaimTypes.Role));
                if (businessids == "[]" || businessids == "[0]" || businessids == "['0']" || businessids == "[\"0\"]")
                {
                    if (userrole == Role.Admin.ToString())
                    {
                        if (keywords == "#")
                        {
                            var data = await mongo.DbGetCollection<Project>().Find(s => true).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                            var len = await mongo.DbGetCollection<Project>().Find(s => true).CountDocumentsAsync();
                            return Result.Success().SetData(new
                            {
                                tables = data,
                                counts = len
                            });
                        }
                        else if (!string.IsNullOrEmpty(keywords))
                        {
                            var data = await mongo.DbGetCollection<Project>().Find(s => s.Name.Contains(keywords)).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                            var len = await mongo.DbGetCollection<Project>().Find(s => s.Name.Contains(keywords)).CountDocumentsAsync();

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
                        var res = await mongo.DbGetCollection<Relation>().Find(s => s.FinancialStaffId == HttpContext.GetUserId()).ToListAsync();
                        if (res.Count > 0)
                        {
                            var filter = Builders<BusinessDepartment>.Filter.Or(res.Select(s => Builders<BusinessDepartment>.Filter.Eq(s1 => s1.Id, s.BusinessDepartmentId)));
                            var bus = await mongo.DbGetCollection<BusinessDepartment>().Find(filter).ToListAsync();

                            if (bus.Count > 0)
                            {
                                var filter1 = Builders<Project>.Filter.Or(bus.Select(s => Builders<Project>.Filter.Eq(s1 => s1.BusinessDepartmentId, s.Id)));
                                if (keywords == "#")
                                {
                                    var pros = await mongo.DbGetCollection<Project>().Find(filter1).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();

                                    var len = await mongo.DbGetCollection<Project>().Find(filter1).CountDocumentsAsync();

                                    return Result.Success().SetData(new
                                    {
                                        tables = pros,
                                        counts = len
                                    });
                                }
                                else
                                {
                                   

                                 var newfilter =   Builders<Project>.Filter.And(Builders<Project>.Filter.Or(filter1),Builders<Project>.Filter.Regex(s => s.Name, new BsonRegularExpression($".*{keywords}.*", "i"))
                        );
                                    var len = await mongo.DbGetCollection<Project>().Find(newfilter).CountDocumentsAsync();
                                    var pros = await mongo.DbGetCollection<Project>().Find(newfilter).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                                    return Result.Success().SetData(new
                                    {
                                        tables = pros,
                                        counts = len
                                    });
                                }
                            }
                            else
                            {
                                return Result.Success("无数据");
                            }
                        }
                        else
                        {
                            return Result.Success("无数据");
                        }
                    }
                }
                else
                {
                    if (keywords == "#")
                    {
                        List<FilterDefinition<Project>> filters = new List<FilterDefinition<Project>>();
                        //filters.Add(Builders<BusinessDepartment>.Filter.Regex(s => s.Name, new BsonRegularExpression($".*{keywords}.*", "i")));
                        filters.AddRange(businessids.GetModelFromJsonString<long[]>().Select(s => Builders<Project>.Filter.Eq(s1 => s1.BusinessDepartmentId, s)));
                        var filter = Builders<Project>.Filter.Or(filters);

                        var len = await mongo.DbGetCollection<Project>().Find(filter).CountDocumentsAsync();
                        var data = await mongo.DbGetCollection<Project>().Find(filter).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();


                        return Result.Success().SetData(new { tables = data, counts = len });
                    }
                    else
                    {
                        //List<FilterDefinition<Project>> filters = new List<FilterDefinition<Project>>();
                        //filters.Add(Builders<Project>.Filter.Regex(s => s.Name, new BsonRegularExpression($".*{keywords}.*", "i")));
                        //filters.AddRange(businessids.GetModelFromJsonString<long[]>().Select(s => Builders<Project>.Filter.Eq(s1 => s1.BusinessDepartmentId, s)));
                        //var filter = Builders<Project>.Filter.Or(filters);

                        List<FilterDefinition<Project>> filters = new List<FilterDefinition<Project>>();
                        filters.AddRange(businessids.GetModelFromJsonString<long[]>().Select(s => Builders<Project>.Filter.Eq(s1 => s1.BusinessDepartmentId, s)));
                        var filtersABCD = Builders<Project>.Filter.And(
                            Builders<Project>.Filter.Or(filters),
                             Builders<Project>.Filter.Regex(s => s.Name, new BsonRegularExpression($".*{keywords}.*", "i"))
                        );
                        var len = await mongo.DbGetCollection<Project>().Find(filtersABCD).CountDocumentsAsync();


                        var data = await mongo.DbGetCollection<Project>().Find(filtersABCD).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
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
        /// <param name="id">业务id</param>
        /// <returns></returns>
        [HttpGet]
        //[PermissionFilter(Role.Admin)]
        public async Task<Result> GetOneyewuAllInfo(long id, string keywords)
        {
            if (string.IsNullOrEmpty(keywords))
            {
                return Result.SuccessError("参数解析错误");
            }
            else
            {
                if (keywords == "#")
                {
                    var buisexit = await mongo.DbGetCollection<BusinessDepartment>().Find(s => s.Id == id).AnyAsync();

                    if (buisexit == false)
                    {
                        return Result.SuccessError("无相关信息");
                    }
                    var business = await mongo.DbGetCollection<BusinessDepartment>().Find(s => s.Id == id).FirstAsync();


                    var projects = await mongo.DbGetCollection<Project>().Find(s => s.BusinessDepartmentId == id).ToListAsync();

                    var busi = new
                    {
                        Business = business,
                        Projects = projects
                    };
                    return Result.Success().SetData(busi);
                }
                else
                {
                    var buisexit = await mongo.DbGetCollection<BusinessDepartment>().Find(s => s.Id == id).AnyAsync();

                    if (buisexit == false)
                    {
                        return Result.SuccessError("无相关信息");
                    }
                    var business = await mongo.DbGetCollection<BusinessDepartment>().Find(s => s.Id == id).FirstAsync();


                    var projects = await mongo.DbGetCollection<Project>().Find(s => s.BusinessDepartmentId == id && s.Name.Contains(keywords)).ToListAsync();

                    var busi = new
                    {
                        Business = business,
                        Projects = projects
                    };
                    return Result.Success().SetData(busi);
                }
            }

        }

        [HttpGet]
        public async Task<Result> GetOneProjectAllInfo(long id, string keywords)
        {
            var types = await mongo.DbGetCollection<FlowType>().Find(s => true).ToListAsync();
            if (keywords == "#")
            {
                var proexit = await mongo.DbGetCollection<Project>().Find(s => s.Id == id).AnyAsync();

                if (proexit == false)
                {
                    return Result.SuccessError("无相关信息");
                }
                var pro = await mongo.DbGetCollection<Project>().Find(s => s.Id == id).FirstAsync();
                var liushuis = await mongo.DbGetCollection<Flow>().Find(s => s.ProjectId == pro.Id).ToListAsync();
                var businessexist = await mongo.DbGetCollection<BusinessDepartment>().Find(s => s.Id == pro.BusinessDepartmentId).AnyAsync();
                if (!businessexist)
                {
                    var busi = new
                    {
                        Business = new { },
                        Projects = pro,
                        Company = new { },
                        Flows = liushuis.Select(s =>
                    new
                    {
                        flow = s,
                        flowType = types.Where(si => si.Id == s.FlowTypeId).FirstOrDefault()
                    }
                    )

                    };
                    return Result.Success().SetData(busi);
                }
                else
                {
                    var business = await mongo.DbGetCollection<BusinessDepartment>().Find(s => s.Id == pro.BusinessDepartmentId).FirstAsync();
                    var comexist = await mongo.DbGetCollection<Company>().Find(s => s.Id == business.CompanyId).AnyAsync();
                    if (comexist)
                    {

                        var computy = await mongo.DbGetCollection<Company>().Find(s => s.Id == business.CompanyId).FirstAsync();


                        var busi = new
                        {
                            Business = business,
                            Projects = pro,
                            Company = computy,
                            Flows = liushuis.Select(s =>
                    new
                    {
                        flow = s,
                        flowType = types.Where(si => si.Id == s.FlowTypeId).FirstOrDefault()
                    }
                    )
                        };
                        return Result.Success().SetData(busi);
                    }
                    else
                    {
                        var busi = new
                        {
                            Business = business,
                            Projects = pro,
                            Company = new { },
                            Flows = liushuis.Select(s =>
                    new
                    {
                        flow = s,
                        flowType = types.Where(si => si.Id == s.FlowTypeId).FirstOrDefault()
                    }
                    )
                        };
                        return Result.Success().SetData(busi);
                    }


                }
            }
            else
            {
                var proexit = await mongo.DbGetCollection<Project>().Find(s => s.Id == id).AnyAsync();

                if (proexit == false)
                {
                    return Result.SuccessError("无相关信息");
                }
                var pro = await mongo.DbGetCollection<Project>().Find(s => s.Id == id).FirstAsync();
                var liushuis = await mongo.DbGetCollection<Flow>().Find(s => s.ProjectId == pro.Id && s.SerialNumber.Contains(keywords)).ToListAsync();
                var businessexist = await mongo.DbGetCollection<BusinessDepartment>().Find(s => s.Id == pro.BusinessDepartmentId).AnyAsync();
                if (!businessexist)
                {
                    var busi = new
                    {
                        Business = new { },
                        Projects = pro,
                        Company = new { },
                        Flows = liushuis.Select(s =>
                    new
                    {
                        flow = s,
                        flowType = types.Where(si => si.Id == s.FlowTypeId).FirstOrDefault()
                    }
                    )

                    };
                    return Result.Success().SetData(busi);
                }
                else
                {
                    var business = await mongo.DbGetCollection<BusinessDepartment>().Find(s => s.Id == pro.BusinessDepartmentId).FirstAsync();
                    var comexist = await mongo.DbGetCollection<Company>().Find(s => s.Id == business.CompanyId).AnyAsync();
                    if (comexist)
                    {

                        var computy = await mongo.DbGetCollection<Company>().Find(s => s.Id == business.CompanyId).FirstAsync();


                        var busi = new
                        {
                            Business = business,
                            Projects = pro,
                            Company = computy,
                            Flows = liushuis.Select(s =>
                    new
                    {
                        flow = s,
                        flowType = types.Where(si => si.Id == s.FlowTypeId).FirstOrDefault()
                    }
                    )
                        };
                        return Result.Success().SetData(busi);
                    }
                    else
                    {
                        var busi = new
                        {
                            Business = business,
                            Projects = pro,
                            Company = new { },
                            Flows = liushuis.Select(s =>
                    new
                    {
                        flow = s,
                        flowType = types.Where(si => si.Id == s.FlowTypeId).FirstOrDefault()
                    }
                    )
                        };
                        return Result.Success().SetData(busi);
                    }


                }
            }





        }





        [HttpGet]
        public async Task<Result> getprojectsuggestionlist(string buids, int pagenumber, int pagesize, string keywords)
        {
            try
            {
                if (buids == "[]" || buids == "[0]" || buids == "['0']" || buids == "[\"0\"]")
                {
                    if (keywords == "#")
                    {
                        var data = await mongo.DbGetCollection<Project>().Find(s => true).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                        return Result.Success().SetData(data);
                    }
                    else if (!string.IsNullOrEmpty(keywords))
                    {
                        var data = await mongo.DbGetCollection<Project>().Find(s => s.Name.Contains(keywords)).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                        return Result.Success().SetData(data);
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
                        List<FilterDefinition<Project>> filters = new List<FilterDefinition<Project>>();
                        //filters.Add(Builders<BusinessDepartment>.Filter.Regex(s => s.Name, new BsonRegularExpression($".*{keywords}.*", "i")));
                        filters.AddRange(buids.GetModelFromJsonString<long[]>().Select(s => Builders<Project>.Filter.Eq(s1 => s1.BusinessDepartmentId, s)));
                        var filter = Builders<Project>.Filter.Or(filters);
                        var data = await mongo.DbGetCollection<Project>().Find(filter).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                        return Result.Success().SetData(data);
                    }
                    else
                    {
                        List<FilterDefinition<Project>> filters = new List<FilterDefinition<Project>>();
                        filters.AddRange(buids.GetModelFromJsonString<long[]>().Select(s => Builders<Project>.Filter.Eq(s1 => s1.BusinessDepartmentId, s)));
                        var filtersABCD = Builders<Project>.Filter.And(
                            Builders<Project>.Filter.Or(filters),
                             Builders<Project>.Filter.Regex(s => s.Name, new BsonRegularExpression($".*{keywords}.*", "i"))
                        );
                        var data = await mongo.DbGetCollection<Project>().Find(filtersABCD).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                        return Result.Success().SetData(data);
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误{ex.Message}+发生位置:{ex.StackTrace}");
                return Result.SuccessError("参数解析错误");
            }

        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="id">业务id  </param>
        ///// <param name="keywords">关键字</param>
        ///// <returns></returns>
        //[HttpGet]
        ////[PermissionFilter(Role.Admin)]
        //public async Task<Result> GetOneBusinessProject(long id,string keywords)
        //{
        //    var project = await mongo.DbGetCollection<Project>().Find(s => s.BusinessDepartmentId == id).AnyAsync();
        //    if (project == false)
        //    {
        //        return Result.SuccessError("无相关信息");
        //    }
        //    var business = await mongo.DbGetCollection<BusinessDepartment>().Find(s => s.Id == project.BusinessDepartmentId).FirstAsync();

        //    Company company = new Company();

        //    if (business is not null)
        //    {
        //        company = await mongo.DbGetCollection<Company>().Find(s => s.Id == business.CompanyId).FirstAsync();
        //    }
        //    var busi = new
        //    {
        //        Company = company,
        //        Business = business,
        //        Projects = project
        //    };
        //    return Result.Success().SetData(busi);
        //}

        /// <summary>
        /// 获取业务的信息（不包含project信息）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<Result> GetOneProjectInfo(long id)
        {
            var bu = await mongo.DbGetCollection<Project>().Find(s => s.Id == id).FirstAsync();
            return Result.Success().SetData(bu);
        }
        // POST api/<CompanyController>
        [HttpPost]
        public async Task<Result> AddProject([FromBody] ProjectAddDto project)
        {
            if (ModelState.IsValid)
            {
                var com = await mongo.DbGetCollection<Project>().Find(s => s.Name == project.Name).AnyAsync();
                if (com)
                {
                    return Result.SuccessError("当前项目已注册");
                }
                else
                {
                    var newcom = new Project();
                    newcom.Name = project.Name;
                    newcom.Remark = project.Remark;
                    newcom.ProjectNumber = project.ProjectNumber;
                    newcom.ManagerName = project.ManagerName;
                    newcom.ContractAmount = project.ContractAmount;
                    newcom.ReceivedAmount = 0;
                    newcom.Id = Idgen.CreateId();
                    newcom.BusinessDepartmentId = project.BusinessDepartmentId;
                    await mongo.DbGetCollection<Project>().InsertOneAsync(newcom);
                    LogHelper.LogInsert(HttpContext.GetUserId(), "增加一个项目");
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
        public async Task<Result> UpdateProject([FromBody] ProjectUpdateDto project)
        {
            if (ModelState.IsValid)
            {
                var com = await mongo.DbGetCollection<Project>().Find(s => s.Id == project.Id).FirstAsync();
                if (com is null)
                {
                    return Result.SuccessError("参数解析错误");
                }
                else
                {
                    var filter = Builders<Project>.Filter.Eq(s => s.Id, project.Id);
                    var update = Builders<Project>.Update
                        .Set(s => s.Name, project.Name)
                        .Set(s => s.ManagerName, project.ManagerName)
                        //.Set(s => s.ReceivedAmount, project.ReceivedAmount)
                        .Set(s => s.Remark, project.Remark)
                        .Set(s => s.ContractAmount, project.ContractAmount)
                        .Set(s => s.ProjectNumber, project.ProjectNumber);
                    var re = await mongo.DbGetCollection<Project>().UpdateOneAsync(filter, update);
                    LogHelper.LogUpdate(HttpContext.GetUserId(), "修改一个项目");
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
        public async Task<Result> DeleteProject(long id)
        {
            var bu = await mongo.DbGetCollection<Project>().Find(s => s.Id == id).AnyAsync();
            if (bu)
            {
                var filter = Builders<Project>.Filter.Eq(r => r.Id, id);
                var re = await mongo.DbGetCollection<Project>().DeleteOneAsync(filter);
                LogHelper.LogDelete(HttpContext.GetUserId(), "删除一个项目");
                return re.IsAcknowledged ? Result.Success("操作成功") : Result.SuccessError("处理错误");
            }
            else
            {
                return Result.SuccessError("参数解析错误");
            }
        }
    }
}
