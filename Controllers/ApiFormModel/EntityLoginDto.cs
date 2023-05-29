using ServiceStack.DataAnnotations;
using TicketAppApi.Model;

namespace TicketAppApi.Controllers.ApiFormModel
{
    public class EntityLoginDto
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? PassWord { get; set; }
    }

    public class EntityAddDto
    {
        /// <summary>
        /// 手机号
        /// </summary>
        [Required]
        public string? UserName { get; set; } // 财务人员账号
        [Required]
        public string? Password { get; set; } // 财务人员密码
        [Required]
        public string? Name { get; set; } // 财务人员姓名
        public string? Remark { get; set; } // 描述
        /// <summary>
        /// 昵称
        /// </summary>
        public string? Nick { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [Required]
        public string? Email { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [Required]
        public string? Address { get; set; }
        [Required]
        public Role Role { get; set; }
    }
    public class EntityUpdateDto
    {
        [Required]
        public string? Name { get; set; } // 财务人员姓名
        public string? Remark { get; set; } // 描述
        public string? Nick { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Address { get; set; }
    }

    public class EntityUpdatePwdDto
    {
        [Required]
        public string? OldPwd { get; set; } // 描述
        [Required]
        public string? NewPwd { get; set; }
        [Required]
        public string? NewConfirmPwd { get; set; }

    }
}
