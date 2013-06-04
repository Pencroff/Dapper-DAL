using Dapper_DAL.Infrastructure;
using Dapper_DAL.Infrastructure.EnumQueriesStoredProcedures;
using Dapper_DAL.Infrastructure.Interfaces;
using Dapper_DAL.Models;

namespace Dapper_DAL
{
    public class Example
    {
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

        public void GetSomeRopository()
        {
            IRepository<Customer, CustomerEnum> repo = UnitOfWork.GetRepository<Customer, CustomerEnum>();
        }
    }
}