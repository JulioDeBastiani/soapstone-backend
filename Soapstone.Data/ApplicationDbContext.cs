using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Soapstone.Domain;

namespace Soapstone.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Upvote> Upvotes { get; set; }
        public DbSet<Downvote> Downvotes { get; set; }
        public DbSet<SavedPost> SavedPosts { get; set; }
        public DbSet<Report> Reports { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}