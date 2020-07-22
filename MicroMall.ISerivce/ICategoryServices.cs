using MicroMall.Model.DbModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicroMall.ISerivces
{
    public interface ICategoryServices
    {
        /// <summary>
        /// 获取企业的分类信息
        /// </summary>
        /// <param name="groupId">集团Id</param>
        /// <returns></returns>
        Task<List<mall_category>> GetCategories(int groupId);
    }
}
