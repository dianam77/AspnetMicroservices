using AspNetRunBasic.Models;

namespace AspNetRunBasic.Services
{
    public interface IBasketService
    {
        Task<BasketModel> GetBasket(string userName);

        Task<BasketModel> UpdateBasket(BasketModel model);

        Task CheckoutBasket(BasketCheckoutModel model);
    }
}
