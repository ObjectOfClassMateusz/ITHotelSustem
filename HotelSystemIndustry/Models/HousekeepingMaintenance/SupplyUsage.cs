using HotelSystemIndustry.Models.Housekeeping;
using HotelSystemIndustry.Models.HousekeepingMaintenance;
using System.ComponentModel.DataAnnotations;

public class SupplyUsage
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public required RoomCleaning RoomCleaning { get; set; }
    [Required]
    public required HousekeepingSupply Supply { get; set; }
    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal AmountUsed { get; set; }
}