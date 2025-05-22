using Shopping.Aggregator.Models;

namespace Shopping.Aggregator.Services
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient _Client;

        public BasketService(HttpClient client)
        {
            _Client = client;
        }
        public async Task<BasketModel>GetBasket(string username)
        {
            var response = await _Client.GetAsync($"/api/v1/Basket/{username}");
            return await response.ReadContentAs<BasketModel>();
        }
    }
}
