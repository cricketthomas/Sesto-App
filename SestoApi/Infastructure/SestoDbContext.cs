using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using sesto.api.Infastructure.Data;

namespace sesto.api.Infastructure
{

    public class SestoDbContext : DbContext
    {

        public SestoDbContext(DbContextOptions<SestoDbContext> options, IConfiguration configuration) : base(options) { }

        public DbSet<User> User { get; set; }
        public DbSet<Bookmark> Bookmark { get; set; }
        public DbSet<PlaceLocationResult> Location { get; set; }
        public DbSet<PlaceActivity> Activity { get; set; }
        public DbSet<ActivityAttribute> ActivityAttribute { get; set; }
        public DbSet<ActivityAttributesTypes> ActivityAttributesTypes { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseSqlServer(configuration.get);


        //Data Source=Referrer.db;foreign keys=true;
    }

}