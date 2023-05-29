using Amazon.Auth.AccessControlPolicy;
using IdGen;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MQTTDashWebApi.Extension;
using Ocelot.Infrastructure;
using OfficeOpenXml;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using TicketAppApi.Controllers.ApiFormModel;
using TicketAppApi.Excepsion;
using TicketAppApi.Extension;
using TicketAppApi.JwtExtension;
using TicketAppApi.Model;
using TicketAppApi.MongoDbHelper;
using TicketAppApi.ServiceInject;
using static System.Reflection.Metadata.BlobBuilder;
using LicenseContext = OfficeOpenXml.LicenseContext;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TicketAppApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Policy = JWTService.PolicyName, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FlowController : ControllerBase
    {
        public FlowController(IOptions<JWTTokenOptions> jwtoptions, MemoryCache memoryCache, JwtInvorker jwtInvorker, IMongoDatabase mongo, IdGenerator idgen)
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
        //GET: api/<FlowController>
        [HttpGet]
        public async Task<Result> GetFlowTypes()
        {
            return Result.Success().SetData(await mongo.DbGetCollection<FlowType>().Find(s=>true).ToListAsync());
        }
        [HttpGet]
        public async Task<Result> GetFlowTypesSuggestion(int counts,string keywords)
        {
            if (keywords == "#")
            {
                return Result.Success().SetData(await mongo.DbGetCollection<FlowType>().Find(s => true).Limit(counts).ToListAsync());
            }
            return Result.Success().SetData(await mongo.DbGetCollection<FlowType>().Find(s => s.Name.Contains(keywords)).Limit(counts).ToListAsync());
        }
        //// GET api/<FlowController>/5
        [HttpPost]
        public async Task<Result> AddFlowtype(FlowTypeDto flowType)
        {
            if (ModelState.IsValid)
            {
                var i = new FlowType();
                i.Id = Idgen.CreateId();
                i.Name = flowType.Name;
                i.Remark = flowType.Remark;

                await mongo.DbGetCollection<FlowType>().InsertOneAsync(i);

                LogHelper.LogInsert(HttpContext.GetUserId(),"添加一条流水类型");

                return Result.Success("添加成功");
            }
            return Result.SuccessError("参数填写不正确或不完整");
        }
        //// GET api/<FlowController>/5
        [HttpPost]
        public async Task<Result> UpdateFlowtype(FlowType flowType)
        {
            if (ModelState.IsValid && flowType.Id != 0)
            {
                var filter = Builders<FlowType>.Filter.Eq(s => s.Id, flowType.Id);
                var update = Builders<FlowType>.Update
                    .Set(s => s.Name, flowType.Name)
                    .Set(s => s.Remark, flowType.Remark);
                await mongo.DbGetCollection<FlowType>().UpdateOneAsync(filter,update);

                LogHelper.LogUpdate(HttpContext.GetUserId(), "更新一条流水类型");

                return Result.Success("修改成功");
            }
            return Result.SuccessError("参数填写不正确或不完整");
        }

        // DELETE api/<FlowController>/5
        [HttpDelete]
        public async Task<Result> DeleteFlowType(long id)
        {
            var re = await mongo.DbGetCollection<FlowType>().DeleteOneAsync(s => s.Id == id);
            LogHelper.LogDelete(HttpContext.GetUserId(), "删除一条流水类型");
            return re.IsAcknowledged ? Result.Success("操作成功") : Result.SuccessError("操作失败");
        }

        // POST api/<FlowController>
        [HttpPost]
        public async Task<Result> AddFlow(FlowAddDto flow)
        {
            //var files =  HttpContext.Request.Form.Files;
            if (ModelState.IsValid)
            {
                var fl = new Flow();
                fl.Id = Idgen.CreateId();
                fl.ProjectId = flow.ProjectId;
                fl.SerialNumber = Idgen.CreateId().ToString();
                fl.DocumentNumber = flow.DocumentNumber;
                fl.FlowTypeId = flow.FlowTypeId;
                fl.FinancialStaffId = HttpContext.GetUserId();
                fl.FinanceManager =( await mongo.DbGetCollection<FinancialStaff>().Find(s=>s.Id == HttpContext.GetUserId()).FirstOrDefaultAsync()).Name;
                fl.Remark = flow.Remark;
                fl.InAmount = flow.InAmount;
                fl.OutAmount = flow.OutAmount;
                foreach (var item in flow.Files)
                {
                    var file = new SupportingMaterial();
                    file.FileName = item.FileName;
                    file.FileType = item.FileType;
                    file.FileStream = item.FileStream;
                    file.Id = Idgen.CreateId();
                    file.FlowId = fl.Id;
                    await mongo.DbGetCollection<SupportingMaterial>().InsertOneAsync(file);
                }

                await mongo.DbGetCollection<Flow>().InsertOneAsync(fl);

                var filter = Builders<Project>.Filter.Eq(s => s.Id, flow.ProjectId);
                var update = Builders<Project>.Update
                    .Inc(s => s.ReceivedAmount, flow.OutAmount - flow.InAmount);

               var re = await mongo.DbGetCollection<Project>().UpdateOneAsync(filter, update);
                LogHelper.LogInsert(HttpContext.GetUserId(), "增加一条流水");
                return re.IsAcknowledged ?  Result.Success("添加成功") : Result.SuccessError("添加失败");
            }
            else
            {
                return Result.SuccessError("信息填写不全");
            }
        }

        [HttpGet]
        public async Task<Result> GetAprojectFlow(long projectid,string keywords)
        {
            var proisexit = await mongo.DbGetCollection<Project>().Find(s => s.Id == projectid).AnyAsync();
            if (proisexit)
            {
                var types = await mongo.DbGetCollection<FlowType>().Find(s => true).ToListAsync();
                if (keywords == "#")
                {
                    var flows = await mongo.DbGetCollection<Flow>().Find(s => s.ProjectId == projectid).ToListAsync();
                    return Result.Success().SetData(flows.Select(s=>
                    new { 
                        flow = s,
                        flowType = types.Where(si=>si.Id == s.FlowTypeId ).FirstOrDefault()
                    }
                    ));
                }
                else
                {
                    var flows = await mongo.DbGetCollection<Flow>().Find(s => s.ProjectId == projectid && s.DocumentNumber.Contains(keywords)).ToListAsync();
                    return Result.Success().SetData(flows.Select(s =>
                    new {
                        flow = s,
                        flowType = types.Where(si => si.Id == s.FlowTypeId).FirstOrDefault()
                    }
                    ));
                }
               
            }
            return Result.SuccessError("无相关数据");
        }


        [HttpGet]
        //[PermissionFilter(Role.Admin)]
        public async Task<Result> GetFlowList(string projectids, int pagenumber, int pagesize, string keywords)
        {
            try
            {
                var types = await mongo.DbGetCollection<FlowType>().Find(s => true).ToListAsync();

                var userrole = (HttpContext.User.FindFirstValue(ClaimTypes.Role));

                if (projectids == "[]" || projectids == "[0]" || projectids == "['0']" || projectids == "[\"0\"]")
                {
                    if (userrole == Role.Admin.ToString())
                    {
                        if (keywords == "#")
                        {
                            var data = await mongo.DbGetCollection<Flow>().Find(s => true).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                            var len = await mongo.DbGetCollection<Flow>().Find(s => true).CountDocumentsAsync();
                            return Result.Success().SetData(new
                            {
                                tables = data.Select(s => new {
                                    flow = s,
                                    flowType = types.Where(ss => ss.Id == s.FlowTypeId).FirstOrDefault()
                                }),
                                counts = len
                            });
                        }
                        else if (!string.IsNullOrEmpty(keywords))
                        {
                            var data = await mongo.DbGetCollection<Flow>().Find(s => s.DocumentNumber.Contains(keywords)).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                            var len = await mongo.DbGetCollection<Flow>().Find(s => s.DocumentNumber.Contains(keywords)).CountDocumentsAsync();
                            return Result.Success().SetData(new
                            {
                                tables = data.Select(s => new {
                                    flow = s,
                                    flowType = types.Where(ss => ss.Id == s.FlowTypeId).FirstOrDefault()
                                }),
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
                            var bus =  await mongo.DbGetCollection<BusinessDepartment>().Find(filter).ToListAsync();

                            if (bus.Count > 0)
                            {
                                var filter1 = Builders<Project>.Filter.Or(bus.Select(s => Builders<Project>.Filter.Eq(s1 => s1.BusinessDepartmentId, s.Id)));

                                var pros = await mongo.DbGetCollection<Project>().Find(filter1).ToListAsync();
                                if (pros.Count > 0)
                                {
                                    var filter2 = Builders<Flow>.Filter.Or(pros.Select(s => Builders<Flow>.Filter.Eq(s1 => s1.ProjectId, s.Id)));

                                    if (keywords == "#")
                                    {
                                        var flows = await mongo.DbGetCollection<Flow>().Find(filter2).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                                        var len = await mongo.DbGetCollection<Flow>().Find(filter2).CountDocumentsAsync();
                                        return Result.Success().SetData(new
                                        {
                                            tables = flows.Select(s => new {
                                                flow = s,
                                                flowType = types.Where(ss => ss.Id == s.FlowTypeId).FirstOrDefault()
                                            }),
                                            counts = len
                                        });
                                    }
                                    else
                                    {
                                        var newf = Builders<Flow>.Filter.And(Builders<Flow>.Filter.Or(filter2), Builders<Flow>.Filter.Regex(s => s.DocumentNumber, new BsonRegularExpression($".*{keywords}.*", "i")));
                                        var flows = await mongo.DbGetCollection<Flow>().Find(newf).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();

                                        var len = await mongo.DbGetCollection<Flow>().Find(newf).CountDocumentsAsync();
                                        return Result.Success().SetData(new
                                        {
                                            tables = flows.Select(s => new {
                                                flow = s,
                                                flowType = types.Where(ss => ss.Id == s.FlowTypeId).FirstOrDefault()
                                            }),
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
                        List<FilterDefinition<Flow>> filters = new List<FilterDefinition<Flow>>();
                        //filters.Add(Builders<BusinessDepartment>.Filter.Regex(s => s.Name, new BsonRegularExpression($".*{keywords}.*", "i")));
                        filters.AddRange(projectids.GetModelFromJsonString<long[]>().Select(s => Builders<Flow>.Filter.Eq(s1 => s1.ProjectId, s)));
                        var filter = Builders<Flow>.Filter.Or(filters);
                        var data = await mongo.DbGetCollection<Flow>().Find(filter).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();

                        var len = await mongo.DbGetCollection<Flow>().Find(filter).CountDocumentsAsync();
                        return Result.Success().SetData(new { tables = data.Select(s => new { flow = s, flowType = types.Where(ss => ss.Id == s.FlowTypeId).FirstOrDefault() }), counts = len });
                    }
                    else
                    {
                        List<FilterDefinition<Flow>> filters = new List<FilterDefinition<Flow>>();
                        filters.AddRange(projectids.GetModelFromJsonString<long[]>().Select(s => Builders<Flow>.Filter.Eq(s1 => s1.ProjectId, s)));

                        var filtersABCD = Builders<Flow>.Filter.And(
                            Builders<Flow>.Filter.Or(filters),
                             Builders<Flow>.Filter.Regex(s => s.DocumentNumber, new BsonRegularExpression($".*{keywords}.*", "i"))
                        );
                        var len = await mongo.DbGetCollection<Flow>().Find(filtersABCD).CountDocumentsAsync();
                        var data = await mongo.DbGetCollection<Flow>().Find(filtersABCD).Skip((pagenumber - 1) * pagesize).Limit(pagesize).ToListAsync();
                        return Result.Success().SetData(new { tables = data.Select(s => new { flow = s, flowType = types.Where(ss => ss.Id == s.FlowTypeId).FirstOrDefault() }), counts = len });
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误{ex.Message}+发生位置:{ex.StackTrace}");
                return Result.SuccessError("参数解析错误");
            }

        }

        [HttpDelete]
        public async Task<Result> DeleteFlow(long id)
        {
           

            var flow = await mongo.DbGetCollection<Flow>().Find(s => s.Id == id).FirstOrDefaultAsync();
            var filter = Builders<Project>.Filter.Eq(s => s.Id, flow.ProjectId);
            var update = Builders<Project>.Update.Inc(s => s.ReceivedAmount,( flow.OutAmount - flow.InAmount)*-1);

            var re = await mongo.DbGetCollection<Flow>().DeleteManyAsync(s => s.Id == id);
            var re2 = await mongo.DbGetCollection<SupportingMaterial>().DeleteManyAsync(s => s.FlowId == id);


            var re1 = await mongo.DbGetCollection<Project>().UpdateOneAsync(filter, update);
            LogHelper.LogDelete(HttpContext.GetUserId(), "删除一条流水");
            return re.IsAcknowledged && re1.IsAcknowledged && re2 .IsAcknowledged? Result.Success("操作成功") : Result.SuccessError("操作失败");
        }

        [HttpPost]
        public async Task<Result> UpdateFlow(FlowUpdateDto flow)
        {
            if (flow.Id == 0)
            {
                return Result.SuccessError("信息填写不全");
            }
        

            //var files = HttpContext.Request.Form.Files;
            if (ModelState.IsValid)
            {
                var fl = new Flow();
                fl.Id = Idgen.CreateId();
                fl.ProjectId = flow.ProjectId;
                fl.SerialNumber = Idgen.CreateId().ToString();
                fl.DocumentNumber = flow.DocumentNumber;
                fl.FlowTypeId = flow.FlowTypeId;
                fl.FinancialStaffId = HttpContext.GetUserId();
                fl.FinanceManager = (await mongo.DbGetCollection<FinancialStaff>().Find(s => s.Id == HttpContext.GetUserId()).FirstOrDefaultAsync()).Name;
                fl.Remark = flow.Remark;
                fl.InAmount = flow.InAmount;
                fl.OutAmount = flow.OutAmount;
                foreach (var item in flow.Files)
                {
                    var file = new SupportingMaterial();
                    file.FileName = item.FileName;
                    file.FileType = item.FileType;
                    file.FileStream = item.FileStream;
                    file.Id = Idgen.CreateId();
                    file.FlowId = fl.Id;
                    await mongo.DbGetCollection<SupportingMaterial>().InsertOneAsync(file);
                }
                await mongo.DbGetCollection<Flow>().InsertOneAsync(fl);


                var old_flow = await mongo.DbGetCollection<Flow>().Find(s => s.Id == flow.Id).FirstOrDefaultAsync();
                var filter = Builders<Project>.Filter.Eq(s => s.Id, old_flow.ProjectId);
                var update = Builders<Project>.Update
                    .Inc(s => s.ReceivedAmount, (old_flow.OutAmount - old_flow.InAmount) * -1 + (flow.OutAmount - flow.InAmount));

                var re1 = await mongo.DbGetCollection<Project>().UpdateOneAsync(filter, update);
                var re = await mongo.DbGetCollection<Flow>().DeleteManyAsync(s => s.Id == flow.Id);
                var re2 = await mongo.DbGetCollection<SupportingMaterial>().DeleteManyAsync(s => s.FlowId == flow.Id);
                LogHelper.LogUpdate(HttpContext.GetUserId(), "更新一条流水");
                return re.IsAcknowledged && re1.IsAcknowledged && re2.IsAcknowledged ? Result.Success("修改成功") : Result.SuccessError("修改失败");
            }
            else
            {
                return Result.SuccessError("信息填写不全");
            }
        }

        [HttpDelete]
        public async Task<Result> DeleteSupportMaterial(string ids)
        {
            try
            {
                List<FilterDefinition<SupportingMaterial>> filters = new List<FilterDefinition<SupportingMaterial>>();
                filters.AddRange(ids.GetModelFromJsonString<long[]>().Select(s => Builders<SupportingMaterial>.Filter.Eq(s1 => s1.Id, s)));
                var filter = Builders<SupportingMaterial>.Filter.Or(filters);

                var re2 = await mongo.DbGetCollection<SupportingMaterial>().DeleteOneAsync(filter);
                return  re2.IsAcknowledged ? Result.Success("操作成功") : Result.SuccessError("操作失败");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误：{ex.Message}，发生位置:{ex.StackTrace}");
                return Result.SuccessError("操作失败");
            }
                       


           
           
        }
        [HttpGet]
        public async Task<Result> GetAFlowAllInfo(long id)
        {
             var types = await mongo.DbGetCollection<FlowType>().Find(s => true).ToListAsync();
            if (await mongo.DbGetCollection<Flow>().Find(s=>s.Id == id).AnyAsync())
            {
                var flow = await mongo.DbGetCollection<Flow>().Find(s => s.Id == id).FirstAsync();
                var proexist = await mongo.DbGetCollection<Project>().Find(s => s.Id == flow.ProjectId).AnyAsync();
                if (proexist)
                {
                    var pro = await mongo.DbGetCollection<Project>().Find(s => s.Id == flow.ProjectId).FirstAsync();
                    var busiexist = await mongo.DbGetCollection<BusinessDepartment>().Find(s => s.Id == pro.BusinessDepartmentId).AnyAsync();
                    if (busiexist)
                    {
                        var bus = await mongo.DbGetCollection<BusinessDepartment>().Find(s => s.Id == pro.BusinessDepartmentId).FirstAsync();
                        var comexist = await mongo.DbGetCollection<Company>().Find(s => s.Id == bus.CompanyId).AnyAsync();
                        if (comexist)
                        {
                            var come = await mongo.DbGetCollection<Company>().Find(s => s.Id == bus.CompanyId).FirstAsync();
                            return Result.Success().SetData(new { 
                                Company = come,
                                BusinessDepartment = bus,
                                Project = pro,
                                Flow = new {
                                    Flow = flow,
                                    User = await mongo.DbGetCollection<FinancialStaff>().Find(s => s.Id == flow.FinancialStaffId).FirstOrDefaultAsync(),
                                    FlowType = types.Where(s => s.Id == flow.FlowTypeId).FirstOrDefault(),
                                    FlowFiles = (await mongo.DbGetCollection<SupportingMaterial>().Find(s => s.FlowId == flow.Id).ToListAsync()).Select(s => new {
                                        base64 =(s.FileStream),
                                        fileName = s.FileName,
                                        fileType = s.FileType
                                    })
                                    }
                                
                            });
                        }
                        else
                        {
                            return Result.Success().SetData(new
                            {
                                Company = new { },
                                BusinessDepartment = bus,
                                Project = pro,
                                User = await mongo.DbGetCollection<FinancialStaff>().Find(s => s.Id == flow.FinancialStaffId).FirstOrDefaultAsync(),
                                Flow = new
                                {
                                    Flow = flow,
                                    FlowType = types.Where(s => s.Id == flow.FlowTypeId).FirstOrDefault(),
                                    FlowFiles = (await mongo.DbGetCollection<SupportingMaterial>().Find(s => s.FlowId == flow.Id).ToListAsync()).Select(s => new {
                                        base64 = (s.FileStream),
                                        fileName = s.FileName,
                                        fileType = s.FileType
                                    })
                                },
                            });
                        }
                    }
                    else
                    {
                        return Result.Success().SetData(new
                        {
                            Company = new { },
                            BusinessDepartment = new { },
                            Project = pro,
                            User = await mongo.DbGetCollection<FinancialStaff>().Find(s => s.Id == flow.FinancialStaffId).FirstOrDefaultAsync(),
                            Flow = new
                            {
                                Flow = flow,
                                FlowType = types.Where(s => s.Id == flow.FlowTypeId).FirstOrDefault(),
                                FlowFiles = (await mongo.DbGetCollection<SupportingMaterial>().Find(s => s.FlowId == flow.Id).ToListAsync()).Select(s => new {
                                    base64 = (s.FileStream),
                                    fileName = s.FileName,
                                    fileType = s.FileType
                                })
                            },
                        });
                    }

                }
                else
                {
                    return Result.Success().SetData(new
                    {
                        Company = new { },
                        BusinessDepartment = new {},
                        Project = new { },
                        User = await mongo.DbGetCollection<FinancialStaff>().Find(s => s.Id == flow.FinancialStaffId).FirstOrDefaultAsync(),
                        Flow = new
                        {
                            Flow = flow,
                            FlowType = types.Where(s => s.Id == flow.FlowTypeId).FirstOrDefault(),
                            FlowFiles = (await mongo.DbGetCollection<SupportingMaterial>().Find(s => s.FlowId == flow.Id).ToListAsync()).Select(s => new {
                                base64 = (s.FileStream),
                                fileName = s.FileName,
                                fileType = s.FileType
                            })
                        },
                    });
                }
            }
            else
            {
                return Result.SuccessError("参数解析错误");
            }
           
        }

        [HttpPost]
        public  FileStreamResult OutputFlow(List<OutFlow> data)
        {

           
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            Stream stream = new MemoryStream();
            ExcelPackage package = new(stream);
            // 添加worksheet
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("流水");
            //单元格自动适应大小
            worksheet.Cells.Style.ShrinkToFit = true;
            worksheet.Cells.Style.Numberformat.Format = "yyyy/MM/dd HH:mm:ss";
            worksheet.Cells["A1"].LoadFromCollection(new List<deviceexcelnames> { new() }, false, OfficeOpenXml.Table.TableStyles.Light13);
            worksheet.Cells.Style.Numberformat.Format = "yyyy/MM/dd HH:mm:ss";
            //全部字段导出
            worksheet.Cells["A2"].LoadFromCollection(data.Select(s => new
            {
                s.SerialNumber,
                s.DocumentNumber,
                s.FlowType,
                s.InAmount,
                s.OutAmount,
                s.FinanceManager,
                time = s.CreateTime.ToString(),
                s.Remark
            }), false, OfficeOpenXml.Table.TableStyles.Light13);
            package.Save();
            package.Dispose();

            string fileExt = Path.GetExtension("thumb.xlsx");
            var provider = new FileExtensionContentTypeProvider();
            var memi = provider.Mappings[fileExt];
            stream.Position = 0;
            LogHelper.LogInfo(HttpContext.GetUserId(), "导出流水表");
            return new FileStreamResult(stream, memi);
        }
        
    }
}
