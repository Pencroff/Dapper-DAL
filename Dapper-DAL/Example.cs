using System;
using System.Collections;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using Dapper_DAL.Infrastructure;
using Dapper_DAL.Infrastructure.EnumQueriesStoredProcedures;
using Dapper_DAL.Infrastructure.Interfaces;
using Dapper_DAL.Models;

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
            IEnumerable<Customer> customers = repo.Exec<Customer>(CustomerEnum.GetCustomerByPage);
        }

        public void GetDataByGetByMethod()
        {
            // Get Repository
            IRepository<Customer, CustomerEnum> repo = UnitOfWork.GetRepository<Customer, CustomerEnum>();
            // Executing stored procedure
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