namespace BookShopSystem.Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using BookShopSystem.Models;

    public class BookBindingModel
    {
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
        public string Categories { get; set; }
    }
}