using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketAppApi.Model
{
    // 流水模型
    public class Flow
    {
        [BsonElement("_id")]
        public long Id { get; set; } // 流水ID，主键
        public long ProjectId { get; set; } // 所属项目ID，外键
        public string? SerialNumber { get; set; } // 流水编号
        public string? DocumentNumber { get; set; } // 单据编号
        public long FlowTypeId { get; set; } // 流水类型
        public double InAmount { get; set; } // 入账
        public double OutAmount { get; set; } // 出张
        public long FinancialStaffId { get; set; } // 经手人ID，外键
        public string? FinanceManager { get;set; } // 财务负责人
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public string? Remark { get; set; } // 描述
    }
}
