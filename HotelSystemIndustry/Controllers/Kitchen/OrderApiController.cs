using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models.Kitchen;
using HotelSystemIndustry.ViewModels.Kitchen;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelSystemIndustry.Controllers.Kitchen
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class OrderApiController : Controller
    {
        private HotelDbContext _context;
        
        public OrderApiController(HotelDbContext context)
        {
            _context = context;
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterOrder([FromForm] NewOrderViewModel model)
        {
            Order order = new Order
            {
                Id = Guid.NewGuid(),
                SubmissionTime = DateTime.Now.ToUniversalTime(),
                RealisedTime = null,
                TypeId = model.Type,
                DeliveryDestination = model.Destination,
                Products = new List<OrderProduct>()
            };

            var type = await _context.KitchenOrderTypes
                .Where(t => t.Id == model.Type)
                .SingleAsync();
            if (type == null)
                return BadRequest("Invalid order type!");

            order.Type = type;

            _context.Add(order);

            foreach (var prodAndNumber in model.Products)
            {
                var product = await _context.KitchenProducts
                    .Where(p => p.Id == prodAndNumber.ProductId)
                    .SingleAsync();
                if (product == null)
                    return BadRequest("Invalid product in order!");

                OrderProduct orderProduct = new OrderProduct
                {
                    OrderId = order.Id,
                    Order = order,
                    ProductId = prodAndNumber.ProductId,
                    Product = product,
                    Count = prodAndNumber.Count
                };

                _context.Add(orderProduct);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

    }

}