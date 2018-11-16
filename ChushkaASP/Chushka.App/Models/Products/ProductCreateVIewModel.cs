using Chushka.Models.Enums;

namespace Chushka.App.Models.Products
{
    public class ProductCreateViewModel
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }
    }
}
