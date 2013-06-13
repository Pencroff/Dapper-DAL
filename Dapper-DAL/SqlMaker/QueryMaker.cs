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
            //Clauses.Add(Clause.New(ClauseType.Action, "SELECT\n", extra: columns));
            //return this;
            //Clauses.Add(Clause.New(ClauseType.Column, "-", name: columnName, aliace: columnAliace));
            //return this;
            //Clauses.Add(Clause.New(ClauseType.Action, "WHERE\n", name: fieldName, condition: condition, aliace: parameterAliace));
            //return this;
        }

        #region SELECT
        public ISqlMakerSelect SELECT(string columns = null)
        {
            throw new System.NotImplementedException();
        }

        public ISqlMakerSelect SelectDistinct(string columns = null)
        {
            throw new System.NotImplementedException();
        }
        #endregion SELECT

        #region INSERT
        public ISqlMakerInsert INSERT(string tableName)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }


        #endregion UPDATE

        #region DELETE
        public ISqlMakerDelete DELETE(string tableName)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerUpdate SET(string columnsValues = null)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerUpdate Val(string columnName, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerUpdate WHERE(string whereConditions)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerUpdate WHERE(string fieldName, string condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerUpdate WHERE(string fieldName, Condition condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerUpdate WhereAnd(string fieldName, string condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerUpdate WhereAnd(string fieldName, Condition condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerUpdate WhereOr(string fieldName, string condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerUpdate WhereOr(string fieldName, Condition condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerDelete ISqlMakerDelete.WHERE(string whereConditions)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerDelete ISqlMakerDelete.WHERE(string fieldName, string condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerDelete ISqlMakerDelete.WHERE(string fieldName, Condition condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerDelete ISqlMakerDelete.WhereAnd(string fieldName, string condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerDelete ISqlMakerDelete.WhereAnd(string fieldName, Condition condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerDelete ISqlMakerDelete.WhereOr(string fieldName, string condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerDelete ISqlMakerDelete.WhereOr(string fieldName, Condition condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }
        #endregion DELETE

        
    }
}