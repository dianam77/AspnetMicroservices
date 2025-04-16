using Discount.GRPC;
using Grpc.Core;

namespace Basket.API.GrpcServices
{
    public class DiscountGrpcService
    {
        private readonly DiscountProtoService.DiscountProtoServiceClient _discountProtoService;

        public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient discountProtoService)
        {
            _discountProtoService = discountProtoService ?? throw new ArgumentNullException(nameof(discountProtoService));
        }

        public async Task<CouponModel> GetDiscount(string productName)
        {
            var request = new GetDiscountRequest { ProductName = productName };

            try
            {
                var response = await _discountProtoService.GetDiscountAsync(request);

                return new CouponModel
                {
                    Amount = response.Amount,
                    Description = response.Description,
                    ProductName = response.ProductName
                };
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"gRPC Error while applying discount for product: {productName}, Error: {ex.Status.Detail}");
                throw;
            }
        }
    }
}
