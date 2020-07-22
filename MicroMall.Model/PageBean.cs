using System;
using System.Collections.Generic;
using System.Text;

namespace MicroMall.Model
{
    /// <summary>
    /// 分页信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageBean<T>
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public virtual int TotalCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 每页条数
        /// </summary>
        public virtual int PageSize
        {
            get;
            private set;
        }

        /// <summary>
        /// 当前页数据源
        /// </summary>
        public virtual IList<T> Data
        {
            get;
            private set;
        }

        /// <summary>
        /// 总页数
        /// </summary>
        public int? TotalPage
        {
            get
            {
                int total = (int)Math.Ceiling((double)TotalCount / PageSize);
                return total > 0 ? total : 1;
            }
        }

        public PageBean(int totalCount, int pageSize, IList<T> data)
        {
            this.TotalCount = totalCount;
            this.Data = data;
            this.PageSize = pageSize;
        }
    }
}
