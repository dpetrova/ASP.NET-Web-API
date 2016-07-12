namespace BookShopSystem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Book
    {
        private ICollection<Category> categories;

        public Book()
        {
            this.categories = new HashSet<Category>();
        }
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50), MinLength(1)]
        public string Title { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Copies { get; set; }
        public int? AuthorId { get; set; }
        [Required]
        public Edition Edition { get; set; }
        public AgeRestriction AgeRestriction { get; set; }
        public DateTime? ReleaseDate { get; set; }
        
        public virtual Author Author { get; set; }

        public virtual ICollection<Category> Categories
        {
            get { return this.categories;}
            set { this.categories = value; }
        }
    }
}
