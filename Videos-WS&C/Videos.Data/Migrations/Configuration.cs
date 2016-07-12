namespace Videos.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Models;

    internal sealed class Configuration : DbMigrationsConfiguration<Videos.Data.VideosDbContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Videos.Data.VideosDbContext context)
        {
            if (!context.Locations.Any())
            {
                SeedLocations(context);
            }
        }

        private static void SeedLocations(VideosDbContext context)
        {
            var locations = new[]
            {
                new Location {Country = "Bulgaria", City = "Sofia"},
                new Location {Country = "Germany", City = "Munich"},
                new Location {Country = "France", City = "Paris"}
            };

            foreach (var location in locations)
            {
                context.Locations.Add(location);
            }

            context.SaveChanges();
        }
    }
}
