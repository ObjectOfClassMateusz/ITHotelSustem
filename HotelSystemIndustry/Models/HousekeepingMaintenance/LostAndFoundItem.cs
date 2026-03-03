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
        public DateTime FoundDate { get; set; }
        public LostAndFoundStatus Status { get; set; }
        public required Room Room { get; set; }

        public required EmployeeProfile FoundByEmployee { get; set; }
    }
}