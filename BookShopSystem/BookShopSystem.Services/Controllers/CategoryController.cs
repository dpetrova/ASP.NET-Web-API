namespace BookShopSystem.Services.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Configuration;
    using System.Web.Http;
    using System.Web.OData;
    using BookShopSystem.Models;
    using Data;
    using Models;

    [RoutePrefix("api/categories")]
    public class CategoryController : ApiController
    {
        ApplicationDbContext context = new ApplicationDbContext();

        //Endpoint: GET => /api/categories => returns all categories (Id;Name)
        [HttpGet]
        [Route("")]
        [EnableQuery]
        public IQueryable<CategoryViewModel> GetAllCategories ()
        {
           var dbCategories = context.Categories;
           var categoriesa = dbCategories.Select(c => new CategoryViewModel()
            {
                Id = c.Id,
                Name = c.Name
            });
            return categoriesa;
        }


        //Endpoint: GET => /api/categories/{id} => returns category(Id; Name) by Id
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetCategoryById(int id)
        {
            Category dbCategory = context.Categories.Find(id);
           
            if (dbCategory == null)
            {
                return this.NotFound();
            }
            var category = new CategoryViewModel(dbCategory);
           
            return this.Ok(category);
        }

        [HttpPut]
        [Route("{id}")]
        //Endpoint: PUT => /api/categories/{id} => Edits a category name (foundbyId). Make sure no duplicates are created
        public IHttpActionResult PutCategory(int id, [FromBody] string newName)
        {
            var dbCategory = context.Categories.Find(id);
            if (dbCategory == null)
            {
                return this.NotFound();
            }
            var dbCategorySameName = context.Categories.FirstOrDefault(c => c.Name == newName);
            if (dbCategorySameName == null)
            {
                dbCategory.Name = newName;
                context.SaveChanges();
                return this.Ok("change is done");
            }
            return this.BadRequest("Dublicate names. Change is not done");
        }

        [HttpDelete]
        [Route("{id}")]
        //Endpoint: DELETE => /api/categories/{id} => Deletes a category (foundbyId).
        public IHttpActionResult DeleteCategory(int id)
        {
            var dbCategory = context.Categories.Find(id);
            if (dbCategory == null)
            {
                return this.NotFound();
            }
            context.Categories.Remove(dbCategory);
            context.SaveChanges();
            return this.Ok("delete is done");
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateCategory([FromBody] string newName)
        {
            //var dbCategorySameName = context.Categories.Any(c => c.Name == newName);
            if (!context.Categories.Any(c => c.Name == newName))
            {
                context.Categories.Add(new Category()
                {
                    Name = newName
                });
                context.SaveChanges();
                return this.Ok("new category is added");
            }
            return this.BadRequest("Dublicate names. Change is not done");
        }

    }
}
