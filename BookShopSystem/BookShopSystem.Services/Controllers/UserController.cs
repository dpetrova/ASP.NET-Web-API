
namespace BookShopSystem.Services.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.OData;
    using Data;
    using Models.ViewModels;
    using WebGrease.Css.Extensions;

    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        ApplicationDbContext context = new ApplicationDbContext();

        //Endpoint: GET => /api/user/{username}/purchases => Gets all purchases of the specified user, ordered by date of purchase. 
        //Returns username and for each purchase - book title, purchase price, date of purchase and whether it's recalled or not.

        [HttpGet]
        [Route("{username}/purchases")]
        [EnableQuery]
        public IQueryable<PurchiseViewModel> PurchisesByUser(string username)
        {
            var resultPurchases = context.Purchases.Where(p => p.ApplicationUser.UserName == username)
                .Select(p => new PurchiseViewModel()
                {
                    UserName = p.ApplicationUser.UserName,
                    Title = p.Book.Title,
                    Price = p.Price,
                    DateOfPurchise = p.DateOfPurchase,
                    IsRecall = p.IsRecalled
                })
                .OrderBy(p => p.DateOfPurchise);
            
            return resultPurchases;
        }
    }
}
