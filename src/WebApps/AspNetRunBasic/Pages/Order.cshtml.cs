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
            try
            {
                Orders = await _orderService.GetOrderByUserName("swg");
            }
            catch (Exception ex)
            {
                // لاگ کامل خطا
                Console.WriteLine("Order API error: " + ex);
                Orders = Enumerable.Empty<OrderResponseModel>();   // صفحه خالی ولی بی‌خطا
                ModelState.AddModelError(string.Empty,
                    "خطا در بازیابی سفارش‌ها. لطفاً بعداً دوباره تلاش کنید.");
            }
            return Page();
        }

    }
}