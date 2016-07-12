namespace Restaurants.Data
{
    using System.Data.Entity;
    using System.Data.Entity.Validation;
    using System.Linq;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Migrations;
    using Models;
    using Restauranteur.Models;

    public class RestaurantsContext : IdentityDbContext<ApplicationUser>
    {
        public RestaurantsContext()
            : base("RestaurantsContext")
        {
            Database.SetInitializer(
                new MigrateDatabaseToLatestVersion<RestaurantsContext, Configuration>());
        }

        public virtual IDbSet<Rating> Ratings { get; set; }

        public virtual IDbSet<Town> Towns { get; set; }

        public virtual IDbSet<Restaurant> Restaurants { get; set; }

        public virtual IDbSet<Meal> Meals { get; set; }

        public virtual IDbSet<MealType> MealTypes { get; set; }

        public virtual IDbSet<Order> Orders { get; set; }

        //to show more-detailed errors
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }
    }
}