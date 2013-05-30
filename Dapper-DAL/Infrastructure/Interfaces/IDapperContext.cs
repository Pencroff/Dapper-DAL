using System;
using System.Data;

namespace Dapper_DAL.Infrastructure.Interfaces
{
    public interface IDapperContext : IDisposable
    {
        IDbConnection Connection { get; }
    }
}