using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicroMall.ISerivces
{
    public interface IOrderServices
    {
        /// <summary>
        /// 获取订单号
        /// </summary>
        /// <returns></returns>
        string GetOrderNum();
    }
}
