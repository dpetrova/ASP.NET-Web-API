namespace BookShopSystem.Models
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

        private ICollection<Purchase> purchases;

        public ApplicationUser()
        {
            this.purchases = new HashSet<Purchase>();
        }
        public virtual ICollection<Purchase> Purchases {
            get { return this.purchases; }
            set { this.purchases = value; }
        }
        //•	ApplicationUser - has many purchases. It should extend (inherit) the default built-in IdentityUser class and add any extra properties.
        //Users should be able to login/register/logout by using the standart identity API.
    }    
}
