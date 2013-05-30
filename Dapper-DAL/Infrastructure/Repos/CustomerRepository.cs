using DapperDAL.Models;
using Dapper_DAL.Infrastructure.EnumStoredProcedures;
using Dapper_DAL.Infrastructure.Interfaces;


namespace Dapper_DAL.Infrastructure.Repos
{
    public class LanguageRepository : Repository<Customer, CustomerSpEnum>
    {
        public LanguageRepository(IDapperContext context) : base(context)
        {
        }
    }
}