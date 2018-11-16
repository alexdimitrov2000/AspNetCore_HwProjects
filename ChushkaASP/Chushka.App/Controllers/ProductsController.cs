using Chushka.App.Models.Products;
using Chushka.Models;
using Chushka.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Chushka.App.Controllers
{
    public class ProductsController : BaseController
    {
        [Authorize]
        [Route("Products/Details/{id}")]
        public IActionResult Details(int id)
        {
            var productExists = this.Context.Products
                .Any(p => p.Id == id);

            if (!productExists)
            {
                return this.BadRequest("Invalid product id");
            }

            var productViewModel = this.Context.Products
                .Where(p => p.Id == id)
                .Select(p => new ProductDetailsViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    Type = p.Type.ToString()
                })
                .First();

            return this.View(productViewModel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(ProductCreateViewModel model)
        {
            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Type = Enum.Parse<ProductType>(model.Type)
            };

            this.Context.Products.Add(product);
            this.Context.SaveChanges();

            return this.Redirect($"/Products/Details/{product.Id}");
        }

        [Authorize(Roles = "Admin")]
        [Route("Products/Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var productExists = this.Context.Products
                   .Any(p => p.Id == id);

            if (!productExists)
            {
                return this.BadRequest("Invalid product id");
            }

            var productViewModel = this.Context.Products
                .Select(p => new ProductEditViewModel
                {
                    Description = p.Description,
                    Name = p.Name,
                    Price = p.Price,
                    Type = p.Type.ToString(),
                    Id = p.Id
                })
                .First(p => p.Id == id);

            return this.View(productViewModel);
        }

        [Authorize(Roles = "Admin")]
        [Route("Products/Edit/{id}")]
        [HttpPost]
        public IActionResult Edit(ProductEditViewModel model)
        {
            var product = this.Context.Products.FirstOrDefault(p => p.Id == model.Id);

            if (product == null)
            {
                return this.BadRequest("Invalid product id");
            }

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Type = Enum.Parse<ProductType>(model.Type);

            this.Context.SaveChanges();

            return this.Redirect($"/Products/Details/{product.Id}");
        }

        [Authorize]
        [Route("Products/Order/{id}")]
        public IActionResult Order(int id)
        {
            var client = this.Context.Users.FirstOrDefault(u => u.UserName == this.User.Identity.Name);

            var order = new Order
            {
                OrderedOn = DateTime.UtcNow,
                ProductId = id,
                Client = client
            };

            this.Context.Orders.Add(order);

            try
            {
                this.Context.SaveChanges();
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }

            return this.Redirect("/Orders/All");
        }

        [Authorize(Roles = "Admin")]
        [Route("Products/Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var productExists = this.Context.Products
                      .Any(p => p.Id == id);

            if (!productExists)
            {
                return this.BadRequest("Invalid product id");
            }

            var productViewModel = this.Context.Products
                .Select(p => new ProductEditViewModel
                {
                    Description = p.Description,
                    Name = p.Name,
                    Price = p.Price,
                    Type = p.Type.ToString(),
                    Id = p.Id
                })
                .First(p => p.Id == id);

            return this.View(productViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Products/DoDelete/{id}")]
        public IActionResult DoDelete(int id)
        {
            var product = this.Context.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return this.BadRequest("Invalid product id");
            }

            try
            {
                this.Context.Products.Remove(product);
                this.Context.SaveChanges();
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }

            return this.Redirect("/");
        }
    }
}
