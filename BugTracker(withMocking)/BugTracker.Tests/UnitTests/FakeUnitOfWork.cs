using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Tests.UnitTests
{
    using Data.Models;
    using Data.Repositories;
    using Data.UnitOfWork;

    public class FakeUnitOfWork : IBugTrackerData
    {
        private IRepository<Bug> fakeBugRepository;
        private int saveChangesCallCount;

        public int SaveChangesCallCount
        {
            get { return this.saveChangesCallCount; }
            private set { this.saveChangesCallCount = value; }
        }

        public FakeUnitOfWork(IRepository<Bug> fakeRepository)
        {
            this.fakeBugRepository = fakeRepository;
        }

        public IRepository<User> Users
        {
            get { throw new NotImplementedException(); }
        }

        public IRepository<Bug> Bugs
        {
            get { return this.fakeBugRepository; }
        }

        public IRepository<Comment> Comments
        {
            get { throw new NotImplementedException(); }
        }

        public int SaveChanges()
        {
            this.SaveChangesCallCount++;
            return this.SaveChangesCallCount;
        }
    }
}
