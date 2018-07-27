using Microsoft.EntityFrameworkCore;
using SwaggerIntroduction.Models.DataModels;

namespace SwaggerIntroduction.Repository
{
    public class UserDbContext : DbContext
    {
        public DbSet<UserMaster> UserMaster { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }

        public UserDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<T> GetDbSet<T>() where T : class
        {
            return Set<T>();
        }
    }
}