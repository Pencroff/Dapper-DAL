using Dapper_DAL.General;

namespace Dapper_DAL.Infrastructure.EnumStoredProcedures
{
    public class CustomerSpEnum : EnumBase<CustomerSpEnum, string>
    {
        public static readonly CustomerSpEnum GetCustomerByPage = new CustomerSpEnum("GetCustomerByPage", "[dbo].[spCustomerListByPageGet]");

        public CustomerSpEnum(string Name, string EnumValue)
            : base(Name, EnumValue)
        {
        }
    }
}