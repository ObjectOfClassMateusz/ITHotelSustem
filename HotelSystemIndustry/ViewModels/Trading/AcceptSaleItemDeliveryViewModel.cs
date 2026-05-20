namespace HotelSystemIndustry.ViewModels.Trading
{
    
    public class AcceptSaleItemDeliveryViewModel
    {
        
        public Guid SaleItemId { get; set; }

        public string Variant { get; set; } = string.Empty;

        public uint AddCount { get; set; } = 0;

        public Guid ToMagazineId { get; set; }

    }

}