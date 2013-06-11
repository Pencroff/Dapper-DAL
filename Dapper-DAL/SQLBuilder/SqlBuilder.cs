using Dapper_DAL.SQLBuilder.Interfaces;

namespace Dapper_DAL.SQLBuilder
{
    public class SqlBuilder : ISqlBuilder
    {
        private static ISqlBuilder _sqlBuilder;
        public static ISqlBuilder Current 
        {
            get { return _sqlBuilder ?? (_sqlBuilder = new SqlBuilder()); }
        }

        public ISqlBuilder SELECT(string columns = null)
        {
            return this;
        }

        public ISqlBuilder Column(string columnName, string columnAliace = null)
        {
            return this;
        }

        public ISqlBuilder FROM(string tables = null)
        {
            return this;
        }

        public ISqlBuilder Table(string tableName, string tableAliace = null)
        {
            return this;
        }

        public ISqlBuilder WHERE(string whereConditions)
        {
            return this;
        }

        public ISqlBuilder AndWhere(string fieldName, string condition, string parameterAliace = null)
        {
            return this;
        }

        public ISqlBuilder OrWhere(string fieldName, string condition, string parameterAliace = null)
        {
            return this;
        }
    }
}