using MongoDB.Bson.Serialization.Attributes;

namespace TicketAppApi.Model
{
    public class Relation
    {
        [BsonElement("_id")]
        public long Id { get; set; } 
        public long FinancialStaffId { get; set; } 
        public long BusinessDepartmentId { get; set; } 
    }
}
