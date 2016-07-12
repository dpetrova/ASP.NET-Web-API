namespace BookShopSystem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Security.AccessControl;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Routing;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class Purchase
    {
     
        public Purchase()
        {
            this.IsRecalled = false;
        }

        //has user, book, price, date of purchase and is-recalled
        [Key]
        public int Id { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }
        [Required]
        public int BookId { get; set; }
        [Required]
        public DateTime DateOfPurchase { get; set; }
        [Required]
        public bool IsRecalled { get; set; }

        public decimal Price { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Book Book {get; set;}

    }
}
