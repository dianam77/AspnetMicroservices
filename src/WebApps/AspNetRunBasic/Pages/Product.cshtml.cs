using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetRunBasic.Models;
using AspNetRunBasic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspnetRunBasics
{
    public class ProductModel : PageModel
    {
        private readonly ICatalogService _catalogService;
        private readonly IBasketService _basketService;

        public ProductModel(ICatalogService catalogService,
                            IBasketService basketService)
        {
            _catalogService = catalogService;
            _basketService = basketService;
        }

        public IEnumerable<string> CategoryList { get; set; } = new List<string>();
        public IEnumerable<CatalogModel> ProductList { get; set; } = new List<CatalogModel>();

        [BindProperty(SupportsGet = true)]
        public string SelectedCategory { get; set; } = string.Empty;

        /* ---------- GET ---------- */
        public async Task<IActionResult> OnGetAsync()
        {
            var products = await _catalogService.GetCatalog();
            CategoryList = products.Select(p => p.Category).Distinct();
            ProductList = string.IsNullOrWhiteSpace(SelectedCategory)
                            ? products
                            : products.Where(p => p.Category == SelectedCategory);

            return Page();
        }

        /* ---------- POST: Add / Update ---------- */
        public async Task<IActionResult> OnPostUpdateQuantityAsync(string productId, int quantity)
        {
            if (string.IsNullOrWhiteSpace(productId) || quantity < 0)
                return RedirectToPage("Product", new { SelectedCategory });

            const string userName = "swg";
            var basket = await _basketService.GetBasket(userName);

            var item = basket.Items?.FirstOrDefault(i => i.ProductId == productId);

            if (item == null && quantity > 0)
            {
                var product = await _catalogService.GetCatalog(productId);
                if (product == null)                      // دفاع در برابر Null
                    return RedirectToPage("Product", new { SelectedCategory });

                basket.Items.Add(new BasketItemModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    Color = "Black"
                });
            }
            else if (item != null)
            {
                if (quantity == 0)
                    basket.Items.Remove(item);
                else
                    item.Quantity = quantity;
            }

            await _basketService.UpdateBasket(basket);
            return RedirectToPage("Product", new { SelectedCategory });
        }
    }

}
