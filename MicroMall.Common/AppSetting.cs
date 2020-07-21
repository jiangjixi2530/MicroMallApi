using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroMall.Common
{
    /// <summary>
    /// 应用级配置
    /// </summary>
    public class AppSetting
    {
        /// <summary>
        /// appsettings.json配置
        /// </summary>
        static IConfiguration Configuration { get; set; }
        public AppSetting(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 封装要操作的字符
        /// </summary>
        /// <param name="sections">节点配置</param>
        /// <returns></returns>
        public static string app(params string[] sections)
        {
            try
            {
                if (sections.Any())
                {
                    return Configuration[string.Join(":", sections)];
                }
            }
            catch (Exception) { }

            return "";
        }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string ConnectionString => Configuration.GetConnectionString("mallDb");
        /// <summary>
        /// 当前时间
        /// </summary>
        public static DateTime NowTime => DateTime.Now;
    }
}
