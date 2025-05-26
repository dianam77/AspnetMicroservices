using AspNetRunBasic.Models;

namespace AspNetRunBasic.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponseModel>> GetOrderByUserName(string userName);
    }
}
