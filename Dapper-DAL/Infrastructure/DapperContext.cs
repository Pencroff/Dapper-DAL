using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper_DAL.Infrastructure.Interfaces;
using StackExchange.Profiling;

namespace Dapper_DAL.Infrastructure
{
    public class DapperContext : IDapperContext
    {
        private readonly string _connectionStringName;
        private readonly string _connectionString;
        private bool _useMiniProfiler;
        private IDbConnection _connection;

        public DapperContext()
        {
            var temp = ConfigurationManager.AppSettings["UseMiniProfilerForSql"];
            if (!bool.TryParse(temp, out _useMiniProfiler))
            {
                _useMiniProfiler = false;
            }
            _connectionStringName = ConfigurationManager.AppSettings["UsedConnectionString"];
            _connectionString = ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString; ;
        }

        public IDbConnection Connection 
        {
            get
            {
                if (_connection == null)
                {
                    if (_useMiniProfiler)
                    {
                        _connection = new StackExchange.Profiling.Data.ProfiledDbConnection(new SqlConnection(_connectionString), MiniProfiler.Current);
                    }
                    else
                    {
                        _connection = new SqlConnection(_connectionString);    
                    }
                }
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }
                return _connection;
            } 
        }
        public void Dispose()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
                _connection.Close();
        }
    }
}