using ServiceStack.DataAnnotations;
using TicketAppApi.Model;

namespace TicketAppApi.Controllers.ApiFormModel
{
    public class ProjectAddDto
    {
        [Required]
        public string? Name { get; set; } // 项目名称
        [Required]
        public string? ManagerName { get; set; } // 项目名称
        [Required]
        public string? ProjectNumber { get; set; } // 项目编号
        [Required]
        public long BusinessDepartmentId { get; set; } // 所属业务部ID，外键
        [Required]
        public double ContractAmount { get; set; } // 合同金额
        public string? Remark { get; set; } // 项目简介
    }
    public class ProjectUpdateDto
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public string? Name { get; set; } // 项目名称
        [Required]
        public string? ManagerName { get; set; } // 项目名称
        [Required]
        public string? ProjectNumber { get; set; } // 项目编号
        [Required]
        public double ContractAmount { get; set; } // 合同金额
        [Required]
        public double ReceivedAmount { get; set; } // 到账金额
        public string? Remark { get; set; } // 项目简介
    }
}
