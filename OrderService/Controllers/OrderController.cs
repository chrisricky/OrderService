using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(OrderDbContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Fetching all orders");
            var orders = await _context.Orders.ToListAsync();
            return Ok(new { ResponseCode = 200, ResponseMessage = "Success", Data = orders });
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(Order order)
        {
            order.OrderDate = DateTime.UtcNow;
            order.Status = "Pending";
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Order placed by {User}", order.Username);
            return Ok(new { ResponseCode = 200, ResponseMessage = "Order placed successfully", Data = order });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Order updated)
        {
            _logger.LogInformation("Updating order with ID {Id}", id);
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order with ID {Id} not found", id);
                return NotFound(new { ResponseCode = 404, ResponseMessage = "Order not found" });
            }

            order.Status = updated.Status;
            order.TotalAmount = updated.TotalAmount;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Order with ID {Id} updated successfully", id);

            return Ok(new { ResponseCode = 200, ResponseMessage = "Order updated successfully", Data = order });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            _logger.LogInformation("Deleting order with ID {Id}", id);
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order with ID {Id} not found", id);
                return NotFound(new { ResponseCode = 404, ResponseMessage = "Order not found" });
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Order with ID {Id} deleted successfully", id);

            return Ok(new { ResponseCode = 200, ResponseMessage = "Order deleted successfully" });
        }
    }
}
