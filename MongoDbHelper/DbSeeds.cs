using MongoDB.Driver;
using System.Xml.Linq;
using TicketAppApi.Extension;
using TicketAppApi.GlobalSettng;
using TicketAppApi.Model;

namespace TicketAppApi.MongoDbHelper
{
    public static class DbSeeds
    {
        public static async void InitDbSeeds(this IServiceCollection  services)
        {
           var len = await DBHelper.Db.DbGetCollection<FlowType>().Find(s => true).ToListAsync();
            if (len.Count == 1 && Appsettings.appBool("UseDbSeed"))
            {
               await DBHelper.Db.DbGetCollection<FlowType>().InsertManyAsync(new List<FlowType>() { 
                    new FlowType(){
                        Id = IdGenFunc.CreateOneId(),
                        Name = "测试流水1",
                        Remark = "测试用"
                    },
                    new FlowType(){
                        Id = IdGenFunc.CreateOneId(),
                        Name = "测试流水2",
                        Remark = "测试用"
                    },
                    new FlowType(){
                        Id = IdGenFunc.CreateOneId(),
                        Name = "测试流水3",
                        Remark = "测试用"
                    },
                    new FlowType(){
                        Id = IdGenFunc.CreateOneId(),
                        Name = "测试流水4",
                        Remark = "测试用"
                    },
                     });
            }
            var admin = await DBHelper.Db.DbGetCollection<FinancialStaff>().Find(s => s.UserName =="13879694217").AnyAsync();
            if (admin is false && Appsettings.appBool("UseDbSeed"))
            {
                var user = new FinancialStaff()
                {
                    Id = IdGenFunc.CreateOneId(),
                    UserName = "13879694217",
                    Password = "123456",
                    CreateTime = DateTime.Now,
                    IsDeleted = false,
                    Name = "管理员",
                    Remark = "",
                    Icon = "",
                    Nick = "",
                    Email = "",
                    Ip = "",
                    Address = "",
                    Role = Role.Admin
                };
                user.BuildPassword();
                await DBHelper.Db.DbGetCollection<FinancialStaff>().InsertManyAsync(new List<FinancialStaff>() { user });
            }
        }
    }
}
