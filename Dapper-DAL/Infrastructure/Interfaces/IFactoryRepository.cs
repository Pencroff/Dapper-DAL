using Dapper_DAL.General;

namespace Dapper_DAL.Infrastructure.Interfaces
{
    public interface IFactoryRepository
    {
        IRepository<T, TRepoSp> CreateRepository<T, TRepoSp>(IDapperContext context)
            where T : class
            where TRepoSp : EnumBase<TRepoSp, string>;
    }
}