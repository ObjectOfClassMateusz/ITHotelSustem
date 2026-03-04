using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.HousekeepingMaintenance
{
    public class EmployeeShift
    {
        [Key]
        public Guid Id { get; set; }

        public required EmployeeProfile Employee { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; }
    }
}