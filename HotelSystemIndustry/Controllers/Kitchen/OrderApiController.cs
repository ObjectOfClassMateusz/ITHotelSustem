using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.Models;
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
        public async Task<IList<Hotel>> GetHotels()
        {
            return await _context.Hotels.ToListAsync();
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
        public async Task<Guid> SubmitOrder(NewOrderViewModel model)
        {
            Guid guid = Guid.NewGuid();

            Order order = new Order
            {
                Id = guid,
                SubmissionTime = DateTime.Now.ToUniversalTime(),
                RealisedTime = null,
                TypeId = model.Type,
                HotelId = model.HotelId,
                DeliveryDestination = model.Destination,
                Products = new List<OrderProduct>()
            };

            var type = await _context.KitchenOrderTypes
                .Where(t => t.Id == model.Type)
                .FirstOrDefaultAsync();
            if (type == null)
                return Guid.Empty;

            order.Type = type;

            _context.Add(order);

            foreach (var prodAndNumber in model.Products)
            {
                var product = await _context.KitchenProducts
                    .Where(p => p.Id == prodAndNumber.ProductId)
                    .FirstOrDefaultAsync();
                if (product == null)
                    return Guid.Empty;

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

            return guid;
        }


        [HttpGet("[action]/{hotelId}")]
        [Authorize(Roles="KitchenEmployee")]
        public async Task<List<Order>> GetOrdersToRealise(Guid hotelId)
        {
            var unrealisedOrders = await _context.KitchenOrders
                .AsNoTracking()
                .Include(p => p.Type)
                .Include(p => p.Products)
                    !.ThenInclude(op => op.Product)
                .Where(p => p.RealisedTime == null && p.HotelId == hotelId)
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