using Microsoft.EntityFrameworkCore;
using Reactor.Data.EfContext;

namespace Reactor.Tests.Helper
{
    public static class InMemoryDbHelper
    {
        public static DbContextOptions<ReactorDbContext> SetUpInMemoryDb(string dbName)
        {
            var options = new DbContextOptionsBuilder<ReactorDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return options;
        }
    }
}