using Dapper_DAL.General;

namespace Dapper_DAL.Infrastructure.EnumStoredProcedures
{
    public class EmptySpEnum : EnumBase<EmptySpEnum, string>
    {
        public EmptySpEnum(string Name, string EnumValue)
            : base(Name, EnumValue)
        {
        }
    }
}