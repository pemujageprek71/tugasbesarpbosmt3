using TA_sem3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TA_sem3.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Destination> Destinations => Set<Destination>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<DestinationCategory> DestinationCategories => Set<DestinationCategory>();
    public DbSet<DestinationFacility> DestinationFacilities => Set<DestinationFacility>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<DestinationCategory>().HasKey(x => new { x.DestinationId, x.CategoryId });
        b.Entity<DestinationFacility>().HasKey(x => new { x.DestinationId, x.FacilityId });

        b.Entity<Category>().HasIndex(c => c.CategoryName).IsUnique();
        b.Entity<Facility>().HasIndex(f => f.FacilityName).IsUnique();
        b.Entity<Destination>().HasIndex(d => d.Region);
    }
}
