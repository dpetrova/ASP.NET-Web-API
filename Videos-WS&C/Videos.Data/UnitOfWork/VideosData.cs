using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Videos.Data.UnitOfWork
{
    using System.Data.Entity;
    using Models;
    using Repositories;

    public class VideosData : IVideosData
    {
        private DbContext context;
        private IDictionary<Type, object> repositories;

        public VideosData(DbContext context)
        {
            this.context = context;
            this.repositories = new Dictionary<Type, object>();
        }

        public IRepository<ApplicationUser> Users
        {
            get { return this.GetRepository<ApplicationUser>(); }
        }

        public IRepository<Location> Locations
        {
            get { return this.GetRepository<Location>(); }
        }

        public IRepository<Playlist> Playlists
        {
            get { return this.GetRepository<Playlist>(); }
        }

        public IRepository<Tag> Tags
        {
            get { return this.GetRepository<Tag>(); }
        }

        public IRepository<Video> Videos
        {
            get { return this.GetRepository<Video>(); }
        }

        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        public int SaveChangesAsync()
        {
            return this.context.SaveChanges();
        }

        private IRepository<T> GetRepository<T>() where T : class
        {
            var type = typeof(T);
            if (!this.repositories.ContainsKey(type))
            {
                var typeOfRepository = typeof(GenericRepository<T>);
                var repository = Activator.CreateInstance(typeOfRepository, this.context);

                this.repositories.Add(type, repository);
            }

            return (IRepository<T>)this.repositories[type];
        }
    }
}
