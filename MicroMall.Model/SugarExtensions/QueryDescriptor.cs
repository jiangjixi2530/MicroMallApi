using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroMall.Model.SugarExtensions
{
    /// <summary>
    /// 排序类型
    /// </summary>
    public enum OrderSequence
    {
        Asc,
        Desc
    }
    /// <summary>
    /// 排序枚举
    /// </summary>
    public class OrderByClause
    {
        public string Sort { get; set; }
        public OrderSequence Order { get; set; }
    }
    public class QueryDescriptor
    {
        /// <summary>
        /// 行数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public List<OrderByClause> OrderBys { get; set; }
        /// <summary>
        /// 条件
        /// </summary>
        public List<ConditionalModel> Conditions { get; set; }
    }
}
