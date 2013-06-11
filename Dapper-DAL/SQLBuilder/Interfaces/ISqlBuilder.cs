
namespace Dapper_DAL.SQLBuilder.Interfaces
{
    public interface ISqlBuilder
    {
        ISqlBuilder SELECT(string columns = null);
        ISqlBuilder Column(string columnName, string columnAliace = null);
        ISqlBuilder FROM(string tables = null);
        ISqlBuilder Table(string tableName, string tableAliace = null);
        ISqlBuilder WHERE(string whereConditions);
        ISqlBuilder AndWhere(string fieldName, string condition, string parameterAliace = null);
        ISqlBuilder OrWhere(string fieldName, string condition, string parameterAliace = null);
    }
}