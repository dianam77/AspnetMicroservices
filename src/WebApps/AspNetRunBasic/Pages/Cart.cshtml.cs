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
            return Page();
        }

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

            return RedirectToPage();
        }
    }
}
