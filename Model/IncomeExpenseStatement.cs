using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketAppApi.Model
{
    // 收支统计报表模型
    public class IncomeExpenseStatement
    {
        [BsonElement("_id")]
        public long Id { get; set; } // 报表ID，主键
        public long CompanyId { get; set; } // 公司ID，外键
        public long BusinessDepartmentId { get; set; } // 业务部ID，外键
        public long FinancialStaffId { get; set; } // 财务人员ID，外键
        public int Year { get; set; } // 年份
        public double Income { get; set; } // 收入
        public double Expense { get; set; } // 支出

        // 定义 Company、BusinessDepartment、FinancialStaff 对象作为外键关系
        public Company? Company { get; set; }
        public BusinessDepartment? BusinessDepartment { get; set; }
        public FinancialStaff? FinancialStaff { get; set; }
        public string? Remark { get; set; } // 描述
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}
