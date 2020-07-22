using MicroMall.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MicroMall.IRepository
{
    public interface IBaseRepository
    {
        #region 事务

        /// <summary>
        /// 初始化事务
        /// </summary>
        /// <param name="level"></param>
        void BeginTran(IsolationLevel level = IsolationLevel.ReadCommitted);

        /// <summary>
        /// 完成事务
        /// </summary>
        void CommitTran();

        /// <summary>
        /// 完成事务
        /// </summary>
        void RollbackTran();
        #endregion
        #region 新增 
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock">是否加锁</param>
        /// <returns>新增是否成功</returns>
        Task<bool> Add<T>(T entity, bool isLock = false) where T : class, new();

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock">是否加锁</param>
        /// <returns>返回实体</returns>
        Task<T> AddReturnEntity<T>(T entity, bool isLock = false) where T : class, new();

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock">是否加锁</param>
        /// <returns>返回Id</returns>
        Task<int> AddReturnId<T>(T entity, bool isLock = false) where T : class, new();

        /// <summary>
        /// 新增
        /// </summary> 
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock">是否加锁</param>
        /// <returns>返回bool, 并将identity赋值到实体</returns>
        Task<bool> AddReturnBool<T>(T entity, bool isLock = false) where T : class, new();

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entitys">泛型集合</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>返回bool, 并将identity赋值到实体</returns>
        Task<bool> AddReturnBool<T>(List<T> entitys, bool isLock = false) where T : class, new();

        #endregion
        #region 修改 

        /// <summary>
        /// 修改（主键是更新条件）
        /// </summary>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>修改是否成功</returns>
        Task<bool> Update<T>(T entity, bool isLock = false) where T : class, new();

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="update"> 实体对象 </param> 
        /// <param name="where"> 条件 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>修改是否成功</returns>
        Task<bool> Update<T>(Expression<Func<T, T>> update, Expression<Func<T, bool>> where, bool isLock = false) where T : class, new();

        /// <summary>
        /// 修改（主键是更新条件）
        /// </summary>
        /// <param name="entitys"> 实体对象集合 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>修改是否成功</returns>
        Task<bool> Update<T>(List<T> entitys, bool isLock = false) where T : class, new();

        #endregion
        #region 删除

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        Task<bool> Delete<T>(T entity, bool isLock = false) where T : class, new();

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="where"> 条件 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        Task<bool> Delete<T>(Expression<Func<T, bool>> where, bool isLock = false) where T : class, new();

        /// <summary>
        /// 删除所有
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns></returns>
        Task<bool> DeleteAll<T>(bool isLock = false) where T : class, new();
        /// <summary>
        /// 根据主键物理删除实体对象
        /// </summary>
        /// <param name="id">主键</param>
        ///<param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        Task<bool> DeleteById<T>(dynamic id, bool isLock = false) where T : class, new();

        /// <summary>
        /// 根据主键批量物理删除实体集合
        /// </summary>
        /// <param name="ids">主键集合</param>
        ///<param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        Task<bool> DeleteByIds<T>(dynamic[] ids, bool isLock = false) where T : class, new();

        #endregion
        #region 查询

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <returns>实体</returns>
        Task<List<T>> GetList<T>() where T : class, new();

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="whereLambda">查询表达式</param>
        /// <param name="orderbyLambda">排序表达式</param> 
        /// <param name="isAsc">是否升序</param> 
        /// <returns>实体</returns>
        Task<List<T>> GetList<T>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, object>> orderbyLambda = null, bool isAsc = true) where T : class, new();

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns>实体</returns>
        //List<T> GetList<T>(string sql) where T : class, new();
        Task<List<T>> GetList<T>(string sql, object parameters) where T : class, new();

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="whereLambda">查询表达式</param>
        /// <returns>实体</returns>
        Task<DataTable> GetDataTable<T>(Expression<Func<T, bool>> whereLambda) where T : class, new();

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns>实体</returns>
        Task<DataTable> GetDataTable(string sql);

        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <param name="whereLambda">查询表达式</param> 
        /// <param name="orderbyLambda">排序表达式</param> 
        /// <param name="isAsc">是否升序</param> 
        /// <returns></returns>
        Task<T> Single<T>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, object>> orderbyLambda = null, bool isAsc = true) where T : class, new();

        /// <summary>
        /// 根据主键获取实体对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetById<T>(dynamic id) where T : class, new();

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="whereLambda">查询表达式</param> 
        /// <returns></returns>
        Task<bool> IsExist<T>(Expression<Func<T, bool>> whereLambda) where T : class, new();
        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        Task<int> ExecuteSql(string sql);
        #endregion

        #region 分页查询

        /// <summary>
        /// 获取分页列表【页码，每页条数】
        /// </summary>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        Task<PageBean<T>> GetPageList<T>(int pageIndex, int pageSize) where T : class, new();

        /// <summary>
        /// 获取分页列表【排序，页码，每页条数】
        /// </summary>
        /// <param name="orderExp">排序表达式</param>
        /// <param name="isAsc">排序类型</param>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        Task<PageBean<T>> GetPageList<T>(Expression<Func<T, object>> orderExp, bool isAsc, int pageIndex, int pageSize) where T : class, new();

        /// <summary>
        /// 获取分页列表【Linq表达式条件，页码，每页条数】
        /// </summary>
        /// <param name="whereExp">Linq表达式条件</param>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        Task<PageBean<T>> GetPageList<T>(Expression<Func<T, bool>> whereExp, int pageIndex, int pageSize) where T : class, new();

        /// <summary>
        /// 获取分页列表【Linq表达式条件，排序，页码，每页条数】
        /// </summary>
        /// <param name="whereExp">Linq表达式条件</param>
        /// <param name="orderExp">排序表达式</param>
        /// <param name="isAsc">排序类型</param>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        Task<PageBean<T>> GetPageList<T>(Expression<Func<T, bool>> whereExp, Expression<Func<T, object>> orderExp, bool isAsc, int pageIndex, int pageSize) where T : class, new();

        #endregion
    }
}
