using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Reactor.Data.EfContext
{
    public class ReactorDbContextDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ReactorDbContext>
    {
        public ReactorDbContext CreateDbContext(string[] args)
        {
            //Environment Variable
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            //Configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .Build();
            
            //DbContext Options builder
            var builder = new DbContextOptionsBuilder();

            var connectionString = configuration.GetConnectionString("ReactorConnStr");
            
            if(string.IsNullOrEmpty(connectionString)) throw new Exception("Connection string cannot be empty");

            builder.UseSqlServer(connectionString);
            
            return new ReactorDbContext(builder.Options);
        }
    }
}