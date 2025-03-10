using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AquariumForum.Models;
using AquariumForum.Areas.Identity.Data;

namespace AquariumForum.Data
{
    public class AquariumContext : IdentityDbContext<User>
    {
        public AquariumContext(DbContextOptions<AquariumContext> options) : base(options) { }

        public DbSet<Discussion> Discussions { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}