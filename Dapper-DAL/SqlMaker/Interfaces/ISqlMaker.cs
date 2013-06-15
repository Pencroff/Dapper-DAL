
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
        //ISqlMakerSelect WHERE(string fieldName, Condition condition, string parameterAliace = null);
        ISqlMakerSelect WhereAnd(string whereConditions);
        //ISqlMakerSelect WhereAnd(string fieldName, Condition condition, string parameterAliace = null);
        ISqlMakerSelect WhereOr(string whereConditions);
        //ISqlMakerSelect WhereOr(string fieldName, Condition condition, string parameterAliace = null);
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
        //ISqlMakerSelect HAVING(string fieldName, Condition condition, string parameterAliace = null);
        ISqlMakerSelect HavingAnd(string havingConditions);
        //ISqlMakerSelect HavingAnd(string fieldName, Condition condition, string parameterAliace = null);
        ISqlMakerSelect HavingOr(string havingConditions);
        //ISqlMakerSelect HavingOr(string fieldName, Condition condition, string parameterAliace = null);
    }

    //[ WITH <common_table_expression> [ ,...n ] ]
    //INSERT 
    //{
    //        [ TOP ( expression ) [ PERCENT ] ] 
    //        [ INTO ] 
    //        { <object> | rowset_function_limited 
    //          [ WITH ( <Table_Hint_Limited> [ ...n ] ) ]
    //        }
    //    {
    //        [ ( column_list ) ] 
    //        [ <OUTPUT Clause> ]
    //        { VALUES ( { DEFAULT | NULL | expression } [ ,...n ] ) [ ,...n     ] 
    //        | derived_table 
    //        | execute_statement
    //        | <dml_table_source>
    //        | DEFAULT VALUES 
    //        }
    //    }
    //}
    //[;]
    public interface ISqlMakerInsert : ISqlMakerBase
    {
        ISqlMakerInsert Col(string columnName);
        ISqlMakerInsert VALUES(string parameters = null);
        ISqlMakerInsert Param(string paramName);
    }
    
    //[ WITH <common_table_expression> [...n] ]
    //UPDATE 
    //    [ TOP ( expression ) [ PERCENT ] ] 
    //    { { table_alias | <object> | rowset_function_limited 
    //         [ WITH ( <Table_Hint_Limited> [ ...n ] ) ]
    //      }
    //      | @table_variable    
    //    }
    //    SET
    //        { column_name = { expression | DEFAULT | NULL }
    //          | { udt_column_name.{ { property_name = expression
    //                                | field_name = expression }
    //                                | method_name ( argument [ ,...n ] )
    //                              }
    //          }
    //          | column_name { .WRITE ( expression , @Offset , @Length ) }
    //          | @variable = expression
    //          | @variable = column = expression
    //          | column_name { += | -= | *= | /= | %= | &= | ^= | |= } expression
    //          | @variable { += | -= | *= | /= | %= | &= | ^= | |= } expression
    //          | @variable = column { += | -= | *= | /= | %= | &= | ^= | |= } expression
    //        } [ ,...n ] 

    //    [ <OUTPUT Clause> ]
    //    [ FROM{ <table_source> } [ ,...n ] ] 
    //    [ WHERE { <search_condition> 
    //            | { [ CURRENT OF 
    //                  { { [ GLOBAL ] cursor_name } 
    //                      | cursor_variable_name 
    //                  } 
    //                ]
    //              }
    //            } 
    //    ] 
    //    [ OPTION ( <query_hint> [ ,...n ] ) ]
    //[ ; ]
    public interface ISqlMakerUpdate : ISqlMakerBase
    {
        ISqlMakerUpdate SET(string columnsValues = null);
        ISqlMakerUpdate Val(string columnName, string parameterAliace);
        ISqlMakerUpdate WHERE(string whereConditions);
        //ISqlMakerUpdate WHERE(string fieldName, Condition condition, string parameterAliace = null);
        ISqlMakerUpdate WhereAnd(string whereConditions);
        //ISqlMakerUpdate WhereAnd(string fieldName, Condition condition, string parameterAliace = null);
        ISqlMakerUpdate WhereOr(string whereConditions);
        //ISqlMakerUpdate WhereOr(string fieldName, Condition condition, string parameterAliace = null);
    }

    //[ WITH <common_table_expression> [ ,...n ] ]
    //DELETE 
    //    [ TOP ( expression ) [ PERCENT ] ] 
    //    [ FROM ] 
    //    { { table_alias
    //      | <object> 
    //      | rowset_function_limited 
    //      [ WITH ( table_hint_limited [ ...n ] ) ] } 
    //      | @table_variable
    //    }
    //    [ <OUTPUT Clause> ]
    //    [ FROM table_source [ ,...n ] ] 
    //    [ WHERE { <search_condition> 
    //            | { [ CURRENT OF 
    //                   { { [ GLOBAL ] cursor_name } 
    //                       | cursor_variable_name 
    //                   } 
    //                ]
    //              }
    //            } 
    //    ] 
    //    [ OPTION ( <Query Hint> [ ,...n ] ) ] 
    //[; ]
    public interface ISqlMakerDelete : ISqlMakerBase
    {
        ISqlMakerDelete WHERE(string whereConditions);
        //ISqlMakerDelete WHERE(string fieldName, Condition condition, string parameterAliace = null);
        ISqlMakerDelete WhereAnd(string whereConditions);
        //ISqlMakerDelete WhereAnd(string fieldName, Condition condition, string parameterAliace = null);
        ISqlMakerDelete WhereOr(string whereConditions);
        //ISqlMakerDelete WhereOr(string fieldName, Condition condition, string parameterAliace = null);
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