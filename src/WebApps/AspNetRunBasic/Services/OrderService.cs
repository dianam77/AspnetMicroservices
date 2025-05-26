using AspNetRunBasic.Extensions;
using AspNetRunBasic.Models;

namespace AspNetRunBasic.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _Client;

        public OrderService(HttpClient client)
        {
            _Client = client;
        }

        public async Task<IEnumerable<OrderResponseModel>> GetOrderByUserName(string userName)
        {
            var response = await _Client.GetAsync($"/Order/{userName}");
            return await response.ReadContentAs<List<OrderResponseModel>>();   
        }
    }
}
