using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketAppApi.Model
{
    // 项目模型
    public class Project
    {
        [BsonElement("_id")]
        public long Id { get; set; } // 项目ID，主键
        public string? Name { get; set; } // 项目名称
        public string? ManagerName { get; set; } // 项目名称
        public string? ProjectNumber { get; set; } // 项目编号
        public string? Remark { get; set; } // 项目简介
        public long BusinessDepartmentId { get; set; } // 所属业务部ID，外键
        public double ContractAmount { get; set; } // 合同金额
        public double ReceivedAmount { get; set; } // 到账金额
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }

}
