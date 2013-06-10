using System.Data;
using Dapper_DAL.General;

namespace Dapper_DAL.Infrastructure.EnumQueriesStoredProcedures
{
    public sealed class EmptyEnum : EnumBase<EmptyEnum, string>
    {
        public EmptyEnum(string Name, string EnumValue, CommandType? cmdType)
            : base(Name, EnumValue, cmdType)
        {
        }
    }
}