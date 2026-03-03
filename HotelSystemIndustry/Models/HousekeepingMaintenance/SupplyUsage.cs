using HotelSystemIndustry.Models.Housekeeping;
using HotelSystemIndustry.Models.HousekeepingMaintenance;
using System.ComponentModel.DataAnnotations;

public class SupplyUsage
{
    [Key]
    public Guid Id { get; set; }
    public required RoomCleaning RoomCleaning { get; set; }
    public required HousekeepingSupply Supply { get; set; }
    public decimal AmountUsed { get; set; }
}