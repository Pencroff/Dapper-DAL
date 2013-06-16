
using Dapper_DAL.Infrastructure.Enum;

namespace Dapper_DAL.SqlMaker.Interfaces    
{
    public interface ISqlMakerBase
    {
        string RawSql();
    }
    
    public interface ISqlMakerSelect : ISqlMakerBase
    {
        ISqlMakerSelect UNION(bool IsALL = false);

        ISqlMakerSelect Col(string columnName, string columnAliace = null);
        ISqlMakerSelect FROM(string tables = null);
        ISqlMakerSelect Tab(string tableName, string tableAliace = null, string tableScheme = null);
        ISqlMakerSelect WHERE(string whereConditions);
        ISqlMakerSelect WhereAnd(string whereConditions);
        ISqlMakerSelect WhereOr(string whereConditions);
        ISqlMakerSelect JOIN(string tableName, string tableAliace = null);
        ISqlMakerSelect LeftJoin(string tableName, string tableAliace = null);
        ISqlMakerSelect RightJoin(string tableName, string tableAliace = null);
        ISqlMakerSelect FullJoin(string tableName, string tableAliace = null);
        ISqlMakerSelect ON(string condition);
        ISqlMakerSelect OnAnd(string condition);
        ISqlMakerSelect OnOr(string condition);
        ISqlMakerSelect ORDERBY(string columnName, SortAs direction);
        ISqlMakerSelect OrderThen(string columnName, SortAs direction);
        ISqlMakerSelect GROUPBY(string columnName);
        ISqlMakerSelect GroupThen(string columnName);
        ISqlMakerSelect HAVING(string havingConditions);
        ISqlMakerSelect HavingAnd(string havingConditions);
        ISqlMakerSelect HavingOr(string havingConditions);        
    }

    public interface ISqlMakerInsert : ISqlMakerBase
    {
        ISqlMakerInsert Col(string columnName);
        ISqlMakerInsert VALUES(string parameters = null);
        ISqlMakerInsert Param(string paramName);
    }
    
    public interface ISqlMakerUpdate : ISqlMakerBase
    {
        ISqlMakerUpdate SET(string columnsValues = null);
        ISqlMakerUpdate Val(string columnName, string parameterAliace);
        ISqlMakerUpdate WHERE(string whereConditions);
        ISqlMakerUpdate WhereAnd(string whereConditions);
        ISqlMakerUpdate WhereOr(string whereConditions);
    }

    public interface ISqlMakerDelete : ISqlMakerBase
    {
        ISqlMakerDelete WHERE(string whereConditions);
        ISqlMakerDelete WhereAnd(string whereConditions);
        ISqlMakerDelete WhereOr(string whereConditions);
    }

    public interface ISqlFirst
    {
        ISqlMakerSelect SELECT(string columns = null);
        ISqlMakerSelect SelectDistinct(string columns = null);
        ISqlMakerInsert INSERT(string tableName);
        ISqlMakerUpdate UPDATE(string tableName);
        ISqlMakerDelete DELETE(string tableName);
    }

    public interface ISqlMaker : ISqlFirst, ISqlMakerSelect, ISqlMakerInsert, ISqlMakerUpdate, ISqlMakerDelete
    {
    }
}