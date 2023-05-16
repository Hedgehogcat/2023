namespace Hedgehogcat.Web.Admin.Helpers;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Hedgehogcat.Web.Admin.Entities;

public class DataContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }

    private readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // in memory database used for simplicity, change to a real db for production applications
        options.UseInMemoryDatabase("TestDb");
    }
}