using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using Dapper_DAL.Infrastructure;
using Dapper_DAL.Infrastructure.Enum;
using Dapper_DAL.Infrastructure.EnumQueriesStoredProcedures;
using Dapper_DAL.Infrastructure.Interfaces;
using Dapper_DAL.Models;
using Dapper_DAL.SqlMaker;

namespace Dapper_DAL
{
    public class Example
    {
        /// <summary>
        /// Example UnitOfWork property
        /// </summary>
        public IUnitOfWork UnitOfWork { get; set; }

        /// <summary>   
        /// Constructor injection
        /// </summary>
        /// <param name="unitOfWork">reference to data access layer</param>
        public Example(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        /// <summary>
        /// Example manually UnitOfWork initialization
        /// </summary>
        public void Initialization()
        {
            IDapperContext context = new DapperContext();
            IFactoryRepository repoFactory = new FactoryRepository();
            IUnitOfWork unitOfWork = new UnitOfWork(context, repoFactory);
            UnitOfWork = unitOfWork;
        }

        public void GetDataByStoredProcedure()
        {
            // Get Repository
            IRepository<Customer, CustomerEnum> repo = UnitOfWork.GetRepository<Customer, CustomerEnum>();
            // Executing stored procedure
            var param = new DynamicParameters();
            param.Add("@startIndex", 10);
            param.Add("@endIndex", 20);
            param.Add("@count", dbType: DbType.Int32, direction: ParameterDirection.Output);
            //Example for string return / out param
            //param.Add("@errorMsg", dbType: DbType.String, size: 4000, direction: ParameterDirection.ReturnValue);
            IEnumerable<Customer> customers = repo.Exec<Customer>(CustomerEnum.GetCustomerByPage, param);
            int count = param.Get<int>("@count");
        }

        public void GetDataByGetByMethod()
        {
            // Get Repository
            IRepository<Customer, CustomerEnum> repo = UnitOfWork.GetRepository<Customer, CustomerEnum>();
            // Get data with filtering and ordering
            IEnumerable<Customer> customers = 
                repo.GetBy(
                    where: new { Zip = "12345", Registered = new DateTime(year: 2013, month: 7, day: 7) }, 
                    order: new { Registered = SortAs.Desc, Name = SortAs.Asc }
                );
            // Generated SQL Query : 
            // 'SELECT * FROM Customer WHERE Zip = @Zip AND Registered = @Registered ORDER BY Registered DESC, Name ASC'
            // and setup parameters @Zip, @Registered
        }
    }
}