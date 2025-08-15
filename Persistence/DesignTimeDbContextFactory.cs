using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence.Context;

namespace Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SHFTDbContext>
    {
        public SHFTDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<SHFTDbContext> dbContextOptionsBuilder = new();
            dbContextOptionsBuilder.UseNpgsql(Configuration.ConnectionString);
            return new(dbContextOptionsBuilder.Options);
        }
    }

    static class Configuration
    {
        static public string ConnectionString
        {
            get
            {
                ConfigurationManager configurationManager = new();

                // First try SHFTAPI directory
                string shftApiPath = Path.Combine(Directory.GetCurrentDirectory(), "../../SHFTAPI");
                if (Directory.Exists(shftApiPath))
                {
                    configurationManager.SetBasePath(shftApiPath);
                    configurationManager.AddJsonFile("appsettings.json", optional: false);
                    configurationManager.AddJsonFile("appsettings.Development.json", optional: true);
                }
                else
                {
                    // Fallback: Use local appsettings.json in Persistence directory
                    configurationManager.SetBasePath(Directory.GetCurrentDirectory());
                    configurationManager.AddJsonFile("appsettings.json", optional: false);
                }

                return configurationManager.GetConnectionString("DefaultConnection");
            }
        }
    }
}