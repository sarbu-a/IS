using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCShop.Data;
using PCShop.Models;

namespace PCShop.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int productId, int quantity = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
                return NotFound();

            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                Type = OrderType.Purchase,
                Items = new List<OrderItem>()
            };

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity
            });

            if (product.HasPromotion)
            {
                var discount = -(product.Price * quantity * 0.10m);

                order.Items.Add(new OrderItem
                {
                    Name = "Promotie",
                    Price = discount,
                    Quantity = 1,
                    IsPromotionDiscount = true
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = order.Id });
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}