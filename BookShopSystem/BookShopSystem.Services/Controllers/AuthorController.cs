namespace BookShopSystem.Services.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.OData;
    using BookShopSystem.Models;
    using Data;
    using Models;

    [RoutePrefix("api/authors")]
    public class AuthorController : ApiController
    {
       
        private ApplicationDbContext context = new ApplicationDbContext();

        //Endpoint: GET => /api/authors/{id} => Gets author with id, first name, last name and a list of all book titles.
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetAuthor(int id)
        {
            var dbAutor = context.Authors.FirstOrDefault(a => a.Id == id);
            if (dbAutor == null)
            {
                return this.NotFound();
            }
            return this.Ok(new AuthorViewModel(dbAutor));
        }


        //Endpoint: POST => /api/authors => Creates a new author with first name and last name (mandatory).
        [HttpPost]
        [Route("")]
        public IHttpActionResult AddAuthor([FromBody]AuthorBindingModel author)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest("The data is not valid");
            }
            Author dbAuthor = new Author()
            {
                FirstName = author.FirstName,
                LastName = author.LastName
            };
            context.Authors.Add(dbAuthor);
            context.SaveChanges();
            return this.Ok("The author is added");
        }

        //Endpoint: GET => /api/authors/{id}/books => Gets books from author by id. Returns all data about the book + category names.
        [HttpGet]
        [Route("{id}/books")]
        [EnableQuery]
        public IQueryable<BookViewModel> GetBooksByAuthorId(int id)
        {
            var books = context.Books
                .Where(b => b.AuthorId == id)
                .Select(book => new BookViewModel()
                {
                    Id = book.Id,
                    Title = book.Title,
                    AuthorId = book.AuthorId,
                    AuthorLastName = book.Author.LastName,
                    AgeRestriction = book.AgeRestriction,
                    Copies = book.Copies,
                    Description = book.Description,
                    Price = book.Price,
                    Edition = book.Edition,
                    ReleaseDate = book.ReleaseDate,
                    Categories = book.Categories.Select(bc => bc.Name).ToList()
                });
            return books;
        }

        
    }
}
