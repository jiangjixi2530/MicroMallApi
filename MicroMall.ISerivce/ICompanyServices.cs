using MicroMall.Model;
using MicroMall.Model.DbModels;

namespace MicroMall.ISerivces
{
    public interface ICompanyServices
    {
        #region 查询
        /// <summary>
        /// 根据用户名获取企业信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        mall_company GetCompanyByUserName(string userName);
        /// <summary>
        /// 根据手机号获取企业信息
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        mall_company GetCompanyByPhone(string phone);
        /// <summary>
        /// 根据id获取企业信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        mall_company GetCompayById(int id);
        #endregion
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="name">企业名称</param>
        /// <param name="userName">用户名</param>
        /// <param name="pwd">密码</param>
        /// <param name="phone">手机号</param>
        /// <param name="areaId">区县Id</param>
        /// <returns></returns>
        int Register(string name, string userName, string pwd,string phone,int areaId);
    }
}
