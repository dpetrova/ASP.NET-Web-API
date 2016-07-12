namespace BookShopSystem.Services.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Principal;
    using System.Web.Http;
    using System.Web.OData;
    using BookShopSystem.Models;
    using Data;
    using Models;
    using Models.BindingModels;
    using Models.ViewModels;


    [RoutePrefix("api/books")]
    public class BookController : ApiController
    {
        ApplicationDbContext context = new ApplicationDbContext();

        //Endpoint: GET => /api/books/{id} => returns data about a book(all data+category names, authorname,authorID) by id.
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetBookById(int id)
        {
           if (context.Books.Any(b => b.Id == id))
            {
                Book book = context.Books.Find(id);
                return this.Ok(new BookViewModel(book));
            }

            return this.NotFound();
        }

        //Endpoint: GET => /api/books?search={word} => returns top 10 books(Title; Id) which contain the given substring, sorted by title (ascending). 
        [HttpGet]
        [Route("")]
        [EnableQuery]
        public IQueryable<BookViewModelTitleId> BooksContainingWord (string search)
        {
            var books = context.Books
                .Where(b => b.Title.Contains(search))
                .OrderBy(b => b.Title)
                .Take(10)
                .Select(b => new BookViewModelTitleId()
                {
                    Id = b.Id,
                    Title = b.Title
                });
            return books;
        }

        //Endpoint: PUT =>/api/books/{id} => Edit book by ID. Receives book title, description, price, copies, edition, age restriction, release date and author id.
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult PutEditBook(int id, [FromBody]BookBindingModel book)
        {
            Book dbBook = context.Books.FirstOrDefault(b => b.Id == id);

            if (dbBook == null)
            {
                return this.NotFound();
            }
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }
            dbBook.Title = book.Title;
            dbBook.Description = book.Description;
            dbBook.Price = book.Price;
            dbBook.Copies = book.Copies;
            dbBook.Edition = book.Edition;
            dbBook.AgeRestriction = book.AgeRestriction;
            dbBook.ReleaseDate = book.ReleaseDate;
            dbBook.AuthorId = book.AuthorId;
            context.SaveChanges();
            return this.Ok("Changes are done");
        }

        //Endpoint: DELETE =>/api/books/{id} => DELETE book by ID.
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteBookById(int id)
        {
            var book = context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return this.NotFound();
            }
            context.Books.Remove(book);
            context.SaveChanges();
            return this.Ok("The book is deleted");
        }


        //Endpoint: POST => /api/books => Add a new book 
        //                  with title, description, price, copies, edition, age restriction, release date and a string with space-separated category names.
        [HttpPost]
        [Route("")]
        public IHttpActionResult AddBook(BookBindingModel book)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            string[] categories = book.Categories.Split(' ');
            var bookCategories = new HashSet<Category>();
            if (categories.Length != 0)
            {
                foreach (var cat in categories)
                {
                    if (!context.Categories.Any(c => c.Name.Equals(cat)))
                    {
                        context.Categories.Add(new Category() {Name = cat});
                        context.SaveChanges();
                    }
                    Category currentCategory = context.Categories.First(c => c.Name.Equals(cat));
                    bookCategories.Add(currentCategory);
                }
            }

            var dbBook = new Book()
            {
                Title = book.Title,
                Description = book.Description,
                Price = book.Price,
                Copies = book.Copies,
                Edition = book.Edition,
                AgeRestriction = book.AgeRestriction,
                ReleaseDate = book.ReleaseDate,
                Categories = bookCategories
            };
            context.Books.Add(dbBook);
            context.SaveChanges();
            return this.Ok("Book is added");
        }

        //Endpoint: PUT => /api/books/buy/{id} => Creates a purchase with the specified book and the currently logged user. 
        //                                        Decrements the book's copies by 1. If there are no copies, return an appropriate status code.    
        [Authorize]
        [HttpPut]
        [Route("buy/{id}")]
        public IHttpActionResult CreatePurchise(int id)
        {
            var book = context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return this.BadRequest();
            }
            if (book.Copies < 1)
            {
                return this.NotFound();
            }
            var identity = Request.GetRequestContext().Principal.Identity;
            var user = context.Users.FirstOrDefault(u => u.Email == identity.Name);
            if (user == null)
            {
                return this.BadRequest();
            }
            var currentpurchase = new Purchase()
            {
                BookId = id,
                DateOfPurchase = DateTime.Now,
                ApplicationUserId = user.Id,
                Price = book.Price
            };
            context.Purchases.Add(currentpurchase);
            book.Copies = book.Copies - 1;
            context.SaveChanges();
            return this.Ok("purchase is done")
            ;
        }

        //Endpoint: PUT => /api/books/recall/{id} => Returns the book if less than 30 days have passed since the purchase. 
        //                                           Increments the book count by 1 and sets the purchase to recalled.
        [Authorize]
        [HttpPut]
        [Route("recall/{id}")]
        public IHttpActionResult RecallPurchise(int id)
        {
            //chek if there is purchise with that id
            var currentPurchise = context.Purchases.FirstOrDefault(p => p.Id == id);
            if (currentPurchise == null)
            {
                return this.BadRequest();
            }
            //check if user in request is the same as logged user
            var identity = Request.GetRequestContext().Principal.Identity;
            var user = context.Users.FirstOrDefault(u => u.Email == identity.Name);
            if (user.Id.Equals( currentPurchise.ApplicationUserId) == false)
            {
                return this.BadRequest();
            }

            //check the data of perchise
            TimeSpan purchisePeriod = DateTime.Today - currentPurchise.DateOfPurchase;
            if (purchisePeriod.Days >= 30)
            {
                return this.BadRequest();
            }

            currentPurchise.IsRecalled = true;
            currentPurchise.Book.Copies = currentPurchise.Book.Copies + 1;
            context.SaveChanges();
            return this.Ok("Purchase Recall is done")
            ;
        }

    }
}
