using MicroMall.Model.SugarExtensions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MicroMall.Model
{
    public class Repository : IRepository
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        private SqlSugarClient DbContext;
        public Repository()
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
        #region 事务

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

        /// <summary>
        /// 新增数据源
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型</typeparam>
        /// <returns>数据源</returns>
        public IInsertable<T> Insertable<T>(T entity) where T : class, new()
        {
            return DbContext.Insertable<T>(entity);
        }
        public IInsertable<T> Insertable<T>(List<T> list) where T : class, new()
        {
            return DbContext.Insertable<T>(list);
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock">是否加锁</param>
        /// <returns>新增是否成功</returns>
        public bool Add<T>(T entity, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                DbContext.Insertable(entity).With(SqlWith.UpdLock)
                : DbContext.Insertable(entity);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="entitys">泛型集合</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>新增是否成功</returns>
        public bool Add<T>(List<T> entitys, bool isLock = false) where T : class, new()
        {
            if (entitys.Count == 0)
                return true;
            var operate = isLock ?
                 DbContext.Insertable(entitys).With(SqlWith.UpdLock)
                 : DbContext.Insertable(entitys);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock">是否加锁</param>
        /// <returns>返回实体</returns>
        public T AddReturnEntity<T>(T entity, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                DbContext.Insertable(entity).With(SqlWith.UpdLock)
                : DbContext.Insertable(entity);
            var result = operate.ExecuteReturnEntity();
            return result;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock">是否加锁</param>
        /// <returns>返回Id</returns>
        public int AddReturnId<T>(T entity, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                DbContext.Insertable(entity).With(SqlWith.UpdLock)
                : DbContext.Insertable(entity);
            var result = operate.ExecuteReturnIdentity();
            return result;

        }

        /// <summary>
        /// 新增
        /// </summary> 
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock">是否加锁</param>
        /// <returns>返回bool, 并将identity赋值到实体</returns>
        public bool AddReturnBool<T>(T entity, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                DbContext.Insertable(entity).With(SqlWith.UpdLock)
                : DbContext.Insertable(entity);
            var result = operate.ExecuteCommandIdentityIntoEntity();
            return result;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="entitys">泛型集合</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>返回bool, 并将identity赋值到实体</returns>
        public bool AddReturnBool<T>(List<T> entitys, bool isLock = false) where T : class, new()
        {
            if (entitys.Count == 0)
                return true;
            var operate = isLock ?
                DbContext.Insertable(entitys).With(SqlWith.UpdLock)
                : DbContext.Insertable(entitys);
            var result = operate.ExecuteCommandIdentityIntoEntity();
            return result;
        }

        #endregion

        #region 修改 

        /// <summary>
        /// 修改数据源
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型</typeparam>
        /// <returns>数据源</returns>
        public IUpdateable<T> Updateable<T>() where T : class, new()
        {
            return DbContext.Updateable<T>();
        }

        /// <summary>
        /// 修改（主键是更新条件）
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>修改是否成功</returns>
        public bool Update<T>(T entity, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                DbContext.Updateable(entity).With(SqlWith.UpdLock)
                : DbContext.Updateable(entity);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="update"> 实体对象 </param> 
        /// <param name="where"> 条件 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>修改是否成功</returns>
        public bool Update<T>(Expression<Func<T, T>> update, Expression<Func<T, bool>> where, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                DbContext.Updateable<T>().UpdateColumns(update).Where(where).With(SqlWith.UpdLock)
                : DbContext.Updateable<T>().UpdateColumns(update).Where(where);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="update"> 匿名对象 </param> 
        /// <param name="where"> 条件 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>修改是否成功</returns>
        public bool Update<T>(Expression<Func<T, object>> update, Expression<Func<T, bool>> where, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
               DbContext.Updateable<T>().UpdateColumns(update).Where(where).With(SqlWith.UpdLock)
               : DbContext.Updateable<T>().UpdateColumns(update).Where(where);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 修改（主键是更新条件）
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="entitys"> 实体对象集合 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>修改是否成功</returns>
        public bool Update<T>(List<T> entitys, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
               DbContext.Updateable(entitys).With(SqlWith.UpdLock)
                : DbContext.Updateable(entitys);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        #endregion

        #region 删除

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="list"> 实体对象列表 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public bool Delete<T>(List<T> list, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
               DbContext.Deleteable(list).With(SqlWith.UpdLock)
                : DbContext.Deleteable(list);
            var result = operate.ExecuteCommand();
            return result > 0;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public bool Delete<T>(T entity, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
               DbContext.Deleteable(entity).With(SqlWith.UpdLock)
                : DbContext.Deleteable(entity);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="where"> 条件 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public bool Delete<T>(Expression<Func<T, bool>> where, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                    DbContext.Deleteable<T>().Where(where).With(SqlWith.UpdLock)
                   : DbContext.Deleteable<T>().Where(where);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 删除所有
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public bool DeleteAll<T>(bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                    DbContext.Deleteable<T>().With(SqlWith.UpdLock)
                   : DbContext.Deleteable<T>();
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 根据主键物理删除实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">主键</param>
        ///<param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public bool DeleteById<T>(dynamic id, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                  DbContext.Deleteable<T>().In(id).With(SqlWith.UpdLock)
                 : DbContext.Deleteable<T>().In(id);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 根据主键批量物理删除实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids">主键集合</param>
        ///<param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public bool DeleteByIds<T>(dynamic[] ids, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                DbContext.Deleteable<T>().In(ids).With(SqlWith.UpdLock)
                : DbContext.Deleteable<T>().In(ids);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        #endregion

        #region 查询

        /// <summary>
        /// 查询数据源
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型</typeparam>
        /// <returns>数据源</returns>
        public ISugarQueryable<T> Queryable<T>() where T : class, new()
        {
            return DbContext.Queryable<T>();
        }

        /// <summary>
        /// 多表连接查询数据源
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型</typeparam>
        /// <returns>数据源</returns>
        public ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, object[]>> joinExpression)
        {
            return DbContext.Queryable<T, T2>(joinExpression);
        }

        /// <summary>
        /// 多表连接查询数据源
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型</typeparam>
        /// <returns>数据源</returns>
        public ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, object[]>> joinExpression)
        {
            return DbContext.Queryable<T, T2, T3>(joinExpression);
        }
        /// <summary>
        /// 查询集合
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="whereLambda">查询表达式</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>实体</returns>
        public List<T> GetList<T>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, object>> orderbyLambda = null, bool isAsc = true) where T : class, new()
        {
            var query = DbContext.Queryable<T>().With(SqlWith.NoLock).Where(whereLambda);
            if (orderbyLambda != null)
            {
                query = query.OrderBy(orderbyLambda, isAsc ? OrderByType.Asc : OrderByType.Desc);
            }
            return query.ToList();
        }

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <returns>实体</returns>
        public List<T> GetList<T>() where T : class, new()
        {
            var query = Queryable<T>();
            return query.ToList();
        }


        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns>实体</returns> 
        public List<T> GetList<T>(string sql, object parameters) where T : class, new()
        {
            return DbContext.Ado.SqlQuery<T>(sql, parameters);
        }

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns>实体</returns>
        public List<T> GetList<T>(string sql, params SugarParameter[] parameters) where T : class, new()
        {
            return DbContext.Ado.SqlQuery<T>(sql, parameters);
        }

        /// <summary>
        /// 根据条件获取实体列表
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="conditionals">Sugar调价表达式集合</param>
        /// <returns>实体</returns>
        public List<T> GetList<T>(List<IConditionalModel> conditionals) where T : class, new()
        {
            var query = DbContext.Queryable<T>().With(SqlWith.NoLock).Where(conditionals);
            return query.ToList();
        }

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="whereLambda">查询表达式</param>
        /// <returns>实体</returns>
        public DataTable GetDataTable<T>(Expression<Func<T, bool>> whereLambda) where T : class, new()
        {
            var query = DbContext.Queryable<T>().With(SqlWith.NoLock).Where(whereLambda);
            return query.ToDataTable();
        }

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="sql">sql</param>
        /// <returns>实体</returns>
        public DataTable GetDataTable<T>(string sql) where T : class, new()
        {
            var query = DbContext.SqlQueryable<T>(sql).With(SqlWith.NoLock);
            return query.ToDataTable();
        }
        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql)
        {
            return DbContext.Ado.GetDataTable(sql);
        }
        /// <summary>
        /// 查询存储过程
        /// </summary> 
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameters">参数</param>
        public DataTable GetProcedure(string procedureName, List<SugarParameter> parameters)
        {
            var datas = DbContext.Ado.UseStoredProcedure().GetDataTable(procedureName, parameters);
            return datas;
        }

        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型</typeparam>
        /// <param name="whereLambda">查询表达式</param> 
        /// <param name="orderbyLambda">排序表达式</param> 
        /// <param name="isAsc">是否升序</param> 
        /// <returns></returns>
        public T Single<T>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, object>> orderbyLambda = null, bool isAsc = true) where T : class, new()
        {
            var query = DbContext.Queryable<T>().With(SqlWith.NoLock).Where(whereLambda);

            if (orderbyLambda != null)
            {
                query = query.OrderBy(orderbyLambda, isAsc ? OrderByType.Asc : OrderByType.Desc);
            }

            var rs = query.First();

            return rs;
        }

        /// <summary>
        /// 根据主键获取实体对象
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetById<T>(dynamic id) where T : class, new()
        {
            var query = DbContext.Queryable<T>().With(SqlWith.NoLock);

            return query.InSingle(id);
        }

        /// <summary>
        /// 获取首行首列
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public object GetScalar(string sql, object parameters)
        {
            return DbContext.Ado.GetScalar(sql, parameters);
        }

        /// <summary>
        /// 获取首行首列
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public object GetScalar(string sql, params SugarParameter[] parameters)
        {
            return DbContext.Ado.GetScalar(sql, parameters);
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型</typeparam>
        /// <param name="whereLambda">查询表达式</param> 
        /// <returns></returns>
        public bool IsExist<T>(Expression<Func<T, bool>> whereLambda) where T : class, new()
        {
            var datas = DbContext.Queryable<T>().Any(whereLambda);
            return datas;
        }
        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="sql"></param>
        public int ExecuteSql(string sql)
        {
            return DbContext.Ado.ExecuteCommand(sql);
        }

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns>实体</returns>
        public List<T> GetSugarList<T>(string sql, SugarParameter[] parameters) where T : class, new()
        {
            var para = new List<SugarParameter>(parameters);
            return DbContext.Ado.SqlQuery<T>(sql, parameters);
        }
        #endregion

        #region 分页查询

        /// <summary>
        /// 获取分页列表【页码，每页条数】
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public PageBean<T> GetPageList<T>(int pageIndex, int pageSize) where T : class, new()
        {
            int count = 0;
            var query = DbContext.Queryable<T>();
            var result = query.ToPageList(pageIndex, pageSize, ref count);
            return new PageBean<T>(count, pageSize, result);
        }

        /// <summary>
        /// 获取分页列表【排序，页码，每页条数】
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="orderExp">排序表达式</param>
        /// <param name="orderType">排序类型</param>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public PageBean<T> GetPageList<T>(Expression<Func<T, object>> orderExp, OrderByType orderType, int pageIndex, int pageSize) where T : class, new()
        {
            int count = 0;
            var query = DbContext.Queryable<T>().OrderBy(orderExp, orderType);
            var result = query.ToPageList(pageIndex, pageSize, ref count);
            return new PageBean<T>(count, pageSize, result);
        }

        /// <summary>
        /// 获取分页列表【Linq表达式条件，页码，每页条数】
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="whereExp">Linq表达式条件</param>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public PageBean<T> GetPageList<T>(Expression<Func<T, bool>> whereExp, int pageIndex, int pageSize) where T : class, new()
        {
            int count = 0;
            var query = DbContext.Queryable<T>().Where(whereExp);
            var result = query.ToPageList(pageIndex, pageSize, ref count);
            return new PageBean<T>(count, pageSize, result);
        }

        /// <summary>
        /// 获取分页列表【Linq表达式条件，排序，页码，每页条数】
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="whereExp">Linq表达式条件</param>
        /// <param name="orderExp">排序表达式</param>
        /// <param name="orderType">排序类型</param>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public PageBean<T> GetPageList<T>(Expression<Func<T, bool>> whereExp, Expression<Func<T, object>> orderExp, OrderByType orderType, int pageIndex, int pageSize) where T : class, new()
        {
            int count = 0;
            var query = DbContext.Queryable<T>().Where(whereExp).OrderBy(orderExp, orderType);
            var result = query.ToPageList(pageIndex, pageSize, ref count);
            return new PageBean<T>(count, pageSize, result);
        }

        /// <summary>
        /// 获取分页列表【Sugar表达式条件，页码，每页条数】
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="conditionals">Sugar条件表达式集合</param>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public PageBean<T> GetPageList<T>(List<IConditionalModel> conditionals, int pageIndex, int pageSize) where T : class, new()
        {
            int count = 0;
            var query = DbContext.Queryable<T>().Where(conditionals);
            var result = query.ToPageList(pageIndex, pageSize, ref count);
            return new PageBean<T>(count, pageSize, result);
        }

        /// <summary>
        ///  获取分页列表【Sugar表达式条件，排序，页码，每页条数】
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="conditionals">Sugar条件表达式集合</param>
        /// <param name="orderExp">排序表达式</param>
        /// <param name="orderType">排序类型</param>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public PageBean<T> GetPageList<T>(List<IConditionalModel> conditionals, Expression<Func<T, object>> orderExp, OrderByType orderType, int pageIndex, int pageSize) where T : class, new()
        {
            int count = 0;
            var query = DbContext.Queryable<T>().Where(conditionals).OrderBy(orderExp, orderType);
            var result = query.ToPageList(pageIndex, pageSize, ref count);
            return new PageBean<T>(count, pageSize, result);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="query">查询条件(不能为空)</param>
        /// <returns>实体列表</returns>
        public PageBean<T> GetPageList<T>(QueryDescriptor query) where T : class, new()
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            var listDatas = DbContext.Queryable<T>();
            if (query.Conditions != null)
            {
                var conds = ParseCondition(query.Conditions);
                listDatas = listDatas.Where(conds);
            }

            if (query.OrderBys != null)
            {
                var orderBys = ParseOrderBy(query.OrderBys);
                listDatas = listDatas.OrderBy(orderBys);
            }

            int count = 0;
            var datas = listDatas.ToPageList(query.PageIndex, query.PageSize, ref count);
            return new PageBean<T>(count, query.PageSize, datas);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="query">查询条件</param>
        /// <param name="totalCount">总行数</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataTablePageList<T>(QueryDescriptor query, out int totalCount) where T : class, new()
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            var listDatas = DbContext.Queryable<T>();
            if (query.Conditions != null)
            {
                var conds = ParseCondition(query.Conditions);
                listDatas = listDatas.Where(conds);
            }
            if (query.OrderBys != null)
            {
                var orderBys = ParseOrderBy(query.OrderBys);
                listDatas = listDatas.OrderBy(orderBys);
            }
            totalCount = 0;
            var datas = listDatas.ToDataTablePage(query.PageIndex, query.PageSize, ref totalCount);
            return datas;
        }

        #endregion
        #region 私有方法

        /// <summary>
        /// 查询条件转换
        /// </summary>
        /// <param name="contitons">查询条件</param>
        /// <returns></returns>
        protected List<IConditionalModel> ParseCondition(List<ConditionalModel> contitons)
        {
            var conds = new List<IConditionalModel>();
            foreach (var con in contitons)
            {
                if (con.FieldName.Contains(","))
                {
                    conds.Add(ParseKeyOr(con));
                }
                else
                {
                    conds.Add(new ConditionalModel()
                    {
                        FieldName = con.FieldName,
                        ConditionalType = con.ConditionalType,
                        FieldValue = con.FieldValue
                    });
                }
            }

            return conds;
        }

        /// <summary>
        /// 转换Or条件
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected ConditionalCollections ParseKeyOr(ConditionalModel condition)
        {
            var objectKeys = condition.FieldName.Split(',');
            var conditionalList = new List<KeyValuePair<WhereType, ConditionalModel>>();
            foreach (var objKey in objectKeys)
            {
                var cond = new KeyValuePair<WhereType, ConditionalModel>
                (WhereType.Or, new ConditionalModel()
                {
                    FieldName = objKey,
                    ConditionalType = condition.ConditionalType,
                    FieldValue = condition.FieldValue
                });
                conditionalList.Add(cond);
            }
            return new ConditionalCollections { ConditionalList = conditionalList };
        }

        /// <summary>
        /// 排序转换
        /// </summary>
        /// <param name="orderBys">排序</param>
        /// <returns></returns>
        protected string ParseOrderBy(List<OrderByClause> orderBys)
        {
            var conds = "";
            foreach (var con in orderBys)
            {
                if (con.Order == OrderSequence.Asc)
                {
                    conds += $"{con.Sort} asc,";
                }
                else if (con.Order == OrderSequence.Desc)
                {
                    conds += $"{con.Sort} desc,";
                }
            }

            return conds.TrimEnd(',');
        }

        /// <summary>
        /// 根据SQLSugar表达式获取Sql语句
        /// </summary>
        /// <param name="keyValuePair"></param>
        /// <returns></returns>
        protected string GetSql(KeyValuePair<string, List<SugarParameter>> keyValuePair)
        {
            var sql = keyValuePair.Key;
            foreach (var para in keyValuePair.Value)
            {
                if (sql.IndexOf(para.ParameterName + ",") >= 0)
                    sql = sql.Replace(para.ParameterName + ",", GetSqlValue(para.Value) + ",");
                else
                    sql = sql.Replace(para.ParameterName, GetSqlValue(para.Value));
            }
            return sql;
        }

        /// <summary>
        /// 值转成sql值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string GetSqlValue(object value)
        {

            if (value == null || Convert.IsDBNull(value))
                return string.Empty;
            if (value is string)
            {
                return string.Format("'{0}'", value.ToString());
            }
            else if (value is DateTime)
            {
                DateTime time = (DateTime)value;
                return string.Format("'{0}'", time.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else if (value is bool)
            {
                return Convert.ToBoolean(value) ? "1" : "0";
            }
            else
            {
                return value.ToString();
            }
        }

        #endregion

    }
}
