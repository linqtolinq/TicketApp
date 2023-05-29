using ServiceStack.DataAnnotations;

namespace TicketAppApi.Controllers.ApiFormModel
{
    public class CompanyAddDto
    {
        [Required]
        public string? Name { get; set; } // 公司名称
        [Required]
        public string? ManagerName { get; set; } // 公司名称
        [Required]
        public string? RegisteredLocation { get; set; } // 公司注册地
        [Required]
        public string? PhoneNumber { get; set; } // 公司注册地
        public string? Remark { get; set; } // 描述
    }
    public class CompanyUpdateDto
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public string? Name { get; set; } // 公司名称
        [Required]
        public string? ManagerName { get; set; } // 公司名称
        [Required]
        public string? RegisteredLocation { get; set; } // 公司注册地
        [Required]
        public string? PhoneNumber { get; set; } // 公司注册地
        public string? Remark { get; set; } // 描述
    }
}
