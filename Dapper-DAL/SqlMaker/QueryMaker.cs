using Dapper_DAL.Infrastructure.Enum;
using Dapper_DAL.SqlMaker.Interfaces;

namespace Dapper_DAL.SqlMaker
{
    public class QueryMaker : ISqlMaker
    {
        private static ISqlMaker _sqlMaker;
        public static ISqlMaker Current 
        {
            get { return _sqlMaker ?? (_sqlMaker = new QueryMaker()); }
        }
        public static ISqlFirst New
        {
            get
            {
                _sqlMaker = new QueryMaker();
                return _sqlMaker;
            }
        }

        public string GetRaw()
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect UNION(bool IsALL = false)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect Column(string columnName, string columnAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect FROM(string tables = null)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect Table(string tableName, string tableAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerUpdate SET(string columnsValues = null)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerUpdate Value(string columnName, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerUpdate ISqlMakerUpdate.WHERE(string whereConditions)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerDelete ISqlMakerDelete.WHERE(string fieldName, string condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerDelete ISqlMakerDelete.WhereAnd(string fieldName, string condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerDelete ISqlMakerDelete.WhereOr(string fieldName, string condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerDelete ISqlMakerDelete.WHERE(string whereConditions)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerUpdate ISqlMakerUpdate.WHERE(string fieldName, string condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerUpdate ISqlMakerUpdate.WhereAnd(string fieldName, string condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerUpdate ISqlMakerUpdate.WhereOr(string fieldName, string condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerSelect ISqlMakerSelect.WHERE(string whereConditions)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerSelect ISqlMakerSelect.WHERE(string fieldName, string condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerSelect ISqlMakerSelect.WhereAnd(string fieldName, string condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerSelect ISqlMakerSelect.WhereOr(string fieldName, string condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect JOIN(string tableName, string tableAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect InnerJoin(string tableName, string tableAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect LeftJoin(string tableName, string tableAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect RightJoin(string tableName, string tableAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect ON(string leftColumn, string rightColumn)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect OnAnd(string leftColumn, string rightColumn)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect OnOr(string leftColumn, string rightColumn)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect ORDERBY(string columnName, string direction)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect OrderByThen(string columnName, string direction)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect ORDERBY(string columnName, SortAs direction)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect OrderByThen(string columnName, SortAs direction)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect GROUPBY(string columnName)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect GroupByThen(string columnName)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect HAVING(string fieldName, string condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect HavingAnd(string fieldName, string condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect HavingOr(string fieldName, string condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerInsert Column(string columnName)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerInsert VALUES(string parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerInsert Param(string paramName)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect SELECT(string columns = null)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect SelectDistinct(string columns = null)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerInsert INSERT(string tableName)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerUpdate UPDATE(string tableName)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerDelete DELETE(string tableName)
        {
            throw new System.NotImplementedException();
        }
    }
}