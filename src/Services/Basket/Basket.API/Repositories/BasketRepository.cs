using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCache;

        public BasketRepository(IDistributedCache redisCache)
        {
            _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
        }

        public async Task<ShoppingCart?> GetBasket(string userName)
        {
            var basket = await _redisCache.GetStringAsync(userName).ConfigureAwait(false);
            return string.IsNullOrEmpty(basket) ? null : JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            if (basket == null || string.IsNullOrEmpty(basket.UserName))
                throw new ArgumentException("Basket or UserName cannot be null.");

            var serializedBasket = JsonConvert.SerializeObject(basket);
            await _redisCache.SetStringAsync(basket.UserName, serializedBasket).ConfigureAwait(false);

            return await GetBasket(basket.UserName).ConfigureAwait(false) ?? basket;
        }

        public async Task DeleteBasket(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
                await _redisCache.RemoveAsync(userName).ConfigureAwait(false);
        }
    }
}
