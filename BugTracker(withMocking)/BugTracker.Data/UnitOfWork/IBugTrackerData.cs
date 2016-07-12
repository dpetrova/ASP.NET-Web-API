using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Data.UnitOfWork
{
    using Microsoft.AspNet.Identity;
    using Models;
    using Repositories;

    public interface IBugTrackerData
    {
        IRepository<User> Users { get; }

        IRepository<Bug> Bugs { get; }

        IRepository<Comment> Comments { get; }

        int SaveChanges();
    }
}
