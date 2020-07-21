using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroMall.ISerivces;
using MicroMall.Model;
using MicroMall.Model.DtoModels.Company;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MicroMall.Api.Controllers
{
    /// <summary>
    /// 企业信息相关
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        public ICompanyServices CompanyServices { get; }
        public CompanyController(ICompanyServices companyServices)
        {
            CompanyServices = companyServices;
        }
        /// <summary>
        /// 账号注册
        /// </summary>
        /// <param name="accountRegister"></param>
        /// <returns></returns>
        [HttpPost("companyRegister.htm")]
        public ActionResult<ResponseResult<bool>> CompanyRegister(AccountRegisterDto accountRegister)
        {
            return ResponseResult<bool>.ToSuccess(true);
        }
    }
}
