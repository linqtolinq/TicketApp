using Microsoft.VisualBasic;
using MongoDB.Driver;
using MQTTDashWebApi.Extension;
using System.Collections.Concurrent;
using TicketAppApi.Extension;
using TicketAppApi.MongoDbHelper;

namespace TicketAppApi.Model
{
    public class LogSystem
    {
        public long Id { get; set; }
        public long OpratorId { get; set; }
        public FinancialStaff? Oprator { get; set; }
        public string? Message { get; set; }
        public OprateType? Type { get; set; } = OprateType.Other;
        public DateTime? Time { get; set; }

    }

    public enum OprateType
    {
        Delete = 0,
        Update = 1,
        Insert = 2,
        Other = 3,
    }
    public static class LogHelper
    {
        public static void Log(long Uid, string msg, OprateType oprateType)
        {
            logqueue.Enqueue(new LogSystem()
            {
                Id = IdGenFunc.CreateOneId(),
                Message = msg,
                OpratorId = Uid,
                Time = DateTime.Now,
                Type = oprateType,
            });
        }
        public static void LogDelete(long Uid, string msg)
        {
            logqueue.Enqueue(new LogSystem()
            {
                Id = IdGenFunc.CreateOneId(),
                Message = msg,
                OpratorId = Uid,
                Time = DateTime.Now,
                Type = OprateType.Delete,
            });
        }

        public static void LogInsert(long Uid, string msg)
        {
            logqueue.Enqueue(new LogSystem()
            {
                Id = IdGenFunc.CreateOneId(),
                Message = msg,
                OpratorId = Uid,
                Time = DateTime.Now,
                Type = OprateType.Insert
            });
        }
        public static void LogInfo(long Uid, string msg)
        {
            logqueue.Enqueue(new LogSystem()
            {
                Id = IdGenFunc.CreateOneId(),
                Message = msg,
                OpratorId = Uid,
                Time = DateTime.Now,
                Type = OprateType.Other
            });
        }
        public static void LogUpdate(long Uid, string msg)
        {
            logqueue.Enqueue(new LogSystem()
            {
                Id = IdGenFunc.CreateOneId(),
                Message = msg,
                Type = OprateType.Insert,
                OpratorId = Uid,
                Time = DateTime.Now
            });
        }
        public static void LogInit(this IServiceCollection serviceDescriptors)
        {
            LogSystem data = null;
            (new Thread(async () =>
            {
                while (true)
                {
                    if (logqueue.TryDequeue(out data))
                    {
                        var user = await DBHelper.Db.DbGetCollection<FinancialStaff>().Find(s => s.Id == data.OpratorId).FirstOrDefaultAsync();
                        user.Password = "";
                        user.Salt = "";
                        data.Oprator = user;
                        await DBHelper.Db.DbGetCollection<LogSystem>().InsertOneAsync(data);
                    }
                }
            })).Start();
        }
        public static ConcurrentQueue<LogSystem> logqueue = new ConcurrentQueue<LogSystem>();

    }
}
