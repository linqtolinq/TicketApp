using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using TicketAppApi.Extension;

namespace TicketAppApi.Model
{
    // 财务人员模型
    public class FinancialStaff
    {
        [BsonElement("_id")]
        public long Id { get; set; } // 财务人员ID，主键
        /// <summary>
        /// 账号（手机号）
        /// </summary>
        public string? UserName { get; set; } // 财务人员账号
        public string? Password { get; set; } // 财务人员密码
        public DateTime CreateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 是佛删除
        /// </summary>
        public bool? IsDeleted { get; set; } = false;
        /// <summary>
        /// 姓名
        /// </summary>
        public string? Name { get; set; } // 财务人员姓名
        public string? Remark { get; set; } // 描述
        /// <summary>
        /// 盐
        /// </summary>
        public string? Salt { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string? Icon { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string? Nick { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// 登录ip
        /// </summary>
        public string? Ip { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string? Address { get; set; }

        public Role Role { get; set; } = Role.User;
        /// <summary>
        /// 构建密码，MD5盐值加密
        /// </summary>
        public FinancialStaff BuildPassword(string? password = null)
        {
            //如果不传值，那就把自己的password当作传进来的password
            if (password == null)
            {
                if (Password == null)
                {
                    throw new ArgumentNullException("Password不能为空");
                }
                password = Password;
            }
            Salt = TicketAppApi.Extension.MD5Helper.GenerateSalt();
            Password = TicketAppApi.Extension.MD5Helper.SHA2Encode(password, Salt);
            return this;
        }

        /// <summary>
        /// 判断密码和加密后的密码是否相同
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool JudgePassword(string password)
        {
            if (this.Salt is null)
            {
                throw new ArgumentNullException(this.Salt);
            }
            //var p = TicketAppApi.Extension.MD5Helper.SHA2Encode(password, Salt);
            if (Password == TicketAppApi.Extension.MD5Helper.SHA2Encode(password, Salt))
            {
                return true;
            }
            return false;
        }
    }

}
