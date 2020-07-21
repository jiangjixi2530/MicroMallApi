using System;
using System.Collections.Generic;
using System.Text;

namespace MicroMall.Model
{
    /// <summary>
    /// 接口返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseResult<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public CodeStatus code { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 详细信息
        /// </summary>
        public string subMsg { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public T data { get; set; }
        /// <summary>
        /// 成功返回
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ResponseResult<T> ToSuccess(T t)
        {
            ResponseResult<T> response = new ResponseResult<T>
            {
                code = CodeStatus.SUCCESS,
                data=t,
                success=true
            };
            return response;
        }
        /// <summary>
        /// 失败返回
        /// </summary>
        /// <param name="msg">失败信息</param>
        /// <param name="subMsg">详细信息</param>
        /// <returns></returns>
        public static ResponseResult<T> ToFail(string msg,string subMsg)
        {
            ResponseResult<T> response = new ResponseResult<T>
            {
                code = CodeStatus.FAIL
            };
            response.msg = msg;
            response.subMsg = subMsg;
            return response;
        }
    }
    public enum CodeStatus
    {
        /// <summary>
        /// 成功
        /// </summary>
        SUCCESS = 10000,
        /// <summary>
        /// 失败
        /// </summary>
        FAIL = 400001
    }
}
