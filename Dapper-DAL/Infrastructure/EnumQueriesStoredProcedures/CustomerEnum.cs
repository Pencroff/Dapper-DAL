using System.Data;
using Dapper_DAL.General;

namespace Dapper_DAL.Infrastructure.EnumQueriesStoredProcedures
{
    public class CustomerEnum : EnumBase<CustomerEnum, string>
    {
        public static readonly CustomerEnum GetCustomerByPage = new CustomerEnum("GetCustomerByPage", "[dbo].[spCustomerListByPageGet]", CommandType.StoredProcedure);

        public CustomerEnum(string Name, string EnumValue, CommandType? cmdType)
            : base(Name, EnumValue, cmdType)
        {
        }
    }
}