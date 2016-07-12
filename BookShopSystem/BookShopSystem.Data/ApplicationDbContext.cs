namespace BookShopSystem.Data
{
    using System.Data.Entity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("BookSystemConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, BookShopSystem.Data.Migrations.Configuration>());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public IDbSet<Author> Authors { get; set; }
        public IDbSet<Book> Books { get; set; }
        public IDbSet<Category> Categories { get; set; }
        public IDbSet<Purchase> Purchases { get; set; }
    }
}
