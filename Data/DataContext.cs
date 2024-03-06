using Microsoft.EntityFrameworkCore;
using Model;
using System.Collections.Generic;

namespace Data
{
    public class DataContext : DbContext
    {
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<User> Users => Set<User>();

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
            // Constructor body
        }
    }
}
