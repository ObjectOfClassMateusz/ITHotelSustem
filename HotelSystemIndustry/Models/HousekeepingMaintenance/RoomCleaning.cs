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

        public required Room Room { get; set; }

        public EmployeeProfile? AssignedEmployee { get; set; }

        public DateTime ScheduledDate { get; set; }

        public CleaningStatus Status { get; set; }
    }
}