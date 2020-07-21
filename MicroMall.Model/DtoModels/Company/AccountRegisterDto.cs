using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MicroMall.Model.DtoModels.Company
{
    /// <summary>
    /// 账号注册
    /// </summary>
    public class AccountRegisterDto
    {
        /// <summary>
        /// 企业名称
        /// </summary>
        [Required]
        public string CompanyName { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string PassWord { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [Required]
        public string Phone { get; set; }
        /// <summary>
        /// 区县Id
        /// </summary>
        public string AreaId { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string Relation { get; set; }
    }
}
