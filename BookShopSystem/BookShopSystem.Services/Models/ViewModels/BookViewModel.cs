namespace BookShopSystem.Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using BookShopSystem.Models;

    public class BookViewModel
    {
        public BookViewModel()
        {
            this.Categories = new List<string>();
        }
     
      public BookViewModel(Book book)
        {
            this.Id = book.Id;
            this.Title = book.Title;
            this.AuthorId = book.Id;
            this.AuthorLastName = book.Author.LastName;
            this.AgeRestriction = book.AgeRestriction;
            this.Copies = book.Copies;
            this.Description = book.Description;
            this.Edition = book.Edition;
            this.Price = book.Price;
            this.ReleaseDate = book.ReleaseDate;
            this.Categories = new List<string>();
            foreach (var bc in book.Categories)
            {
                this.Categories.Add(bc.Name);
            }
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Copies { get; set; }
        public int? AuthorId { get; set; }
        public string AuthorLastName { get; set; }
        public Edition Edition { get; set; }
        public AgeRestriction AgeRestriction { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public ICollection<string> Categories { get; set; }
       
    }
}