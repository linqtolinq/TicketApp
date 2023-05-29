using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketAppApi.Model
{
    public class SupportingMaterial
    {
        [BsonElement("_id")]
        public long Id { get; set; }
        /// <summary>
        /// 流水id，外键
        /// </summary>
        public long FlowId { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public string? FileStream { get; set; }
    }
}
