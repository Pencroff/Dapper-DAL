using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper_DAL.General;
using Dapper_DAL.Infrastructure.Interfaces;

namespace Dapper_DAL.Infrastructure
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly Dictionary<Type, object> _repositories;
        private readonly IFactoryRepository _factory;

        public IDapperContext Context { get; private set; }
        public IDbTransaction Transaction { get; private set; }

        public UnitOfWork(IDapperContext context, IFactoryRepository factory)
        {
            Context = context;
            _factory = factory;
            _repositories = new Dictionary<Type, object>();

            // Example of reading SqlCommandTimeOut value from web.config
            //int timeout;
            //var temp = ConfigurationManager.AppSettings["SqlCommandTimeOut"];
            //if (!int.TryParse(temp, out timeout))
            //{
            //    timeout = 30;
            //}
        }

        public IRepository<TSet, TEnumSp> GetRepository<TSet, TEnumSp>() where TSet : class where TEnumSp : EnumBase<TEnumSp, string>
        {
            if (_repositories.Keys.Contains(typeof(TSet)))
                return _repositories[typeof(TSet)] as IRepository<TSet, TEnumSp>;
            IRepository<TSet, TEnumSp> repository = _factory.CreateRepository<TSet, TEnumSp>(Context);
            _repositories.Add(typeof(TSet), repository);
            return repository;
        }

        public IDbTransaction BeginTransaction()
        {
            if (Transaction != null)
            {
                throw new NullReferenceException("Not finished previous transaction");
            }
            Transaction = Context.Connection.BeginTransaction();
            return Transaction;
        }

        public void Commit()
        {
            if (Transaction != null)
            {
                Transaction.Commit();
                Transaction.Dispose();
                Transaction = null;
            }
            else
            {
                throw new NullReferenceException("Tryed commit not opened transaction");
            }
        }

        public void Dispose()
        {
            if (Transaction != null)
            {
                Transaction.Dispose();
            }
            if (Context != null)
            {
                Context.Dispose();
            }
        }
    }
}