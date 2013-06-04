using Dapper_DAL.General;

namespace Dapper_DAL.Infrastructure.EnumQueriesStoredProcedures
{
    public class CustomerEnum : EnumBase<CustomerEnum, string>
    {
        public static readonly CustomerEnum GetCustomerByPage = new CustomerEnum("GetCustomerByPage", "[dbo].[spCustomerListByPageGet]");

        public CustomerEnum(string Name, string EnumValue)
            : base(Name, EnumValue)
        {
        }
    }
}