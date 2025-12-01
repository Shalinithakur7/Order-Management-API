using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementApi.Data;
using OrderManagementApi.Models;
using System.Security.Claims;

namespace OrderManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "JwtBearer")] 
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetCurrentUserName() => User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;


        [HttpPost]
        public async Task<IActionResult> PlaceOrder(OrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = new Order
            {
                ProductName = orderDto.ProductName,
                Quantity = orderDto.Quantity,
                UnitPrice = orderDto.UnitPrice,
                UserName = GetCurrentUserName(), // Set ownership from the authenticated user
                TotalAmount = orderDto.Quantity * orderDto.UnitPrice // Calculate TotalAmount securely
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var userName = GetCurrentUserName();
            // IMPORTANT: Only fetch orders belonging to the currently logged-in user
            var orders = await _context.Orders
                .Where(o => o.UserName == userName)
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var userName = GetCurrentUserName();
            // Fetch order and ensure it belongs to the current user (Authorization check)
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserName == userName);

            if (order == null)
            {
                return NotFound(new { message = $"Order with ID {id} not found or does not belong to user {userName}." });
            }

            return Ok(order);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderDto updatedOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userName = GetCurrentUserName();
            var existingOrder = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserName == userName);

            if (existingOrder == null)
            {
                return NotFound(new { message = $"Order with ID {id} not found or does not belong to user {userName}." });
            }

            // Update fields from DTO
            existingOrder.ProductName = updatedOrderDto.ProductName;
            existingOrder.Quantity = updatedOrderDto.Quantity;
            existingOrder.UnitPrice = updatedOrderDto.UnitPrice;

            // Recalculate TotalAmount on the server
            existingOrder.TotalAmount = updatedOrderDto.Quantity * updatedOrderDto.UnitPrice;

            _context.Entry(existingOrder).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content success
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var userName = GetCurrentUserName();
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserName == userName);

            if (order == null)
            {
                return NotFound(new { message = $"Order with ID {id} not found or does not belong to user {userName}." });
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content success
        }
    }
}