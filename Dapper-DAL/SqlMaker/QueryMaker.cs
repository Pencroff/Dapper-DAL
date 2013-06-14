using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper_DAL.Infrastructure.Enum;
using Dapper_DAL.SqlMaker.Interfaces;

namespace Dapper_DAL.SqlMaker
{
    public class QueryMaker : ISqlMaker
    {
        private enum ClauseType
        {
            ActionInsert,
            ActionInsertValues,
            ActionUpdate,
            ActionSelect,
            ActionDelete,
            Table,
            Column,
            Value, //Col = <valParam>
            Parameter, //<param>
            Condition, //col<condition><param>
        }

        private static string brIndent = "\n\t";
        private static string brIndentX2 = "\n\t\t";
        private static string brIndentX3 = "\n\t\t\t";

        private static string _dbScheme;
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
        public static ISqlFirst New (string dbScheme = null)
        {
            _dbScheme = dbScheme;
            Clauses.Clear();
            _sqlMaker = new QueryMaker();
            return _sqlMaker;
        }

        #region Common Method
        private static string TableNameWithShema(string scheme, string tableName)
        {
            return string.Format("{0}[{1}]", scheme, tableName);
        }
        private static string ResolveStringToRows(string extra, string indent)
        {
            var sb = new StringBuilder();
            var delimiters = new char[] { ',', ';' };
            var array = extra.Split(delimiters);
            var firstParam = true;
            foreach (var s in array)
            {
                if (firstParam)
                {
                    sb.Append(indent + s.Trim());
                    firstParam = false;
                }
                else
                {
                    sb.Append(indent + ", " + s.Trim());
                }
            }
            return sb.ToString();
        }
        #endregion

