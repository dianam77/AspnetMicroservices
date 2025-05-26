using AspNetRunBasic.Extensions;
using AspNetRunBasic.Models;

namespace AspNetRunBasic.Services
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient _Client;

        public BasketService(HttpClient client)
        {
            _Client = client;
        }

        public async Task CheckoutBasket(BasketCheckoutModel model)
        {
            var response = await _Client.PostAsJson($"/Basket/Checkout", model);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("something went wrong when calling api.");
            }
        }
        
        public async Task<BasketModel> GetBasket(string userName)
        {
            var response = await _Client.GetAsync($"/Basket/{userName}");
            return await response.ReadContentAs<BasketModel>();
        }

        public async Task<BasketModel> UpdateBasket(BasketModel model)
        {
            var response = await _Client.PostAsJson($"/Basket",model);
            if(response.IsSuccessStatusCode)
                return await response.ReadContentAs<BasketModel>();
            else
            {
                throw new Exception("something went wrong when calling api.");
            }
        }
    }
}
