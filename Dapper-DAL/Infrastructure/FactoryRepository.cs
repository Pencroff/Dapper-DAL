using Dapper_DAL.General;
using Dapper_DAL.Infrastructure.Interfaces;

namespace Dapper_DAL.Infrastructure
{
    public class FactoryRepository : IFactoryRepository
    {
        public IRepository<T, TEnumSp> CreateRepository<T, TEnumSp>(IDapperContext context) where T : class where TEnumSp : EnumBase<TEnumSp, string>
        {
            IRepository<T, TEnumSp> repository;
            //if (typeof(T) == typeof(<Some type>))
            //{
            //    repository = (IRepository<T, TEnumSp>)new <SomeRepository>(context);
            //}
            //else
            {
                repository = new Repository<T, TEnumSp>(context);
            }
            return repository;
        }

    }
}