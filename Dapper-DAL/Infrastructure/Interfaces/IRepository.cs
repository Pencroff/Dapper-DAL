using System.Collections.Generic;
using System.Data;
using Dapper;
using Dapper_DAL.General;

namespace Dapper_DAL.Infrastructure.Interfaces
{
    public interface IRepository<T, in TRepoQuery>
        where T : class
        where TRepoQuery : EnumBase<TRepoQuery, string>
    {
        IDbConnection Conn { get; }
        IDapperContext Context { get; }


        void Add(T entity, IDbTransaction transaction = null, int? commandTimeout = null);
        void Update(T entity, IDbTransaction transaction = null, int? commandTimeout = null);
        void Remove(T entity, IDbTransaction transaction = null, int? commandTimeout = null);

        T GetByKey(object key, IDbTransaction transaction = null, int? commandTimeout = null);

        IEnumerable<T> GetAll(IDbTransaction transaction = null, int? commandTimeout = null);
        IEnumerable<T> GetBy(object where = null, object order = null, IDbTransaction transaction = null, int? commandTimeout = null);

        IEnumerable<TSp> Exec<TSp>(TRepoQuery repoQuery, DynamicParameters param = null, IDbTransaction transaction = null, int? commandTimeout = null);
        void Exec(TRepoQuery repoQuery, DynamicParameters param = null, IDbTransaction transaction = null, int? commandTimeout = null);
    }
}