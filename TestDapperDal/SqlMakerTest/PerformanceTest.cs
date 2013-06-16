using Dapper_DAL.Infrastructure.Enum;
using Dapper_DAL.SqlMaker;
using NUnit.Framework;

namespace TestDapperDal.SqlMakerTest
{
    [TestFixture]
    public class PerformanceTest
    {
        [SetUp]
        public void SetUp()
        {
            QueryMaker.DbScheme = "dbo";
        }

        [Test]
        public void FullSelectForCustomerTimeTest()
        {
            int cnt = 1000000;
            string sql = null;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < cnt; i++)
            {
                sql = QueryMaker.New()
                    .SELECT()
                        .Col("Id", "Id")
                        .Col("Name", "Name")
                        .Col("Description", "Desc")
                        .Col("Address", "Addr")
                        .Col("Zip", "Zip")
                        .Col("Balance", "Balance")
                        .Col("Registered", "Reg")
                    .FROM()
                        .Tab("Customer")
                    .WHERE("Zip = @zip")
                    .ORDERBY("Name", SortAs.Asc)
                    .RawSql();
            }
            stopwatch.Stop();
            var example = "SELECT\n\tId AS Id\n\t, Name AS Name"
                        + "\n\t, Description AS Desc\n\t, Address AS Addr\n\t, Zip AS Zip"
                        + "\n\t, Balance AS Balance\n\t, Registered AS Reg"
                        + "\nFROM\n\t[dbo].[Customer]"
                        + "\nWHERE\n\tZip = @zip\nORDER BY Name ASC;";
            Assert.That(sql, Is.EqualTo(example).IgnoreCase);
            System.Diagnostics.Trace.WriteLine(stopwatch.Elapsed.TotalMilliseconds.ToString("Total 0.00 ms"));
            System.Diagnostics.Trace.WriteLine(((double)(stopwatch.Elapsed.TotalMilliseconds * 1000) /
        cnt).ToString("0.00 us per one query"));
        }
         
    }
}