using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace Videos.Models
{
    using System.Collections.Generic;

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        //private ICollection<Tag> ownTags;
        private ICollection<Video> ownVideos;
        private ICollection<Playlist> ownPlaylists;

        public ApplicationUser()
        {
            //this.ownTags = new HashSet<Tag>();
            this.ownVideos = new HashSet<Video>();
            this.ownPlaylists = new HashSet<Playlist>();
        }

        //public virtual ICollection<Tag> OwnTags
        //{
        //    get { return this.ownTags; }
        //    set { this.ownTags = value; }
        //}

        public virtual ICollection<Video> OwnVideos
        {
            get { return this.ownVideos; }
            set { this.ownVideos = value; }
        }

        public virtual ICollection<Playlist> OwnPlaylists
        {
            get { return this.ownPlaylists; }
            set { this.ownPlaylists = value; }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
}