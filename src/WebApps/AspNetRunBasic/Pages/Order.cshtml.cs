using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetRunBasic.Models;
using AspNetRunBasic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspnetRunBasics
{
    public class OrderModel : PageModel
    {
        private readonly IOrderService _orderService;

        public OrderModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public IEnumerable<OrderResponseModel> Orders { get; set; } = new List<OrderResponseModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            string userName = User.Identity?.Name ?? "swg"; 

            try
            {
                Orders = await _orderService.GetOrderByUserName(userName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Order API error: " + ex);
                Orders = Enumerable.Empty<OrderResponseModel>();
                ModelState.AddModelError(string.Empty,
                    "خطا در بازیابی سفارش‌ها. لطفاً بعداً دوباره تلاش کنید.");
            }

            return Page();
        }


    }
}