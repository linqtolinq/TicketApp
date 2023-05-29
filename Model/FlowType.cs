using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TicketAppApi.Model
{
    public class FlowType
    {
        [BsonElement("_id")]
        public long Id { get; set; } // 职务ID，主键
        [Required]
        public string? Name { get; set; } // 流水类别名称
        public string? Remark { get; set; } // 描述
    }
}
