using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketAppApi.Model
{
    // 公司模型
    public class Company
    {
        [BsonElement("_id")]
        public long Id { get; set; } // 公司ID，主键
        public string? Name { get; set; } // 公司名称
        public string? ManagerName { get; set; } // 公司名称
        public string? RegisteredLocation { get; set; } // 公司注册地
        public string? Remark { get; set; } // 描述
        public string? PhoneNumber { get; set; } // 公司联系电话
        public DateTime CreateTime { get; set; } = DateTime.Now;

    }
}
