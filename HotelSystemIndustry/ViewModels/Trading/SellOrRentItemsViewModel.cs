using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.ViewModels.Trading
{
    
    public class SaleItemAndCount
    {
        public Guid SaleItemId { get; set; }

        public uint Count { get; set; }
    }

    public class SellOrRentItems
    {
        public Guid? ShopPointId { get; set; }

        public IList<SaleItemAndCount> Items { get; set; } = new List<SaleItemAndCount>();
    }

    public class SellOrRentItemsViewModel
    {
        public SellOrRentItems Items { get; set; } = new();

        public Guid? NewItemId { get; set; }

        public uint NewItemCount { get; set; }
    }

}