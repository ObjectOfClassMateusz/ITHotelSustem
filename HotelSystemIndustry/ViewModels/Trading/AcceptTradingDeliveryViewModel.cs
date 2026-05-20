using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.ViewModels.Trading
{

    public class TradingDeliveryItem
    {
        public Guid SaleItemId { get; set; }

        public Guid MagazineId { get; set; }

        public string? Variant { get; set; } = string.Empty;

        [Range(1, 100)]
        public uint Count { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpireDate { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString="{0:C2}")]
        [Range(typeof(decimal), "0", "10000")]
        public decimal Price { get; set; } = 1;
    }

    
    public class AcceptTradingDeliveryViewModel
    {   
        public IList<TradingDeliveryItem> Items { get; set; } = new List<TradingDeliveryItem>();

        public TradingDeliveryItem NewItem { get; set; } = new();

    }

}