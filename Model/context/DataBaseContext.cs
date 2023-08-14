using Microsoft.EntityFrameworkCore;
using Jwt7.Model.Entitys;
using System.Reflection;

namespace Jwt7.Model.context
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext()
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            /* optionsBuilder.UseSqlServer(@"Server=.;Database=Bahar;User Id=sa;Password=Mahanan1377.;");*/
            optionsBuilder.UseSqlServer(@"Server=.;Database=Jwt7;User Id=sa;Password=Mahanan1377.;trusted_connection=true;encrypt=false;");
        }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
