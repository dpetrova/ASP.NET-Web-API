namespace BookShopSystem.Services.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using BookShopSystem.Models;

    public class AuthorViewModel
    {
        public AuthorViewModel(Author dbAuthor)
        {
            this.Id = dbAuthor.Id;
            this.FirstName = dbAuthor.FirstName;
            this.LastName = dbAuthor.LastName;
            this.Books = dbAuthor.Books.Select(dbb => dbb.Title).ToList();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> Books { get; set; }
    }
}