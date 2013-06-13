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
        
        #endregion SELECT

        #region INSERT
        
        #endregion INSERT

        #region UPDATE
        
        #endregion UPDATE

        #region DELETE
        
        #endregion DELETE

        
    }
}