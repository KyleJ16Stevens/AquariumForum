using Microsoft.EntityFrameworkCore;
using AquariumForum.Models;

namespace AquariumForum.Data 
{
    public class AquariumContext : DbContext 
    {
        public AquariumContext(DbContextOptions<AquariumContext> options) : base(options) { }

        public DbSet<Discussion> Discussions { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
