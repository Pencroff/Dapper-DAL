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
            ActionUpdateSet,
            ActionUpdateValues,
            ActionUpdateWhere,
            ActionSelect,
            ActionSelectWhereOnHaving,
            ActionSelectJoin,
            ActionSelectOrder,
            ActionSelectGroup,
            ActionDelete,
            ActionDeleteWhere,
            Table,
            Column,
            Parameter,
        }

        private static string br = "\n";
        private static string brIndent = "\n\t";
        private static string brIndentX2 = "\n\t\t";

        private static string _dbScheme;

        public static string DbScheme { get { return _dbScheme; } set { _dbScheme = value; } }

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
            if (!string.IsNullOrEmpty(dbScheme))
            {
                _dbScheme = dbScheme;   
            }
            Clauses.Clear();
            _sqlMaker = new QueryMaker();
            return _sqlMaker;
        }

        #region Common Method
        private static string FormatScheme(string globalScheme, string currentScheme = null)
        {
            if (!string.IsNullOrEmpty(currentScheme))
            {
                return string.Format("[{0}].", currentScheme.Trim());
            }
            if (!string.IsNullOrEmpty(globalScheme))
            {
                return string.Format("[{0}].", globalScheme.Trim());
            }
            return string.Empty;
        }
        private static string FormatAliace(string aliace)
        {
            if (!string.IsNullOrEmpty(aliace))
            {
                return string.Format(" AS {0}", aliace.Trim());
            }
            return string.Empty;
        }
        private static string FormatTableNameWithShema(string scheme, string tableName)
        {
            return string.Format("{0}[{1}]", scheme, tableName.Trim());
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
        private static string FormatParameter(string paramName)
        {
            return paramName.Contains("@") ? paramName.Trim() : "@" + paramName.Trim();
        }
        #endregion

        #region Resolve Sql Query
        private static string ResolveInsert(IEnumerable<Clause> list, string dbScheme)
        {
            var sb = new StringBuilder();
            var sqlScheme = FormatScheme(dbScheme);
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
                        sb.Append(" ");
                        sb.Append(FormatTableNameWithShema(sqlScheme, clause.Name));
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
                        var paramName = FormatParameter(clause.Name);
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

        private static string ResolveUpdate(List<Clause> list, string dbScheme)
        {
            var sb = new StringBuilder();
            var sqlScheme = FormatScheme(dbScheme);
            var isSetParam = false;
            foreach (var clause in list)
            {
                switch (clause.ClauseType)
                {
                    case ClauseType.ActionUpdate:
                        sb.Append(clause.SqlPart);
                        sb.Append(" ");
                        sb.Append(FormatTableNameWithShema(sqlScheme, clause.Name));
                        break;
                    case ClauseType.ActionUpdateSet:
                        sb.Append(br);
                        sb.Append(clause.SqlPart);
                        if (!string.IsNullOrEmpty(clause.Extra))
                        {
                            isSetParam = true;
                            sb.Append(ResolveStringToRows(clause.Extra, brIndent));
                        }
                        break;
                    case ClauseType.ActionUpdateValues:
                        if (!isSetParam)
                        {
                            isSetParam = true;
                            sb.Append(brIndent + clause.Name.Trim());
                            sb.Append(" = ");
                            sb.Append(FormatParameter(clause.Aliace));
                        }
                        else
                        {
                            sb.Append(brIndent + ", " + clause.Name.Trim());
                            sb.Append(" = ");
                            sb.Append(FormatParameter(clause.Aliace));
                        }
                        break;
                    case ClauseType.ActionUpdateWhere:
                        sb.Append(clause.SqlPart);
                        sb.Append(clause.Condition.Trim());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            sb.Append(";");
            return sb.ToString();
        }

        private static string ResolveSelect(List<Clause> list, string dbScheme)
        {
            var sb = new StringBuilder();
            bool isFirst = true;
            foreach (var clause in list)
            {
                switch (clause.ClauseType)
                {
                    case ClauseType.ActionSelect:
                        isFirst = true; // SELECT or UNION
                        sb.Append(clause.SqlPart);
                        if (!string.IsNullOrEmpty(clause.Extra))
                        {
                            isFirst = false;
                            sb.Append(ResolveStringToRows(clause.Extra, brIndent));
                        }
                        break;
                    case ClauseType.ActionSelectWhereOnHaving:
                        sb.Append(clause.SqlPart);
                        sb.Append(clause.Condition.Trim());
                        break;
                    case ClauseType.ActionSelectJoin:
                        var schemeJoin = FormatScheme(dbScheme, clause.Extra);
                        var tabNameJoin = FormatTableNameWithShema(schemeJoin, clause.Name);
                        var tabAliaceJoin = FormatAliace(clause.Aliace);
                        sb.Append(clause.SqlPart);
                        sb.Append(tabNameJoin + tabAliaceJoin);
                        break;
                    case ClauseType.ActionSelectOrder:
                        sb.Append(clause.SqlPart);
                        sb.Append(clause.Name.Trim());
                        sb.Append(" ");
                        sb.Append(clause.Direction);
                        break;
                    case ClauseType.ActionSelectGroup:
                        sb.Append(clause.SqlPart);
                        sb.Append(clause.Name.Trim());
                        break;
                    case ClauseType.Table:
                        var scheme = FormatScheme(dbScheme, clause.Extra);
                        var tabName = FormatTableNameWithShema(scheme, clause.Name);
                        var tabAliace = FormatAliace(clause.Aliace);
                        if (isFirst)
                        {
                            isFirst = false;
                            sb.Append(brIndent + tabName + tabAliace);
                        }
                        else
                        {
                            sb.Append(brIndent + ", " + tabName + tabAliace);
                        }
                        break;
                    case ClauseType.Column:
                        var aliace = string.IsNullOrEmpty(clause.Aliace) ? string.Empty : " AS " + clause.Aliace.Trim();
                        if (isFirst)
                        {
                            isFirst = false;
                            sb.Append(brIndent + clause.Name.Trim() + aliace);
                        }
                        else
                        {
                            sb.Append(brIndent + ", " + clause.Name.Trim() + aliace);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            sb.Append(";");
            return sb.ToString();
        }

        private static string ResolveDelete(List<Clause> list, string dbScheme)
        {
            var sb = new StringBuilder();
            var sqlScheme = FormatScheme(dbScheme);
            foreach (var clause in list)
            {
                switch (clause.ClauseType)
                {
                    case ClauseType.ActionDelete:
                        sb.Append(clause.SqlPart);
                        sb.Append(" ");
                        sb.Append(FormatTableNameWithShema(sqlScheme, clause.Name));
                        break;
                    case ClauseType.ActionDeleteWhere:
                        sb.Append(clause.SqlPart);
                        sb.Append(clause.Condition.Trim());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            sb.Append(";");
            return sb.ToString();
        }
        #endregion Resolve Sql Query

        public virtual string RawSql()
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
                    sqlResult = ResolveUpdate(Clauses, _dbScheme);
                    break;
                case ClauseType.ActionSelect:
                    sqlResult = ResolveSelect(Clauses, _dbScheme);
                    break;
                case ClauseType.ActionDelete:
                    sqlResult = ResolveDelete(Clauses, _dbScheme);
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
            var sqlPart = br + "UNION" + (IsALL ? " ALL" : string.Empty) + br + "SELECT";
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
            Clauses.Add(Clause.New(ClauseType.ActionSelect, br + "FROM", extra: tables));
            return this;
        }

        public virtual ISqlMakerSelect Tab(string tableName, string tableAliace = null, string tableScheme = null)
        {
            Clauses.Add(Clause.New(ClauseType.Table, name: tableName, aliace: tableAliace, extra: tableScheme));
            return this;
        }

        ISqlMakerSelect ISqlMakerSelect.WHERE(string whereConditions)
        {
            Clauses.Add(Clause.New(ClauseType.ActionSelectWhereOnHaving, br + "WHERE" + brIndent, condition: whereConditions));
            return this;
        }

        ISqlMakerSelect ISqlMakerSelect.WhereAnd(string whereConditions)
        {
            Clauses.Add(Clause.New(ClauseType.ActionSelectWhereOnHaving, brIndent + "AND ", condition: whereConditions));
            return this;
        }

        ISqlMakerSelect ISqlMakerSelect.WhereOr(string whereConditions)
        {
            Clauses.Add(Clause.New(ClauseType.ActionSelectWhereOnHaving, brIndent + "OR ", condition: whereConditions));
            return this;
        }

        public virtual ISqlMakerSelect JOIN(string tableName, string tableAliace = null)
        {
            Clauses.Add(Clause.New(ClauseType.ActionSelectJoin, br + "INNER JOIN ", name: tableName, aliace: tableAliace));
            return this;
        }

        public virtual ISqlMakerSelect LeftJoin(string tableName, string tableAliace = null)
        {
            Clauses.Add(Clause.New(ClauseType.ActionSelectJoin, br + "LEFT JOIN ", name: tableName, aliace: tableAliace));
            return this;
        }

        public virtual ISqlMakerSelect RightJoin(string tableName, string tableAliace = null)
        {
            Clauses.Add(Clause.New(ClauseType.ActionSelectJoin, br + "RIGHT JOIN ", name: tableName, aliace: tableAliace));
            return this;
        }

        public virtual ISqlMakerSelect FullJoin(string tableName, string tableAliace = null)
        {
            Clauses.Add(Clause.New(ClauseType.ActionSelectJoin, br + "FULL JOIN ", name: tableName, aliace: tableAliace));
            return this;
        }

        public virtual ISqlMakerSelect ON(string condition)
        {
            Clauses.Add(Clause.New(ClauseType.ActionSelectWhereOnHaving, brIndent + "ON ", condition: condition));
            return this;
        }

        public virtual ISqlMakerSelect OnAnd(string condition)
        {
            Clauses.Add(Clause.New(ClauseType.ActionSelectWhereOnHaving, brIndent + "AND ", condition: condition));
            return this;
        }

        public virtual ISqlMakerSelect OnOr(string condition)
        {
            Clauses.Add(Clause.New(ClauseType.ActionSelectWhereOnHaving, brIndent + "OR ", condition: condition));
            return this;
        }

        public virtual ISqlMakerSelect ORDERBY(string columnName, SortAs direction)
        {
            Clauses.Add(Clause.New(ClauseType.ActionSelectOrder, br + "ORDER BY ", name: columnName, direction: direction.ToString().ToUpperInvariant()));
            return this;
        }

        public virtual ISqlMakerSelect OrderThen(string columnName, SortAs direction)
        {
            Clauses.Add(Clause.New(ClauseType.ActionSelectOrder, ", ", name: columnName, direction: direction.ToString().ToUpperInvariant()));
            return this;
        }

        public virtual ISqlMakerSelect GROUPBY(string columnName)
        {
            Clauses.Add(Clause.New(ClauseType.ActionSelectGroup, br + "GROUP BY ", name: columnName));
            return this;
        }

        public virtual ISqlMakerSelect GroupThen(string columnName)
        {
            Clauses.Add(Clause.New(ClauseType.ActionSelectGroup, ", ", name: columnName));
            return this;
        }

        public virtual ISqlMakerSelect HAVING(string havingConditions)
        {
            Clauses.Add(Clause.New(ClauseType.ActionSelectWhereOnHaving, br + "HAVING ", condition: havingConditions));
            return this;
        }

        public virtual ISqlMakerSelect HavingAnd(string havingConditions)
        {
            Clauses.Add(Clause.New(ClauseType.ActionSelectWhereOnHaving, brIndent + "AND ", condition: havingConditions));
            return this;
        }

        public virtual ISqlMakerSelect HavingOr(string havingConditions)
        {
            Clauses.Add(Clause.New(ClauseType.ActionSelectWhereOnHaving, brIndent + "OR ", condition: havingConditions));
            return this;
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
            Clauses.Add(Clause.New(ClauseType.ActionUpdate, "UPDATE", name: tableName));
            return this;
        }

        public virtual ISqlMakerUpdate SET(string columnsValues = null)
        {
            Clauses.Add(Clause.New(ClauseType.ActionUpdateSet, "SET", extra: columnsValues));
            return this;
        }

        public virtual ISqlMakerUpdate Val(string columnName, string parameterAliace)
        {
            Clauses.Add(Clause.New(ClauseType.ActionUpdateValues, name: columnName, aliace: parameterAliace));
            return this;
        }

        ISqlMakerUpdate ISqlMakerUpdate.WHERE(string whereConditions)
        {
            Clauses.Add(Clause.New(ClauseType.ActionUpdateWhere, br + "WHERE ", condition: whereConditions));
            return this;
        }

        ISqlMakerUpdate ISqlMakerUpdate.WhereAnd(string whereConditions)
        {
            Clauses.Add(Clause.New(ClauseType.ActionUpdateWhere, brIndent + "AND ", condition: whereConditions));
            return this;
        }

        ISqlMakerUpdate ISqlMakerUpdate.WhereOr(string whereConditions)
        {
            Clauses.Add(Clause.New(ClauseType.ActionUpdateWhere, brIndent + "OR ", condition: whereConditions));
            return this;
        }
        #endregion UPDATE

        #region DELETE
        public virtual ISqlMakerDelete DELETE(string tableName)
        {
            Clauses.Add(Clause.New(ClauseType.ActionDelete, "DELETE FROM", name: tableName));
            return this;
        }

        public virtual ISqlMakerDelete WHERE(string whereConditions)
        {
            Clauses.Add(Clause.New(ClauseType.ActionDeleteWhere, br + "WHERE ", condition: whereConditions));
            return this;
        }

        public virtual ISqlMakerDelete WhereAnd(string whereConditions)
        {
            Clauses.Add(Clause.New(ClauseType.ActionDeleteWhere, brIndent + "AND ", condition: whereConditions));
            return this;
        }

        public virtual ISqlMakerDelete WhereOr(string whereConditions)
        {
            Clauses.Add(Clause.New(ClauseType.ActionDeleteWhere, brIndent + "OR ", condition: whereConditions));
            return this;
        }
        #endregion DELETE
    }
}
