Dapper-DAL
==========

Data access layer with Dapper as wrapper IDbConnection

Good day. Very often when we start write some .NET project we use [Entity Framework (EF)][1] (Thanks Microsoft for good product). We use EF directly or sometimes we wrap EF with Repository or UnitOfWork and Repository patterns. We do it for testing or for hide EF from upper layers ([read more detail][2]). EF and LINQ very fast, simple and elegant solution.

And then in development process we understand - we need more better performance and move some bussines logic to stored prosedures in SQL. But unfortunatly Stored procedures couldn't elegant use in EF solution. We get high dependency with SP in upper layer.

I try to resolve this problem in my current design. 
What points I develop:

- good performance
- dependency from interfaces
- minimum dependencies with data access layer (`IUnitOfWork`)
- elegant executing queries and stored procedures

Thanks awfuly [SamSaffron][3] for [Dapper.NET][4]

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
Than all classes in upper layer need just one dependency: `IUnitOfWork`. I recomend use constructor injection for Unit of work interface.

##Use any Repository for get data from database.
```C#
IRepository<TModel, TEnum> repo = UnitOfWork.GetRepository<TModel, TEnum>();
```
All repositories have two generic parameters:

- TModel - some type what can be mapped directly from some table of databace
- TEnum - special service class for store queries or stored procedures name with strong typing in development



[1]:http://msdn.microsoft.com/en-us/data/ef.aspx
[2]:http://stackoverflow.com/a/5626884/1053480
[3]:https://github.com/SamSaffron
[4]:https://github.com/SamSaffron/dapper-dot-net
[5]:https://simpleinjector.codeplex.com/