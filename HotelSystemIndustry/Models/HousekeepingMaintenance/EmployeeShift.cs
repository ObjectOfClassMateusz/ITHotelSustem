using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.HousekeepingMaintenance
{
    public class EmployeeShift
    {
        [Key]
        public Guid Id { get; set; }

        public required EmployeeProfile Employee { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}