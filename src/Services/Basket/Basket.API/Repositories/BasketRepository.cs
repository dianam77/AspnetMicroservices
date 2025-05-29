using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCache;
        private readonly ILogger<BasketRepository> _logger;

        public BasketRepository(IDistributedCache redisCache, ILogger<BasketRepository> logger)
        {
            _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ShoppingCart?> GetBasket(string userName)
        {
            userName = userName.Trim(); 
            try
            {
                var basket = await _redisCache.GetStringAsync(userName).ConfigureAwait(false);
                if (string.IsNullOrEmpty(basket))
                {
                    _logger.LogWarning($"Basket not found in cache for user {userName}");
                    return null;
                }
                return JsonConvert.DeserializeObject<ShoppingCart>(basket);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while fetching basket for user {userName}: {ex.Message}");
                throw;
            }
        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            if (basket == null || string.IsNullOrEmpty(basket.UserName))
                throw new ArgumentException("Basket or UserName cannot be null.");

            var serializedBasket = JsonConvert.SerializeObject(basket);

            try
            {
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(3));

                await _redisCache.SetStringAsync(basket.UserName, serializedBasket, options).ConfigureAwait(false);

                var updatedBasket = await GetBasket(basket.UserName).ConfigureAwait(false);
                return updatedBasket ?? basket;  
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while updating basket for user {basket.UserName}: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteBasket(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return;

            try
            {
                await _redisCache.RemoveAsync(userName).ConfigureAwait(false);
                _logger.LogInformation($"Basket deleted from cache for user {userName}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while deleting basket for user {userName}: {ex.Message}");
            }
        }
    }
}
