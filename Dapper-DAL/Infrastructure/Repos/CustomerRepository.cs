using Dapper_DAL.Infrastructure.EnumQueriesStoredProcedures;
using Dapper_DAL.Infrastructure.Interfaces;
using Dapper_DAL.Models;


namespace Dapper_DAL.Infrastructure.Repos
{
    public class LanguageRepository : Repository<Customer, CustomerEnum>
    {
        public LanguageRepository(IDapperContext context) : base(context)
        {
        }
    }
}