        #region Resolve Sql Query
        private static string ResolveInsert(IEnumerable<Clause> list, string dbScheme)
        {
            var sb = new StringBuilder();
            var sqlScheme = dbScheme != null ? string.Format("[{0}].", dbScheme) : string.Empty;
            var isFirstCol = true;
            var lastBkt = false;
            var insertedParams = false;
            var colCount = list.Count(i => i.ClauseType == ClauseType.Column);
            var count = 0;
            foreach (var clause in list)
            {
                switch (clause.ClauseType)
                {
                    case ClauseType.ActionInsert:
                        sb.Append(clause.SqlPart);
                        sb.Append(brIndent);
                        sb.Append(TableNameWithShema(sqlScheme, clause.Name));
                        break;
                    case ClauseType.Column:
                        count += 1;
                        if (isFirstCol)
                        {
                            sb.Append(" (");
                        }
                        if (isFirstCol)
                        {
                            sb.Append(brIndentX2 + "[" + clause.Name + "]");
                            isFirstCol = false;
                        }
                        else
                        {
                            sb.Append(brIndentX2 + ", [" + clause.Name + "]");
                        }
                        if (count == colCount)
                        {
                            sb.Append(brIndent + ")");
                        }
                        break;
                    case ClauseType.ActionInsertValues:
                        lastBkt = true;
                        sb.Append(brIndent);
                        sb.Append(clause.SqlPart);
                        sb.Append(" (");
                        if (!string.IsNullOrEmpty(clause.Extra))
                        {
                            insertedParams = true;
                            sb.Append(ResolveStringToRows(clause.Extra, brIndentX2));
                        }
                        break;
                    case ClauseType.Parameter:
                        var paramName = clause.Name.Contains("@") ? clause.Name.Trim() : "@" + clause.Name.Trim();
                        if (!insertedParams)
                        {
                            insertedParams = true;
                            sb.Append(brIndentX2 + paramName);
                        }
                        else
                        {
                            sb.Append(brIndentX2 + ", " + paramName);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Wrong clause type in Insert resolving method");
                }
            }
            if (lastBkt)
            {
                sb.Append(brIndent + ");");
            }
            return sb.ToString();
        }

        private static string ResolveSelect(List<Clause> list, string dbScheme)
        {
            var sb = new StringBuilder();
            var sqlScheme = dbScheme != null ? string.Format("[{0}].", dbScheme) : string.Empty;
            bool isFirstCol = true;
            foreach (var clause in list)
            {
                switch (clause.ClauseType)
                {
                    case ClauseType.ActionSelect:
                        isFirstCol = true; // SELECT or UNION
                        sb.Append(clause.SqlPart);
                        if (!string.IsNullOrEmpty(clause.Extra))
                        {
                            isFirstCol = false;
                            sb.Append(ResolveStringToRows(clause.Extra, brIndent));
                        }
                        break;
                    case ClauseType.Table:
                        break;
                    case ClauseType.Column:
                        var aliace = string.IsNullOrEmpty(clause.Aliace) ? string.Empty : " AS " + clause.Aliace.Trim();
                        if (isFirstCol)
                        {
                            isFirstCol = false;
                            sb.Append(brIndent + clause.Name.Trim() + aliace);
                        }
                        else
                        {
                            sb.Append(brIndent + ", " + clause.Name.Trim() + aliace);
                        }
                        break;
                    case ClauseType.Condition:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return sb.ToString();
        }
        #endregion Resolve Sql Query
        
        public string GetRaw()
        {
            string sqlResult = null;
            if (Clauses.Count == 0)
            {
                throw new Exception("Empty query");
            }
            var first = Clauses.First();
            switch (first.ClauseType)
            {
                case ClauseType.ActionInsert:
                    sqlResult = ResolveInsert(Clauses, _dbScheme);
                    break;
                case ClauseType.ActionUpdate:
                    break;
                case ClauseType.ActionSelect:
                    sqlResult = ResolveSelect(Clauses, _dbScheme);
                    break;
                case ClauseType.ActionDelete:
                    break;
                default:
                    throw new Exception("Wrong start of query");
            }
            return sqlResult;
        }
        
        #region SELECT
        public virtual ISqlMakerSelect SELECT(string columns = null)
        {
            Clauses.Add(Clause.New(ClauseType.ActionSelect, "SELECT", extra: columns));
            return this;
        }

        public virtual ISqlMakerSelect SelectDistinct(string columns = null)
        {
            Clauses.Add(Clause.New(ClauseType.ActionSelect, "SELECT DISTINCT", extra: columns));
            return this;
        }

        public virtual ISqlMakerSelect UNION(bool IsALL = false)
        {
            var sqlPart = "\nUNION" + (IsALL ? " ALL" : string.Empty) + "\nSELECT";
            Clauses.Add(Clause.New(ClauseType.ActionSelect, sqlPart));
            return this;
        }

        public virtual ISqlMakerSelect Col(string columnName, string columnAliace = null)
        {
            Clauses.Add(Clause.New(ClauseType.Column, name: columnName, aliace: columnAliace));
            return this;
        }

        public virtual ISqlMakerSelect FROM(string tables = null)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerSelect Tab(string tableName, string tableAliace = null, string tableScheme = null)
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

        ISqlMakerSelect ISqlMakerSelect.WHERE(string fieldName, Condition condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerSelect ISqlMakerSelect.WhereAnd(string fieldName, string condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerSelect ISqlMakerSelect.WhereAnd(string fieldName, Condition condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerSelect ISqlMakerSelect.WhereOr(string fieldName, string condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerSelect ISqlMakerSelect.WhereOr(string fieldName, Condition condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerSelect JOIN(string tableName, string tableAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerSelect InnerJoin(string tableName, string tableAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerSelect LeftJoin(string tableName, string tableAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerSelect RightJoin(string tableName, string tableAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerSelect ON(string leftColumn, string rightColumn)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerSelect OnAnd(string leftColumn, string rightColumn)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerSelect OnOr(string leftColumn, string rightColumn)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerSelect ORDERBY(string columnName, string direction)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerSelect ORDERBY(string columnName, SortAs direction)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerSelect OrderByThen(string columnName, string direction)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerSelect OrderByThen(string columnName, SortAs direction)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerSelect GROUPBY(string columnName)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerSelect GroupByThen(string columnName)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerSelect HAVING(string fieldName, string condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerSelect HAVING(string fieldName, Condition condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerSelect HavingAnd(string fieldName, string condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerSelect HavingAnd(string fieldName, Condition condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerSelect HavingOr(string fieldName, string condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerSelect HavingOr(string fieldName, Condition condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }
        #endregion SELECT

        #region INSERT
        public virtual ISqlMakerInsert INSERT(string tableName)
        {
            Clauses.Add(Clause.New(ClauseType.ActionInsert, "INSERT INTO", name: tableName));
            return this;
        }
        public virtual ISqlMakerInsert Col(string columnName)
        {
            Clauses.Add(Clause.New(ClauseType.Column, name: columnName));
            return this;
        }

        public virtual ISqlMakerInsert VALUES(string parameters = null)
        {
            Clauses.Add(Clause.New(ClauseType.ActionInsertValues, "VALUES", extra: parameters));
            return this;
        }

        public virtual ISqlMakerInsert Param(string paramName)
        {
            Clauses.Add(Clause.New(ClauseType.Parameter, name: paramName));
            return this;
        }
        #endregion INSERT

        #region UPDATE
        public virtual ISqlMakerUpdate UPDATE(string tableName)
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

        ISqlMakerUpdate ISqlMakerUpdate.WHERE(string whereConditions)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerUpdate ISqlMakerUpdate.WHERE(string fieldName, string condition, string parameterAliace)
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

        ISqlMakerUpdate ISqlMakerUpdate.WhereAnd(string fieldName, Condition condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerUpdate ISqlMakerUpdate.WhereOr(string fieldName, string condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }

        ISqlMakerUpdate ISqlMakerUpdate.WhereOr(string fieldName, Condition condition, string parameterAliace)
        {
            throw new System.NotImplementedException();
        }
        #endregion UPDATE

        #region DELETE
        public virtual ISqlMakerDelete DELETE(string tableName)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerDelete WHERE(string whereConditions)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerDelete WHERE(string fieldName, string condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerDelete WHERE(string fieldName, Condition condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerDelete WhereAnd(string fieldName, string condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerDelete WhereAnd(string fieldName, Condition condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerDelete WhereOr(string fieldName, string condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }

        public virtual ISqlMakerDelete WhereOr(string fieldName, Condition condition, string parameterAliace = null)
        {
            throw new System.NotImplementedException();
        }
        #endregion DELETE
    }
}