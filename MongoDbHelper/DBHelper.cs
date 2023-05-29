using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using TicketAppApi.Extension;
using TicketAppApi.GlobalSettng;
using TicketAppApi.Model;

namespace TicketAppApi.MongoDbHelper
{
    public static class DBHelper
    {
        public static IMongoDatabase? Db { get; set; }
        public static IServiceCollection DBInit(this IServiceCollection services)
        {
            //var setting = new MongoClientSettings()
            //{
            //    Server = new MongoServerAddress(Appsettings.app("MongoDb:Server"), int.Parse(Appsettings.app("MongoDb:Port") ?? "27017")),
            //    Credential = MongoCredential.CreateCredential(Appsettings.app("MongoDb:Database"), Appsettings.app("MongoDb:User"), Appsettings.app("MongoDb:Pwd")),
            //    ConnectTimeout = TimeSpan.FromSeconds(int.Parse(Appsettings.app("MongoDb:ConnectTimeout") ?? "5")),
            //    MaxConnectionPoolSize = int.Parse(Appsettings.app("MongoDb:MaxConnectionPoolSize") ?? "2000"),
            //};

            //var client = new MongoClient(setting);
            var client = new MongoClient(Appsettings.app("MongoDb:Url"));
            Db = client.GetDatabase(Appsettings.app("MongoDb:Database"));
            Db.InitTables(new List<Type>() { 
                typeof(Flow),
                typeof(FlowType),
                typeof(BusinessDepartment),
                typeof(Company),
                typeof(FinancialStaff),
                typeof(IncomeExpenseStatement),
                typeof(Project),
                typeof(SupportingMaterial),
                typeof(Relation),
                typeof(LogSystem)
            });
            services.AddSingleton(Db);
            services.InitDbSeeds();
            return services;
        }
        public static IMongoCollection<T> DbGetCollection<T>(this IMongoDatabase mongoDatabase, string collectionname = "null")
        {
            return mongoDatabase.GetCollection<T>((collectionname == "null" || string.IsNullOrEmpty(collectionname)) ? typeof(T).Name : collectionname);
        }
        public static  void InitTables(this IMongoDatabase mongoDatabase, List<Type> types) 
        {
            //Type workerType = typeof(IMongoDatabase);
            //MethodInfo doWorkMethod = workerType.GetMethod("InitTables");
            bool isinit = true;
            foreach (var item in types)
            {
                var filter = new BsonDocument("name", item.Name);

                var collections =  mongoDatabase.ListCollections(new ListCollectionsOptions { Filter = filter });

                if ( collections.Any() == false)
                {
                    try
                    {
                        mongoDatabase.CreateCollection(item.Name);
                        Console.WriteLine($"------创建【 {item.Name} 】表成功------");
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                   
                    //MethodInfo curMethod = doWorkMethod.MakeGenericMethod(item);
                    //curMethod.Invoke(mongoDatabase, new object[] { Activator.CreateInstance(item) });
                }
                else
                {
                    isinit = false;
                }
            }
            if (isinit)
            {
                Console.WriteLine("初始化tables");
                mongoDatabase.DbGetCollection<Flow>().InsertOne(new Flow());
                mongoDatabase.DbGetCollection<FlowType>().InsertOne(new FlowType());
                mongoDatabase.DbGetCollection<BusinessDepartment>().InsertOne(new BusinessDepartment());
                mongoDatabase.DbGetCollection<Company>().InsertOne(new Company());
                mongoDatabase.DbGetCollection<FinancialStaff>().InsertOne(new FinancialStaff());
                mongoDatabase.DbGetCollection<IncomeExpenseStatement>().InsertOne(new IncomeExpenseStatement());
                mongoDatabase.DbGetCollection<Project>().InsertOne(new Project());
                mongoDatabase.DbGetCollection<SupportingMaterial>().InsertOne(new SupportingMaterial());
                LogHelper.Log(IdGenFunc.CreateOneId(),"Api服务开始",OprateType.Other);
            }

            //mongoDatabase.DbGetCollection<Flow>().DeleteOne(s => s.Id == 0);
            //mongoDatabase.DbGetCollection<FlowType>().DeleteOne(s => s.Id == 0);
            //mongoDatabase.DbGetCollection<BusinessDepartment>().DeleteOne(s => s.Id == 0);
            //mongoDatabase.DbGetCollection<Company>().DeleteOne(s => s.Id == 0);
            //mongoDatabase.DbGetCollection<FinancialStaff>().DeleteOne(s => s.Id == 0);
            //mongoDatabase.DbGetCollection<IncomeExpenseStatement>().DeleteOne(s => s.Id == 0);
            //mongoDatabase.DbGetCollection<Project>().DeleteOne(s => s.Id == 0);
            //mongoDatabase.DbGetCollection<SupportingMaterial>().DeleteOne(s => s.Id == 0);
        }
        public static async void InitTables<T>(this IMongoDatabase mongoDatabase, T t) where T : new()
        {
            var filter = new BsonDocument("name", typeof(T).Name);

            var collections = await mongoDatabase.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });

            if (await collections.AnyAsync() == false)
            {
              await  mongoDatabase.DbGetCollection<T>().InsertOneAsync(new T());
            }
        }
    }
}
