using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList
{
    public class GetOrderListQuery : IRequest<List<OrdersVm>>
    {
        public string userName { get; set; }

        public GetOrderListQuery(string userName)
        {
            userName = userName ?? throw new ArgumentNullException(nameof(userName));
        }
    }
}
