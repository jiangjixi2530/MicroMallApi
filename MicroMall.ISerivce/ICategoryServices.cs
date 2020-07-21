using MicroMall.Model.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroMall.ISerivces
{
    public interface ICategoryServices
    {
        /// <summary>
        /// 获取企业的分类信息
        /// </summary>
        /// <param name="groupId">集团Id</param>
        /// <returns></returns>
        List<mall_category> GetCategories(int groupId);
    }
}
