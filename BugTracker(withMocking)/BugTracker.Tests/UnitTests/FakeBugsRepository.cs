using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Tests.UnitTests
{
    using Data.Models;
    using Data.Repositories;

    public class FakeBugsRepository : IRepository<Bug>
    {
        public List<Bug> FakeBugs { get; private set; }

        public FakeBugsRepository(List<Bug> fakeBugs)
        {
            this.FakeBugs = fakeBugs;
        }

        public IQueryable<Bug> All()
        {
            throw new NotImplementedException();
        }

        public Bug Find(object id)
        {
            return this.FakeBugs.FirstOrDefault(b => b.Id == (int)id);
        }

        public void Add(Bug entity)
        {
            throw new NotImplementedException();
        }

        public void Update(Bug entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Bug entity)
        {
            throw new NotImplementedException();
        }
    }
}
