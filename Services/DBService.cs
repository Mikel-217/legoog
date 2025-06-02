using System.Data.Common;
using legoog.Services;
using legoog.Models;
using Microsoft.EntityFrameworkCore;


namespace legoog.Services.DB;

public class DBService : DbContext
{
    
    public DbSet<Data> searchData { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder); // Add db config
    }


    // TODO:

    // dotnet ef migrations add Data
    // dotnet ef update database

}