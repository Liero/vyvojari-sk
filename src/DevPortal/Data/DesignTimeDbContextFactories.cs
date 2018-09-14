using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Data
{
    public class DbContextDesignTimeFactory<T> : IDesignTimeDbContextFactory<T> where T : DbContext
    {
        public DbContextDesignTimeFactory(Func<DbContextOptions<T>, T> factory)
        {
            Factory = factory;
        }
        public T CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<DbContextDesignTimeFactory<T>>()
                .Build();

            var connectionString = configuration.GetConnectionString(typeof(T).Name);

            var builder = new DbContextOptionsBuilder<T>();
            builder.UseSqlServer(connectionString);
            return Factory(builder.Options);
        }

        public Func<DbContextOptions<T>, T> Factory { get; }
    }

    public class ApplicationDbContextDesignTimeFactory : DbContextDesignTimeFactory<ApplicationDbContext>
    {
        public ApplicationDbContextDesignTimeFactory() : base(options => new ApplicationDbContext(options))
        {

        }
    }

    public class DevPortalDbContextDesignTimeFactory : DbContextDesignTimeFactory<DevPortalDbContext>
    {
        public DevPortalDbContextDesignTimeFactory() : base(options => new DevPortalDbContext(options))
        {

        }
    }

    public class EventsDbContextDesignTimeFactory : DbContextDesignTimeFactory<EventsDbContext>
    {
        public EventsDbContextDesignTimeFactory() : base(options => new EventsDbContext(options))
        {

        }
    }
}
