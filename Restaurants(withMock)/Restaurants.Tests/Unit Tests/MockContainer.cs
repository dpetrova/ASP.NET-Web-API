using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Tests.Unit_Tests
{
    using Data.Repositories;
    using Data.UnitOfWork;
    using Models;
    using Moq;

    public class MockContainer
    {
        public Mock<IRestaurantsData> MockData { get; set; }

        public Mock<IRepository<Restaurant>> RestaurantsMock { get; set; }

        public Mock<IRepository<Town>> TownsMock { get; set; }

        public Mock<IRepository<Rating>> RatingsMock { get; set; }

        public void SetupMocks()
        {
            var fakeTowns = new[]
            {
                new Town()
                {
                    Id = 1,
                    Name = "Sofia"
                },
                new Town
                {
                    Id = 2,
                    Name = "Plovdiv"
                }
            };

            var fakeRestaurants = new[]
            {
                new Restaurant()
                {
                    Id = 1,
                    Town = fakeTowns[0],
                    TownId = fakeTowns[0].Id,
                    Name = "Sushi Express",
                    Ratings = new []
                    {
                        new Rating { Stars = 7 }
                    }
                },
                new Restaurant()
                {
                    Id = 2,
                    Town = fakeTowns[1],
                    TownId = fakeTowns[1].Id,
                    Name = "Hushovete",
                    Ratings = new []
                    {
                        new Rating { Stars = 5 }
                    }
                },
            };

            this.PrepareFakeTowns(fakeTowns);

            this.PrepareFakeRestaurants(fakeRestaurants);

            this.PrepareFakeData();
        }

        private void PrepareFakeData()
        {
            this.MockData = new Mock<IRestaurantsData>();

            this.MockData.Setup(d => d.Towns)
                .Returns(this.TownsMock.Object);

            this.MockData.Setup(d => d.Restaurants)
                .Returns(this.RestaurantsMock.Object);
        }

        private void PrepareFakeTowns(IEnumerable<Town> towns)
        {
            this.TownsMock = new Mock<IRepository<Town>>();
            this.TownsMock.Setup(r => r.All())
                .Returns(towns.AsQueryable());
        }

        private void PrepareFakeRestaurants(IEnumerable<Restaurant> restaurants)
        {
            this.RestaurantsMock = new Mock<IRepository<Restaurant>>();
            this.RestaurantsMock.Setup(r => r.All())
                .Returns(restaurants.AsQueryable());
        }
    }



    //public class MockContainer
    //{
    //    public Mock<IRepository<Restaurant>> RestaurantsMock { get; set; }

    //    public Mock<IRepository<Town>> TownsMock { get; set; }

    //    public void PrepareMocks()
    //    {
    //        this.SetupMocks();
    //    }
        
    //    private void SetupMocks()
    //    {
    //        var fakeTowns = new List<Town>
    //        {
    //            new Town()
    //            {
    //                Id = 1,
    //                Name = "Sofia"
    //            },
    //            new Town
    //            {
    //                Id = 2,
    //                Name = "Plovdiv"
    //            }
    //        };

    //        var fakeRestaurants = new List<Restaurant>
    //        {
    //            new Restaurant()
    //            {
    //                Id = 1,
    //                Town = fakeTowns[0],
    //                TownId = fakeTowns[0].Id,
    //                Name = "Sushi Express",
    //                Ratings = new []
    //                {
    //                    new Rating { Stars = 7 }
    //                }
    //            },
    //            new Restaurant()
    //            {
    //                Id = 2,
    //                Town = fakeTowns[1],
    //                TownId = fakeTowns[1].Id,
    //                Name = "Hushovete",
    //                Ratings = new []
    //                {
    //                    new Rating { Stars = 5 }
    //                }
    //            }
    //        };

    //        this.TownsMock = new Mock<IRepository<Town>>();
    //        this.TownsMock.Setup(r => r.All())
    //            .Returns(fakeTowns.AsQueryable());
    //        this.TownsMock.Setup(r => r.Find(It.IsAny<int>()))
    //            .Returns((int id) =>
    //            {
    //                var towns = fakeTowns.FirstOrDefault(a => a.Id == id);
    //                return towns;
    //            });


    //        this.RestaurantsMock = new Mock<IRepository<Restaurant>>();
    //        this.RestaurantsMock.Setup(r => r.All())
    //            .Returns(fakeRestaurants.AsQueryable());
    //        this.RestaurantsMock.Setup(r => r.Find(It.IsAny<int>()))
    //            .Returns((int id) =>
    //            {
    //                var restaurants = fakeRestaurants.FirstOrDefault(a => a.Id == id);
    //                return restaurants;
    //            });
    //    }
    //}
}
