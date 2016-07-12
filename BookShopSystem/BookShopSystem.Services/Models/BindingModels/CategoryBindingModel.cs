namespace BookShopSystem.Services.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using BookShopSystem.Models;

    public class CategoryBindongModel
    {
        private ICollection<Book> books;

        public CategoryBindongModel()
        {
            this.books = new HashSet<Book>();
        }
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public virtual ICollection<Book> Books
        {
            get { return this.books; }
            set { this.books = value; }
        }
    }
}