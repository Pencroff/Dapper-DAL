using System.Collections.Generic;
using Dapper_DAL.Infrastructure.Enum;
using Dapper_DAL.SqlMaker.Interfaces;

namespace Dapper_DAL.SqlMaker
{
    public class QueryMaker : ISqlMaker
    {
        private enum ClauseType
        {
            Action,
            Table,
            Column,
            Value,
        }

        private static List<Clause> _clauses;
        private static List<Clause> Clauses
        {
            get {return _clauses ?? (_clauses = new List<Clause>());}
        }

        private class Clause
        {
            public static Clause New(ClauseType type, string sqlPart = null, string name = null, string aliace = null,
                                     string condition = null, string direction = null, string extra = null)
            {
                return new Clause
                    {
                        ClauseType = type,
                        SqlPart = sqlPart,
                        Name = name,
                        Aliace = aliace,
                        Condition = condition,
                        Direction = direction,
                        Extra = extra
                    };
            }

            public ClauseType ClauseType { get; private set; }
            public string SqlPart { get; private set; }
            public string Name { get; private set; }
            public string Aliace { get; private set; }
            public string Condition { get; private set; }
            public string Direction { get; private set; }
            public string Extra { get; private set; }
        }

        private static ISqlMaker _sqlMaker;

        public static ISqlMaker Current 
        {
            get { return _sqlMaker ?? (_sqlMaker = new QueryMaker()); }
        }
        public static ISqlFirst New
        {
            get
            {
                Clauses.Clear();
                _sqlMaker = new QueryMaker();
                return _sqlMaker;
            }
        }

        public string GetRaw()
        {
            throw new System.NotImplementedException();
        }

        #region SELECT
        public ISqlMakerSelect SELECT(string columns = null)
        {
            Clauses.Add(Clause.New(ClauseType.Action, "SELECT\n", extra: columns));
            return this;
        }

        public ISqlMakerSelect SelectDistinct(string columns = null)
        {
            Clauses.Add(Clause.New(ClauseType.Action, "SELECT DISTINCT\n", extra: columns));
            return this;
        }

        public ISqlMakerSelect UNION(bool IsALL = false)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect Col(string columnName, string columnAliace = null)
        {
            Clauses.Add(Clause.New(ClauseType.Column, "-", name: columnName, aliace: columnAliace));
            return this;
        }

        public ISqlMakerSelect FROM(string tables = null)
        {
            Clauses.Add(Clause.New(ClauseType.Action, "FROM\n", extra: tables));
            return this;
        }

        public ISqlMakerSelect Tab(string tableName, string tableAliace = null, string tableScheme = null)
        {
            Clauses.Add(Clause.New(ClauseType.Table, "-", name: tableName, aliace: tableAliace, extra: tableScheme));
            return this;
        }

        ISqlMakerSelect ISqlMakerSelect.WHERE(string whereConditions)
        {
            throw new System.NotImplementedException();
        }
        #endregion SELECT

        #region INSERT
        public ISqlMakerInsert INSERT(string tableName)
        {
            Clauses.Add(Clause.New(ClauseType.Action, "INSERT INTO\n", name: tableName));
            return this;
        }

        public ISqlMakerInsert Col(string columnName)
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
        #endregion INSERT

        #region UPDATE
        public ISqlMakerUpdate UPDATE(string tableName)
        {
            Clauses.Add(Clause.New(ClauseType.Action, "UPDATE\n", name: tableName));
            return this;
        }

        public ISqlMakerUpdate SET(string columnsValues = null)
        {
            Clauses.Add(Clause.New(ClauseType.Action, "SET\n", extra: columnsValues));
            return this;
        }

        public ISqlMakerUpdate Val(string columnName, string parameterAliace)
        {
            Clauses.Add(Clause.New(ClauseType.Value, "-", name: columnName, aliace: parameterAliace));
            return this;
        }

        public ISqlMakerUpdate WHERE(string whereConditions)
        {
            Clauses.Add(Clause.New(ClauseType.Action, "WHERE\n", extra: whereConditions));
            return this;
        }

        ISqlMakerUpdate ISqlMakerUpdate.WHERE(string fieldName, string condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerDelete ISqlMakerDelete.WHERE(string fieldName, Condition condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerUpdate ISqlMakerUpdate.WHERE(string fieldName, Condition condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerUpdate ISqlMakerUpdate.WhereAnd(string fieldName, string condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerDelete ISqlMakerDelete.WhereAnd(string fieldName, Condition condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerUpdate ISqlMakerUpdate.WhereAnd(string fieldName, Condition condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerUpdate ISqlMakerUpdate.WhereOr(string fieldName, string condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerDelete ISqlMakerDelete.WhereOr(string fieldName, Condition condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerUpdate ISqlMakerUpdate.WhereOr(string fieldName, Condition condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        #endregion UPDATE

        #region DELETE
        public ISqlMakerDelete DELETE(string tableName)
        {
            Clauses.Add(Clause.New(ClauseType.Action, "DELETE FROM\n", name: tableName));
            return this;
        }

        ISqlMakerDelete ISqlMakerDelete.WHERE(string whereConditions)
        {
            Clauses.Add(Clause.New(ClauseType.Action, "WHERE\n", extra: whereConditions));
            return this;
        }

        ISqlMakerDelete ISqlMakerDelete.WHERE(string fieldName, string condition, string parameterAliace)
        {
            Clauses.Add(Clause.New(ClauseType.Action, "WHERE\n", name: fieldName, condition: condition, aliace: parameterAliace));
            return this;
        }

        ISqlMakerDelete ISqlMakerDelete.WhereAnd(string fieldName, string condition, string parameterAliace)
        {
            Clauses.Add(Clause.New(ClauseType.Action, "\nAND", name: fieldName, condition: condition, aliace: parameterAliace));
            return this;
        }

        ISqlMakerDelete ISqlMakerDelete.WhereOr(string fieldName, string condition, string parameterAliace)
        {
            Clauses.Add(Clause.New(ClauseType.Action, "\nOR", name: fieldName, condition: condition, aliace: parameterAliace));
            return this;
        }
        #endregion DELETE

        ISqlMakerSelect ISqlMakerSelect.WHERE(string fieldName, string condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect WHERE(string fieldName, Condition condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerSelect ISqlMakerSelect.WhereAnd(string fieldName, string condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect WhereAnd(string fieldName, Condition condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerSelect ISqlMakerSelect.WhereOr(string fieldName, string condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect WhereOr(string fieldName, Condition condition, string parameterAliace = null)
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

        public ISqlMakerSelect ORDERBY(string columnName, SortAs direction)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect OrderByThen(string columnName, string direction)
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

        public ISqlMakerSelect HAVING(string fieldName, Condition condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect HavingAnd(string fieldName, string condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect HavingAnd(string fieldName, Condition condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect HavingOr(string fieldName, string condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect HavingOr(string fieldName, Condition condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

    }
}