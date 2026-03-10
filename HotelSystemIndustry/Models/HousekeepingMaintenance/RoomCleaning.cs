using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.HousekeepingMaintenance
{

    public enum CleaningStatus
    {
        SCHEDULED,
        IN_PROGRESS,
        COMPLETED
    }

    public class RoomCleaning
    {
        [Key]
        public Guid Id { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime ScheduledDate { get; set; }
        [Required]
        public CleaningStatus Status { get; set; }

        [Required]
        public Guid RoomId { get; set; }
        [Required]
        public required Room Room { get; set; }
        public EmployeeProfile? AssignedEmployee { get; set; }
    }
}