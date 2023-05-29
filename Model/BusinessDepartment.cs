using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketAppApi.Model
{
    // 业务部模型
    public class BusinessDepartment
    {
        [BsonElement("_id")]
        public long Id { get; set; } // 业务部ID，主键
        public string? Name { get; set; } // 业务部名称
        public string? ManagerName { get; set; } // 业务部名称
        public long CompanyId { get; set; } // 所属公司ID，外键
        public string? Remark { get; set; } // 描述
        public DateTime CreateTime { get; set; } = DateTime.Now;

    }
}
