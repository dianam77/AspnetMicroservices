using System;
using System.Linq;                // فراموش نشود
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

        // GET: /Cart
        public async Task<IActionResult> OnGetAsync()
        {
            var userName = "swg";
            Cart = await _basketService.GetBasket(userName);
            return Page();
        }

        // POST: /Cart?handler=RemoveToCart
        public async Task<IActionResult> OnPostRemoveToCartAsync(string productId)
        {
            if (string.IsNullOrEmpty(productId))
                return RedirectToPage();

            var userName = "swg";
            var basket = await _basketService.GetBasket(userName);

            var item = basket?.Items?.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                basket.Items.Remove(item);
                await _basketService.UpdateBasket(basket);
            }

            return RedirectToPage();
        }

        // POST: /Cart?handler=UpdateQuantity   ← هندلر جدید
        public async Task<IActionResult> OnPostUpdateQuantityAsync(string productId, int quantity)
        {
            if (string.IsNullOrEmpty(productId) || quantity < 1)
                return RedirectToPage();

            var userName = "swg";
            var basket = await _basketService.GetBasket(userName);

            var item = basket?.Items?.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
                await _basketService.UpdateBasket(basket);
            }

            // پس از به‌روزرسانی مجدداً صفحه را بارگیری می‌کنیم
            return RedirectToPage();
        }
    }
}
