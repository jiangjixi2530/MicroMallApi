using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroMall.ISerivces;
using MicroMall.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MicroMall.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IOrderServices orderServices;

        public TestController(IOrderServices orderServices)
        {
            this.orderServices = orderServices;
        }
        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetText(string name)
        {
            return Ok(orderServices.GetOrderNum());
        }
    }
}
