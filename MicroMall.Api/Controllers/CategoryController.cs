using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroMall.ISerivces;
using MicroMall.Model;
using MicroMall.Model.DbModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MicroMall.Api.Controllers
{
    /// <summary>
    /// 产品分类相关
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        public ICategoryServices CategoryServices { get; }
        public CategoryController(ICategoryServices categoryServices)
        {
            CategoryServices = categoryServices;
        }
        /// <summary>
        /// 获取集团下的所有分类
        /// </summary>
        /// <param name="groupId">集团Id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<ResponseResult<List<mall_category>>> GetCategories(int groupId)
        {
            var categories = CategoryServices.GetCategories(groupId);
            if (categories != null && categories.Count > 0)
            {
                return ResponseResult<List<mall_category>>.ToSuccess(categories);
            }
            else
            {
                return ResponseResult<List<mall_category>>.ToFail("未查询到任何数据", "请检查参数是否正确");
            }
        }
    }
}
