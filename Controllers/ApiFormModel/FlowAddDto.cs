using MongoDB.Bson.Serialization.Attributes;
using ServiceStack.DataAnnotations;
using TicketAppApi.Model;

namespace TicketAppApi.Controllers.ApiFormModel
{
    public class FlowAddDto
    {
        [Required]
        public long ProjectId { get; set; } // 所属项目ID，外键
        //[Required]
        //public string? SerialNumber { get; set; } // 流水编号
        [Required]
        public string? DocumentNumber { get; set; } // 单据编号
        [Required]
        public long FlowTypeId { get; set; } // 流水类型
        [Required]
        public double InAmount { get; set; } // 入账
        [Required]
        public double OutAmount { get; set; } // 出张
        //[Required]
        //public string? FinanceManager { get; set; } // 财务负责人
        public string? Remark { get; set; } // 描述

        public List<SupportingMaterial>? Files { get; set; }
    }

    public class FlowUpdateDto
    {
        [Required]
        public long Id { get; set; } // 主键
        [Required]
        public long ProjectId { get; set; } // 所属项目ID，外键
        [Required]
        public string? DocumentNumber { get; set; } // 单据编号
        [Required]
        public long FlowTypeId { get; set; } // 流水类型
        [Required]
        public double InAmount { get; set; } // 入账
        [Required]
        public double OutAmount { get; set; } // 出张
        public string? Remark { get; set; } // 描述
        public List<SupportingMaterial>? Files { get; set; }
    }
    public class OutFlow
    {
        public long Id { get; set; } // 流水ID，主键
        public long ProjectId { get; set; } // 所属项目ID，外键
        public string? SerialNumber { get; set; } // 流水编号
        public string? DocumentNumber { get; set; } // 单据编号
        public string? FlowType { get; set; } // 单据编号
        public long FlowTypeId { get; set; } // 流水类型
        public double InAmount { get; set; } // 入账
        public double OutAmount { get; set; } // 出张
        public long FinancialStaffId { get; set; } // 经手人ID，外键
        public string? FinanceManager { get; set; } // 财务负责人
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public string? Remark { get; set; } // 描述
    }
}
