using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly DiscountGrpcService _discountGrpcService;
        private readonly ILogger<BasketController> _logger;

        public BasketController(IBasketRepository repository, DiscountGrpcService discountGrpcService, ILogger<BasketController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket = await _repository.GetBasket(userName);
            return Ok(basket ?? new ShoppingCart(userName));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            try
            {
                foreach (var item in basket.Items)
                {
                    try
                    {
                        // Attempt to get the discount for the product via gRPC
                        var coupon = await _discountGrpcService.GetDiscount(item.ProductName);

                        if (coupon == null || coupon.Amount <= 0)
                        {
                            _logger.LogWarning("No valid discount found for product: {ProductName}", item.ProductName);
                            continue; // Skip applying a discount if it's invalid or not found
                        }

                        var originalPrice = item.Price;
                        // Apply the discount to the product's price, ensuring it doesn't go negative
                        item.Price = Math.Max(item.Price - coupon.Amount, 0);

                        _logger.LogInformation("Applied discount of {Discount} to product {ProductName}. Original Price: {OriginalPrice}, New Price: {NewPrice}",
                            coupon.Amount, item.ProductName, originalPrice, item.Price);
                    }
                    catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
                    {
                        // Log a warning if the discount was not found for the product
                        _logger.LogWarning("No discount found for product: {ProductName}. gRPC Status Code: {StatusCode}", item.ProductName, ex.StatusCode);
                    }
                    catch (RpcException ex)
                    {
                        // Capture and log any gRPC error (e.g., service unavailable, timeout, etc.)
                        _logger.LogError(ex, "gRPC Error while applying discount for product: {ProductName}. gRPC Status Code: {StatusCode}", item.ProductName, ex.StatusCode);
                        return StatusCode((int)HttpStatusCode.InternalServerError, $"gRPC Error while applying discount for product: {item.ProductName}");
                    }
                    catch (Exception ex)
                    {
                        // Log any non-gRPC error
                        _logger.LogError(ex, "Unexpected error applying discount for product: {ProductName}", item.ProductName);
                        return StatusCode((int)HttpStatusCode.InternalServerError, $"Error applying discount for product: {item.ProductName}");
                    }
                }

                // After applying discounts, update the basket
                var updatedBasket = await _repository.UpdateBasket(basket);
                return Ok(updatedBasket);
            }
            catch (Exception ex)
            {
                // Log and handle unexpected errors
                _logger.LogError(ex, "Unexpected error while updating basket.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal server error");
            }
        }

        [HttpDelete("{userName}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteBasket(string userName)
        {
            await _repository.DeleteBasket(userName);
            return Ok();
        }
    }
}
