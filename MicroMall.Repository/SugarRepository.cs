using MicroMall.IRepository;
using MicroMall.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MicroMall.Repository
{
    public class SugarRepository : IBaseRepository
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        private SqlSugarClient DbContext;
        public SugarRepository()
        {
            InitDb();
        }
        private void InitDb()
        {
            string connStr = "Server=127.0.0.1;Database=micromall; User=root;Password=a654321;port=3306";// AppSetting.ConnectionString;
            DbContext = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = connStr,
                DbType = SqlSugar.DbType.MySql,
                IsAutoCloseConnection = true,//可以自动关闭连接，不需要close和using
                IsShardSameThread = false, //设为true相同线程是同一个SqlConnection
                                           //SlaveConnectionConfigs = slaveConnectionConfigs
            });
            DbContext.Ado.CommandTimeOut = 100;//设置超时时间 单位s
                                               //sql 监听
            DbContext.Aop.OnLogExecuted = (sql, pars) => //SQL执行完事件
            {
#if DEBUG
                Console.WriteLine($"{sql}{Environment.NewLine} {GetString(pars)}{Environment.NewLine}[耗时 {DbContext.Ado.SqlExecutionTime.TotalSeconds}s ]");
#else
#endif
            };
            DbContext.Aop.OnLogExecuting = (sql, pars) => //SQL执行前事件
            {
            };
            DbContext.Aop.OnError = (ex) =>//执行SQL 错误事件
            {
            };
            DbContext.Aop.OnDiffLogEvent = it =>
            {
                var editBeforeData = it.BeforeData;
                var editAfterData = it.AfterData;
                var sql = it.Sql;
                var parameter = it.Parameters;
                var data = it.BusinessData;
                var time = it.Time;
                var diffType = it.DiffType;//枚举值 insert 、update 和 delete 用来作业务区分
            };
            DbContext.Aop.OnExecutingChangeSql = (sql, pars) => //SQL执行前 可以修改SQL
            {
                return new KeyValuePair<string, SugarParameter[]>(sql, pars);
            };
        }
        private string GetString(SugarParameter[] pars)
        {
            StringBuilder sbr = new StringBuilder();
            foreach (var i in pars)
            {
                sbr.Append($"{i.ParameterName}:{i.Value.ObjToString()},");
            }
            return sbr.ToString();
        }
        #region  事务
        /// <summary>
        /// 初始化事务
        /// </summary>
        /// <param name="level"></param>
        public void BeginTran(IsolationLevel level = IsolationLevel.ReadCommitted)
        {
            DbContext.Ado.BeginTran(IsolationLevel.Unspecified);
        }

        /// <summary>
        /// 完成事务
        /// </summary>
        public void CommitTran()
        {
            DbContext.Ado.CommitTran();
        }

        /// <summary>
        /// 完成事务
        /// </summary>
        public void RollbackTran()
        {
            DbContext.Ado.RollbackTran();
        }
        #endregion
        #region 新增
        public async Task<bool> Add<T>(T entity, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                DbContext.Insertable(entity).With(SqlWith.UpdLock)
                : DbContext.Insertable(entity);
            var result = await operate.ExecuteCommandAsync();
            return result > 0;
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock">是否加锁</param>
        /// <returns>返回实体</returns>
        public async Task<T> AddReturnEntity<T>(T entity, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                DbContext.Insertable(entity).With(SqlWith.UpdLock)
                : DbContext.Insertable(entity);
            var result = await operate.ExecuteReturnEntityAsync();
            return result;
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock">是否加锁</param>
        /// <returns>返回Id</returns>
        public async Task<int> AddReturnId<T>(T entity, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                DbContext.Insertable(entity).With(SqlWith.UpdLock)
                : DbContext.Insertable(entity);
            var result = await operate.ExecuteReturnIdentityAsync();
            return result;
        }
        /// <summary>
        /// 新增
        /// </summary> 
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock">是否加锁</param>
        /// <returns>返回bool, 并将identity赋值到实体</returns>
        public async Task<bool> AddReturnBool<T>(T entity, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                DbContext.Insertable(entity).With(SqlWith.UpdLock)
                : DbContext.Insertable(entity);
            var result = await operate.ExecuteCommandIdentityIntoEntityAsync();
            return result;
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entitys">泛型集合</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>返回bool, 并将identity赋值到实体</returns>
        public async Task<bool> AddReturnBool<T>(List<T> entitys, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                DbContext.Insertable(entitys).With(SqlWith.UpdLock)
                : DbContext.Insertable(entitys);
            var result = await operate.ExecuteCommandIdentityIntoEntityAsync();
            return result;
        }
        #endregion
        #region 修改
        /// <summary>
        /// 修改（主键是更新条件）
        /// </summary>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>修改是否成功</returns>
        public async Task<bool> Update<T>(T entity, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
               DbContext.Updateable(entity).With(SqlWith.UpdLock)
               : DbContext.Updateable(entity);
            var result = await operate.ExecuteCommandAsync();
            return result > 0;
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="update"> 实体对象 </param> 
        /// <param name="where"> 条件 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>修改是否成功</returns>
        public async Task<bool> Update<T>(Expression<Func<T, T>> update, Expression<Func<T, bool>> where, bool isLock = false) where T : class, new()
        {

            var operate = isLock ?
                DbContext.Updateable<T>().SetColumns(update).Where(where).With(SqlWith.UpdLock)
                : DbContext.Updateable<T>().SetColumns(update).Where(where);
            var result = await operate.ExecuteCommandAsync();
            return result > 0;
        }
        /// <summary>
        /// 修改（主键是更新条件）
        /// </summary>
        /// <param name="entitys"> 实体对象集合 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>修改是否成功</returns>
        public async Task<bool> Update<T>(List<T> entitys, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
               DbContext.Updateable(entitys).With(SqlWith.UpdLock)
                : DbContext.Updateable(entitys);
            var result = await operate.ExecuteCommandAsync();
            return result > 0;
        }
        #endregion
        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public async Task<bool> Delete<T>(T entity, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
               DbContext.Deleteable(entity).With(SqlWith.UpdLock)
                : DbContext.Deleteable(entity);
            var result = await operate.ExecuteCommandAsync();
            return result > 0;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="where"> 条件 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public async Task<bool> Delete<T>(Expression<Func<T, bool>> where, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                    DbContext.Deleteable<T>().Where(where).With(SqlWith.UpdLock)
                   : DbContext.Deleteable<T>().Where(where);
            var result = await operate.ExecuteCommandAsync();
            return result > 0;
        }
        /// <summary>
        /// 删除所有
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns></returns>
        public async Task<bool> DeleteAll<T>(bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                    DbContext.Deleteable<T>().With(SqlWith.UpdLock)
                   : DbContext.Deleteable<T>();
            var result = await operate.ExecuteCommandAsync();
            return result > 0;
        }
        /// <summary>
        /// 根据主键物理删除实体对象
        /// </summary>
        /// <param name="id">主键</param>
        ///<param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public async Task<bool> DeleteById<T>(dynamic id, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                  DbContext.Deleteable<T>().In(id).With(SqlWith.UpdLock)
                 : DbContext.Deleteable<T>().In(id);
            var result = await operate.ExecuteCommandAsync();
            return result > 0;
        }
        /// <summary>
        /// 根据主键批量物理删除实体集合
        /// </summary>
        /// <param name="ids">主键集合</param>
        ///<param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public async Task<bool> DeleteByIds<T>(dynamic[] ids, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                DbContext.Deleteable<T>().In(ids).With(SqlWith.UpdLock)
                : DbContext.Deleteable<T>().In(ids);
            var result = await operate.ExecuteCommandAsync();
            return result > 0;
        }
        #endregion
        #region 查询
        /// <summary>
        /// 查询集合
        /// </summary>
        /// <returns>实体</returns>
        public async Task<List<T>> GetList<T>() where T : class, new()
        {
            var query = DbContext.Queryable<T>();
            return await query.ToListAsync();
        }
        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="whereLambda">查询表达式</param>
        /// <param name="orderbyLambda">排序表达式</param> 
        /// <param name="isAsc">是否升序</param> 
        /// <returns>实体</returns>
        public async Task<List<T>> GetList<T>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, object>> orderbyLambda = null, bool isAsc = true) where T : class, new()
        {
            var query = DbContext.Queryable<T>().With(SqlWith.NoLock).Where(whereLambda);
            if (orderbyLambda != null)
            {
                query = query.OrderBy(orderbyLambda, isAsc ? OrderByType.Asc : OrderByType.Desc);
            }
            return await query.ToListAsync();
        }
        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns>实体</returns>
        //List<T> GetList<T>(string sql) where T : class, new();
        public async Task<List<T>> GetList<T>(string sql, object parameters) where T : class, new()
        {
            return await DbContext.Ado.SqlQueryAsync<T>(sql, parameters);
        }

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="whereLambda">查询表达式</param>
        /// <returns>实体</returns>
        public async Task<DataTable> GetDataTable<T>(Expression<Func<T, bool>> whereLambda) where T : class, new()
        {
            var query = DbContext.Queryable<T>().With(SqlWith.NoLock).Where(whereLambda);
            return await query.ToDataTableAsync();
        }

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns>实体</returns>
        public async Task<DataTable> GetDataTable(string sql)
        {
            return await DbContext.Ado.GetDataTableAsync(sql);
        }
        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <param name="whereLambda">查询表达式</param> 
        /// <param name="orderbyLambda">排序表达式</param> 
        /// <param name="isAsc">是否升序</param> 
        /// <returns></returns>
        public async Task<T> Single<T>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, object>> orderbyLambda = null, bool isAsc = true) where T : class, new()
        {
            var query = DbContext.Queryable<T>().With(SqlWith.NoLock).Where(whereLambda);
            if (orderbyLambda != null)
            {
                query = query.OrderBy(orderbyLambda, isAsc ? OrderByType.Asc : OrderByType.Desc);
            }
            var rs = await query.FirstAsync();
            return rs;
        }
        /// <summary>
        /// 根据主键获取实体对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> GetById<T>(dynamic id) where T : class, new()
        {
            var query = DbContext.Queryable<T>().With(SqlWith.NoLock);
            return await query.InSingleAsync(id);
        }
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="whereLambda">查询表达式</param> 
        /// <returns></returns>
        public async Task<bool> IsExist<T>(Expression<Func<T, bool>> whereLambda) where T : class, new()
        {
            var datas = await DbContext.Queryable<T>().AnyAsync(whereLambda);
            return datas;
        }
        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="sql"></param>
        public async Task<int> ExecuteSql(string sql)
        {
            return await DbContext.Ado.ExecuteCommandAsync(sql);
        }
        #endregion

        #region 分页查询
        /// <summary>
        /// 获取分页列表【页码，每页条数】
        /// </summary>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public async Task<PageBean<T>> GetPageList<T>(int pageIndex, int pageSize) where T : class, new()
        {
            RefAsync<int> count = 0;
            var query = DbContext.Queryable<T>();
            var result = await query.ToPageListAsync(pageIndex, pageSize, count);
            return new PageBean<T>(count, pageSize, result);
        }
        /// <summary>
        /// 获取分页列表【排序，页码，每页条数】
        /// </summary>
        /// <param name="orderExp">排序表达式</param>
        /// <param name="orderType">排序类型</param>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public async Task<PageBean<T>> GetPageList<T>(Expression<Func<T, object>> orderExp, bool isAsc, int pageIndex, int pageSize) where T : class, new()
        {
            RefAsync<int> count = 0;
            OrderByType orderType = isAsc ? OrderByType.Asc : OrderByType.Desc;
            var query = DbContext.Queryable<T>().OrderBy(orderExp, orderType);
            var result = await query.ToPageListAsync(pageIndex, pageSize, count);
            return new PageBean<T>(count, pageSize, result);
        }
        /// <summary>
        /// 获取分页列表【Linq表达式条件，页码，每页条数】
        /// </summary>
        /// <param name="whereExp">Linq表达式条件</param>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public async Task<PageBean<T>> GetPageList<T>(Expression<Func<T, bool>> whereExp, int pageIndex, int pageSize) where T : class, new()
        {
            RefAsync<int> count = 0;
            var query = DbContext.Queryable<T>().Where(whereExp);
            var result = await query.ToPageListAsync(pageIndex, pageSize, count);
            return new PageBean<T>(count, pageSize, result);
        }
        /// <summary>
        /// 获取分页列表【Linq表达式条件，排序，页码，每页条数】
        /// </summary>
        /// <param name="whereExp">Linq表达式条件</param>
        /// <param name="orderExp">排序表达式</param>
        /// <param name="isAsc">排序类型</param>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public async Task<PageBean<T>> GetPageList<T>(Expression<Func<T, bool>> whereExp, Expression<Func<T, object>> orderExp, bool isAsc, int pageIndex, int pageSize) where T : class, new()
        {
            RefAsync<int> count = 0;
            OrderByType orderBy = isAsc ? OrderByType.Asc : OrderByType.Desc;
            var query = DbContext.Queryable<T>().Where(whereExp).OrderBy(orderExp, orderBy);
            var result = await query.ToPageListAsync(pageIndex, pageSize, count);
            return new PageBean<T>(count, pageSize, result);
        }
        #endregion
    }
}
