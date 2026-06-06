using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.HousekeepingMaintenance
{
    public enum LostAndFoundStatus
    {
        IN_STORAGE,
        RETURNED_TO_GUEST,
        DISPOSED
    }

    public class LostAndFoundItem
    {
        [Key]
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime FoundDate { get; set; }
        [Required]
        public LostAndFoundStatus Status { get; set; }

        public Guid RoomId { get; set; }
        public required Room Room { get; set; }
        public required string FoundByEmployeeName { get; set; }
    }
}