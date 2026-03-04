using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.HousekeepingMaintenance
{
    public enum MaintenanceStatus
    {
        AWAITING_DECISION,    
        IN_PROGRESS, 
        RESOLVED   
    }

    public enum MaintenancePriority
    {
        LOW,
        MEDIUM,
        HIGH
    }

    public class MaintenanceRequest
    {
        [Key]
        public Guid Id { get; set; }

        public required Room Room { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime ReportedDate { get; set; }

        public required string Description { get; set; }
        [Required]
        public MaintenanceStatus Status { get; set; }
        [Required]
        public MaintenancePriority Priority { get; set; }
    }
}