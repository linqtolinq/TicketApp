using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TicketAppApi.Model;

namespace TicketAppApi.Controllers.ApiFormModel
{
    public class BusinessDepartmentAddDto
    {
        [Required]
        public string? Name { get; set; } // 业务部名称
        [Required]
        public string? ManagerName { get; set; } // 业务部名称
        [Required]
        public long CompanyId { get; set; } // 所属公司ID，外键
        public string? Remark { get; set; } // 描述
    }
    public class BusinessDepartmentUpdateDto
    {
        [Required]
        public long Id { get; set; } // 业务部ID，主键
        [Required]
        public string? Name { get; set; } // 业务部名称
        [Required]
        public string? ManagerName { get; set; } // 业务部名称
        public string? Remark { get; set; } // 描述
    }
}
