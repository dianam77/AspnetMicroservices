using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.models;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailServices;
        private readonly ILogger<CheckoutOrderCommandHandler> _logger;

        public CheckoutOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, IEmailService emailServices, ILogger<CheckoutOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentException(nameof(orderRepository));
            _mapper = mapper ?? throw new ArgumentException(nameof(mapper));
            _emailServices = emailServices ?? throw new ArgumentException(nameof(emailServices));
            _logger = logger ?? throw new ArgumentException(nameof(logger));
        }

        public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var OrderEntity = _mapper.Map<Order>(request);
            var newOrder = await _orderRepository.AddAsync(OrderEntity);

            _logger.LogInformation($"Order {newOrder.Id} is successfully created.");

            await SendMail(newOrder);
            return newOrder.Id;
        }

        private async Task SendMail(Order Order)
        {
            var email = new Email() { To = "ezozkme@gmail.com", Body = $"Order was created.", Subject = "Order wa created" };
            try
            {
                await _emailServices.SendEmail(email);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Order {Order.Id} failed due to an error with the mail servie :{ex.Message}");
            }
        }
    }
}
