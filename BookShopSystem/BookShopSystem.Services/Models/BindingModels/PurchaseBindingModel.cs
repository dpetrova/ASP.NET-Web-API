

namespace BookShopSystem.Services.Models.BindingModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using BookShopSystem.Models;

    public class PurchaseBindingModel
    {
        public PurchaseBindingModel()
        {
            this.IsRecalled = false;
        }

        [Required]
        public string ApplicationUserId { get; set; }
        [Required]
        public int BookId { get; set; }
        [Required]
        public DateTime DateOfPurchase { get; set; }
        [Required]
        public bool IsRecalled { get; set; }
        public decimal Price { get; set; }

    }
}