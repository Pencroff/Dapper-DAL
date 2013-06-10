using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using Dapper_DAL.General;
using Dapper_DAL.Infrastructure.Interfaces;

namespace Dapper_DAL.Infrastructure
{
    public class Repository<T, TRepoQuery> : IRepository<T, TRepoQuery>
        where T : class
        where TRepoQuery : EnumBase<TRepoQuery, string>
    {
        public IDbConnection Conn { get; private set; }
        public IDapperContext Context { get; private set; }
        
        public Repository(IDapperContext context)
        {
            Context = context;
            Conn = Context.Connection;
        }

        public virtual void Add(T entity, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "Add to DB null entity");
            }
            var res = Conn.Insert(entity, transaction: transaction, commandTimeout: commandTimeout);
        }

        public virtual void Update(T entity, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "Update in DB null entity");
            }
            Conn.Update(entity, transaction: transaction, commandTimeout: commandTimeout);
        }

        public virtual void Remove(T entity, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "Remove in DB null entity");
            }
            Conn.Delete(entity, transaction: transaction, commandTimeout: commandTimeout);
        }

        public virtual T GetByKey(object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            return Conn.Get<T>(id, transaction: transaction, commandTimeout: commandTimeout);
        }

        public virtual IEnumerable<T> GetAll(IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return Conn.GetAll<T>(transaction: transaction, commandTimeout: commandTimeout);
        }

        public virtual IEnumerable<T> GetBy(object where = null, object order = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return Conn.GetBy<T>(where: where, order: order, transaction: transaction, commandTimeout: commandTimeout);
        }

        public IEnumerable<TSp> Exec<TSp>(TRepoQuery repoQuery, DynamicParameters param = null, IDbTransaction transaction = null,
                                              int? commandTimeout = null)
        {
            return Conn.Query<TSp>(repoQuery.Value, param, commandType: repoQuery.CmdType, transaction: transaction, commandTimeout: commandTimeout);
        }

        public void Exec(TRepoQuery repoQuery, DynamicParameters param = null, IDbTransaction transaction = null,
                                  int? commandTimeout = null)
        {
            Conn.Execute(repoQuery.Value, param, commandType: repoQuery.CmdType, transaction: transaction, commandTimeout: commandTimeout);
        }
    }
}
