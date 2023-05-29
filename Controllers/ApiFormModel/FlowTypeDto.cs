using System.ComponentModel.DataAnnotations;

namespace TicketAppApi.Controllers.ApiFormModel
{
    public class FlowTypeDto
    {
        [Required]
        public string? Name { get; set; } // 流水类别名称
        public string? Remark { get; set; } // 描述
    }
}
