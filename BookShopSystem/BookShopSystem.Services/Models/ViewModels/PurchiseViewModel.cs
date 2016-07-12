using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShopSystem.Services.Models.ViewModels
{
    public class PurchiseViewModel
    {
        public string UserName { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public DateTime DateOfPurchise { get; set; }
        public bool IsRecall { get; set; }
    }
}