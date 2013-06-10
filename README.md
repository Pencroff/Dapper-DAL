Dapper-DAL
==========

Data access layer with Dapper as wrapper IDbConnection

Good day. Very often when we start write some .NET project we use [Entity Framework (EF)][1] (Thanks Microsoft for good product). We use EF directly or sometimes we wrap EF with Repository or UnitOfWork and Repository patterns. We do it for testing or for hiding EF from upper layers ([read more detail][2]). EF and LINQ very fast, simple and elegant solution.

And then in development process we understand - we need more better performance and move some bussines logic to stored prosedures in SQL. But unfortunatly Stored procedures couldn't be used elegant with EF solution. We get high dependency with SP in upper layer.

I try to resolve this problem in my current design. 
What points I develop:

- good performance
- dependency from interfaces
- minimum dependencies with data access layer (`IUnitOfWork`)
- elegant executing queries and stored procedures

Thanks a lot to [SamSaffron][3] for [Dapper.NET][4]

Let's start explain about Dapper DAL.

##Initialize in Service layer or Bussines logic layer
```C#
IDapperContext context = new DapperContext();
IFactoryRepository repoFactory = new FactoryRepository();
IUnitOfWork unitOfWork = new UnitOfWork(context, repoFactory);
```
We can easy setup all dependencies of Unit Of Work in some IoC container. Using [SimpleInjector][5] for example:
```C#
container.Register<IDapperContext, DapperContext>();
container.Register<IFactoryRepository, FactoryRepository>();
container.Register<IUnitOfWork, UnitOfWork>();
```
Than all classes in upper layer need just one dependency: `IUnitOfWork`. I recomend to use constructor injection for Unit of work interface.

##Using Repository
```C#
IRepository<TModel, TEnum> repo = UnitOfWork.GetRepository<TModel, TEnum>();
```
All repositories have two generic parameters:

- TModel - some type what can be mapped directly from some table of databace
- TEnum - special service class for store queries or stored procedures name with strong typing in development

### Repository interface
```C#
public interface IRepository<T, in TRepoQuery>
        where T : class
        where TRepoQuery : EnumBase<TRepoQuery, string>
{
    IDbConnection Conn { get; }
    IDapperContext Context { get; }

    void Add(T entity, IDbTransaction transaction = null, int? commandTimeout = null);
    void Update(T entity, IDbTransaction transaction = null, int? commandTimeout = null);
    void Remove(T entity, IDbTransaction transaction = null, int? commandTimeout = null);

    T GetByKey(object key, IDbTransaction transaction = null, int? commandTimeout = null);

    IEnumerable<T> GetAll(IDbTransaction transaction = null, int? commandTimeout = null);
    IEnumerable<T> GetBy(object where = null, object order = null, 
				IDbTransaction transaction = null, int? commandTimeout = null);

    IEnumerable<TSp> Exec<TSp>(TRepoQuery repoQuery, DynamicParameters param = null, 
				IDbTransaction transaction = null, int? commandTimeout = null);
    void Exec(TRepoQuery repoQuery, DynamicParameters param = null, 
				IDbTransaction transaction = null, int? commandTimeout = null);
}
```
Some couple of words about properties and methods:

- `IDbConnection Conn` - database connection
- `IDapperContext Context` - database conection wraper for service functions, for example I use it for connecting and configuring MiniProfiler.
- `void Add(T entity, ...`, `void Update(T entity, ...`, `void Remove(T entity, ...`, `T GetByKey(object key, ...` - simple operation with database entities, I used [Dapper.Contrib][6] extension
- `IEnumerable<T> GetAll(...` - get all entities from `T` type table
- `IEnumerable<T> GetBy(object where = null, object order = null, ...` - get entities with some `WHERE` and `ORDER` conditions
- `IEnumerable<TSp> Exec<TSp>` and `void Exec` similar methods for executing complex queries or stored procedures, these methods have just one diference, second methods doesn't have return value

Also I used ModelGenerator.tt from [SimpleCRUD][7] project for generating POCO object. It was corrected for corect get primary key of table, setup table name in Table Attribute

### Example using `GetBy` Method
```C#
IRepository<Customer, CustomerEnum> repo = UnitOfWork.GetRepository<Customer, CustomerEnum>();
IEnumerable<Customer> customers = 
    repo.GetBy(
        where: new { Zip = "12345", Registered = new DateTime(year: 2013, month: 7, day: 7) }, 
        order: new { Registered = SortAs.Desc, Name = SortAs.Asc }
    );
// Generated SQL Query : 
// 'SELECT * FROM Customer WHERE Zip = @Zip AND Registered = @Registered ORDER BY Registered DESC, Name ASC'
```
### Example getting data by stored procedure
```C#
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
``` 
### Query Enum Initialization
```C#
public class CustomerEnum : EnumBase<CustomerEnum, string>
{
    public static readonly CustomerEnum GetCustomerByPage = 
		new CustomerEnum("GetCustomerByPage", "[dbo].[spCustomerListByPageGet]", CommandType.StoredProcedure);

    public CustomerEnum(string Name, string EnumValue, CommandType? cmdType)
        : base(Name, EnumValue, cmdType)
    {
    }
}
```
If you don't have any extra query or stored procedure for database entity, you can use `EmptyEnum`
```C#
var userRepo = UnitOfWork.GetRepository<User, EmptyEnum>();
```

[1]:http://msdn.microsoft.com/en-us/data/ef.aspx
[2]:http://stackoverflow.com/a/5626884/1053480
[3]:https://github.com/SamSaffron
[4]:https://github.com/SamSaffron/dapper-dot-net
[5]:https://simpleinjector.codeplex.com/
[6]:https://github.com/SamSaffron/dapper-dot-net/tree/master/Dapper.Contrib
[7]:https://github.com/ericdc1/Dapper.SimpleCRUD

[![githalytics.com alpha](https://cruel-carlota.pagodabox.com/c5ee47a79b6ef93bd10651743bab2b0d "githalytics.com")](http://githalytics.com/Pencroff/Dapper-DAL)



