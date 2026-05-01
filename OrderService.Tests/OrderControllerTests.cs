using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using OrderService;
using OrderService.Controllers;
using OrderService.Models;
using Xunit;

namespace OrderService.Tests
{
    public class OrderControllerTests
    {
        private OrderController GetController()
        {
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new OrderDbContext(options);
            var logger = NullLogger<OrderController>.Instance;

            return new OrderController(context, logger);
        }

        [Fact]
        public async Task PlaceOrder_ShouldReturnOk()
        {
            var controller = GetController();

            var order = new Order { Username = "testuser", TotalAmount = 100m };
            var result = await controller.PlaceOrder(order);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic response = okResult.Value;
            Assert.Equal(200, (int)response.ResponseCode);
            Assert.Equal("Order placed successfully", (string)response.ResponseMessage);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOrders()
        {
            var controller = GetController();

            await controller.PlaceOrder(new Order { Username = "user1", TotalAmount = 50m });
            await controller.PlaceOrder(new Order { Username = "user2", TotalAmount = 75m });

            var result = await controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic response = okResult.Value;
            Assert.Equal(2, ((IEnumerable<Order>)response.Data).Count());
        }

        [Fact]
        public async Task UpdateOrder_ShouldReturnOk()
        {
            var controller = GetController();

            var order = new Order { Username = "user1", TotalAmount = 50m };
            await controller.PlaceOrder(order);

            var updated = new Order { Status = "Completed", TotalAmount = 60m };
            var result = await controller.UpdateOrder(order.Id, updated);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic response = okResult.Value;
            Assert.Equal("Order updated successfully", (string)response.ResponseMessage);
            Assert.Equal("Completed", ((Order)response.Data).Status);
        }

        [Fact]
        public async Task DeleteOrder_ShouldReturnOk()
        {
            var controller = GetController();

            var order = new Order { Username = "user1", TotalAmount = 50m };
            await controller.PlaceOrder(order);

            var result = await controller.DeleteOrder(order.Id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic response = okResult.Value;
            Assert.Equal("Order deleted successfully", (string)response.ResponseMessage);
        }

        [Fact]
        public async Task DeleteOrder_NotFound_ShouldReturn404()
        {
            var controller = GetController();

            var result = await controller.DeleteOrder(999);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            dynamic response = notFoundResult.Value;
            Assert.Equal(404, (int)response.ResponseCode);
        }
    }
}
