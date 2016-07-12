using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Videos.Data.UnitOfWork
{
    using Models;
    using Repositories;

    public interface IVideosData
    {
        IRepository<Location> Locations { get; }

        IRepository<ApplicationUser> Users { get; }

        IRepository<Playlist> Playlists { get; }

        IRepository<Tag> Tags { get; }

        IRepository<Video> Videos { get; }

        int SaveChanges();
    }
}
