using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Dapper_DAL.Infrastructure.Enum;

namespace Dapper.Contrib.Extensions
{
    public static partial class SqlMapperExtensions
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> GetQueriesAll = new ConcurrentDictionary<RuntimeTypeHandle, string>();
        private static readonly ConcurrentDictionary<int, SqlWhereOrderCache> GetQueriesWhereOrder = new ConcurrentDictionary<int, SqlWhereOrderCache>();

    #region Extra Select

        /// <summary>
        /// </summary>
        /// <typeparam name="T">Interface type to create and populate</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Entity of T</returns>
        public static IEnumerable<T> GetAll<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            string sql;
            if (!GetQueriesAll.TryGetValue(type.TypeHandle, out sql))
            {
                var name = GetTableName(type);

                // TODO: pluralizer 
                // TODO: query information schema and only select fields that are both in information schema and underlying class / interface 
                sql = string.Format("select * from {0}", name);
                GetQueriesAll[type.TypeHandle] = sql;
            }

            IEnumerable<T> obj = null;

            if (type.IsInterface)
            {
                var res = connection.Query(sql);
                if (!res.Any())
                    return (IEnumerable<T>)((object)null);
                var objList = new List<T>();
                foreach (var item in res)
                {
                    T objItem = ProxyGenerator.GetInterfaceProxy<T>();

                    foreach (var property in TypePropertiesCache(type))
                    {
                        var val = item[property.Name];
                        property.SetValue(objItem, val, null);
                    }

                    ((IProxy)objItem).IsDirty = false;   //reset change tracking and return   
                    objList.Add(objItem);
                }
                obj = objList.AsEnumerable();
            }
            else
            {
                obj = connection.Query<T>(sql, transaction: transaction, commandTimeout: commandTimeout);
            }
            return obj;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T">Interface type to create and populate</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="where"></param>
        /// <param name="order"></param>
        /// <returns>Entity of T</returns>
        public static IEnumerable<T> GetBy<T>(this IDbConnection connection, object where = null, object order = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            var isUseWhere = where != null;
            var isUseOrder = order != null;
            if (!isUseWhere && !isUseOrder)
            {
                return GetAll<T>(connection: connection, transaction: transaction, commandTimeout: commandTimeout);
            }
            var whereType = isUseWhere ? where.GetType() : null;
            var orderType = isUseOrder ? order.GetType() : null;
            SqlWhereOrderCache cache;
            var key = GetKeyTypeWhereOrder(type, whereType, orderType);
            if (!GetQueriesWhereOrder.TryGetValue(key, out cache))
            {
                cache = new SqlWhereOrderCache();
                if (isUseWhere)
                {
                    cache.Where = GetListOfNames(whereType.GetProperties());    
                }
                if (isUseOrder)
                {
                    cache.Order = GetListOfNames(orderType.GetProperties());    
                }
                var name = GetTableName(type);
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("select * from {0}", name);
                int cnt, last, i;
                if (isUseWhere)
                {
                    sb.Append(" where ");
                    cnt = cache.Where.Count();
                    last = cnt - 1;
                    for (i = 0; i < cnt; i++)
                    {
                        var prop = cache.Where.ElementAt(i);
                        sb.AppendFormat("[{0}]=@{1}", prop, prop);
                        if (i != last)
                        {
                            sb.Append(" and ");    
                        }
                            
                    }
                }
                if (isUseOrder)
                {
                    sb.Append(" order by ");
                    cnt = cache.Order.Count();
                    last = cnt - 1;
                    for (i = 0; i < cnt; i++)
                    {
                        var prop = cache.Order.ElementAt(i);
                        sb.AppendFormat("[{0}] #{1}", prop, prop);
                        if (i != last)
                        {
                            sb.Append(", ");
                        }
                    }
                }

                // TODO: pluralizer 
                // TODO: query information schema and only select fields that are both in information schema and underlying class / interface 
                cache.Sql = sb.ToString();
                GetQueriesWhereOrder[key] = cache;
            }

            IEnumerable<T> obj = null;
            var dynParms = new DynamicParameters();
            if (isUseWhere)
            {
                foreach (string name in cache.Where)
                {
                    dynParms.Add(name, whereType.GetProperty(name).GetValue(where, null));
                }
            }
            if (isUseOrder)
            {
                foreach (string name in cache.Order)
                {
                    SortAs enumVal = (SortAs)orderType.GetProperty(name).GetValue(order, null);
                    switch (enumVal)
                    {
                        case SortAs.Asc:
                            cache.Sql = cache.Sql.Replace("#" + name, "ASC");
                            break;
                        case SortAs.Desc:
                            cache.Sql = cache.Sql.Replace("#" + name, "DESC");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            if (type.IsInterface)
            {
                var res = connection.Query(cache.Sql);
                if (!res.Any())
                    return (IEnumerable<T>)((object)null);
                var objList = new List<T>();
                foreach (var item in res)
                {
                    T objItem = ProxyGenerator.GetInterfaceProxy<T>();

                    foreach (var property in TypePropertiesCache(type))
                    {
                        var val = item[property.Name];
                        property.SetValue(objItem, val, null);
                    }

                    ((IProxy)objItem).IsDirty = false;   //reset change tracking and return   
                    objList.Add(objItem);
                }
                obj = objList.AsEnumerable();
            }
            else
            {
                obj = connection.Query<T>(cache.Sql, dynParms, transaction: transaction, commandTimeout: commandTimeout);
            }
            return obj;
        }
    #endregion

        private static int GetKeyTypeWhereOrder(Type type, Type where, Type order)
        {
            var handler = type.TypeHandle;
            string whereCondition = @where != null ? @where.TypeHandle.Value.ToString() : string.Empty;
            string orderCondition = order != null ? order.TypeHandle.Value.ToString() : string.Empty; ;
            var str = string.Format("{0}{1}{2}", handler.Value, whereCondition, orderCondition);
            return str.GetHashCode();
        }

        private static IEnumerable<string> GetListOfNames(PropertyInfo[] list)
        {
            List<string> lst = new List<string>();
            foreach (PropertyInfo info in list)
            {
                lst.Add(info.Name);
            }
            return lst.AsEnumerable();
        }
    }

    public class SqlWhereOrderCache
    {
        public string Sql { get; set; }
        public IEnumerable<string> Where { get; set; }
        public IEnumerable<string> Order { get; set; }
    }
}
