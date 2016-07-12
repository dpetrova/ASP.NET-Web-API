namespace BookShopSystem.Services.Models
{
    using BookShopSystem.Models;

    public class CategoryViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public CategoryViewModel(){}

        public CategoryViewModel(Category category)
        {
            this.Id = category.Id;
            this.Name = category.Name;
        }
    }
}