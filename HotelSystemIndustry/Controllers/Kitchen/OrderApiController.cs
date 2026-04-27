using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models.Kitchen;
using HotelSystemIndustry.ViewModels.Kitchen;
using Microsoft.AspNetCore.Authorization;
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


        [HttpGet("[action]")]
        public async Task<List<OrderType>> GetOrderTypes()
        {
            var types = await _context.KitchenOrderTypes
                .ToListAsync();

            return types;
        }


        [HttpGet("[action]")]
        public async Task<List<KitchenProduct>> GetAvailableProducts()
        {
            var products = await _context.KitchenProducts
                .ToListAsync();

            return products;
        }

        
        [HttpPost("[action]")]
        public async Task<bool> SubmitOrder(NewOrderViewModel model)
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
                .FirstOrDefaultAsync();
            if (type == null)
                return false;

            order.Type = type;

            _context.Add(order);

            foreach (var prodAndNumber in model.Products)
            {
                var product = await _context.KitchenProducts
                    .Where(p => p.Id == prodAndNumber.ProductId)
                    .FirstOrDefaultAsync();
                if (product == null)
                    return false;

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

            return true;
        }


        [HttpGet("[action]")]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<List<Order>> GetOrdersToRealise()
        {
            var unrealisedOrders = await _context.KitchenOrders
                .AsNoTracking()
                .Where(p => p.RealisedTime == null)
                .Include(p => p.Type)
                .Include(p => p.Products)
                    !.ThenInclude(op => op.Product)
                .ToListAsync();

            return unrealisedOrders;
        }

        
        [HttpPost("[action]/{id}")]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<bool> MarkOrderRealised(Guid id)
        {
            var order = await _context.KitchenOrders
                .Where(o => o.Id == id && o.RealisedTime == null)
                .FirstOrDefaultAsync();

            if (order == null)
                return false;

            order.RealisedTime = DateTime.Now.ToUniversalTime();

            try
            {
                _context.Update(order);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return true;
        }


        [HttpPost("[action]/{id}")]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<bool> CancelOrder(Guid id)
        {
            var order = await _context.KitchenOrders
                .Where(o => o.Id == id && o.RealisedTime == null)
                .FirstOrDefaultAsync();

            if (order == null)
                return false;

            _context.Remove(order);
            await _context.SaveChangesAsync();
            
            return true;
        }
    }

}