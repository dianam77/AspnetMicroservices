using System;
using System.Threading.Tasks;
using AspNetRunBasic.Models;
using AspNetRunBasic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspnetRunBasics
{
    public class CartModel : PageModel
    {
        private readonly IBasketService _basketService;

        public CartModel(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public BasketModel Cart { get; set; } = new BasketModel();

        public async Task<IActionResult> OnGetAsync()
        {
            var userName = "swg";
            Cart = await _basketService.GetBasket(userName);

            // Debug: ببینید چندتا آیتم دارید و تعدادشون چنده
            foreach (var item in Cart.Items)
            {
                Console.WriteLine($"Product: {item.ProductName}, Quantity: {item.Quantity}");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveToCartAsync(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return RedirectToPage();
            }

            var userName = "swg";
            var basket = await _basketService.GetBasket(userName);

            if (basket?.Items != null)
            {
                var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item != null)
                {
                    basket.Items.Remove(item);
                    await _basketService.UpdateBasket(basket);
                }
            }

            return RedirectToPage();
        }


    }
}