using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.ViewModels.Events
{
    
    public class BookingEventViewModel
    {
        [Required]
        public Guid EventTypeId { get; set; }

        
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; }


        public uint NumGuests { get; set; } = 10;

        [Range(0, 20)]
        public uint NumServantStuff { get; set; } = 0;


        public class EquipmentSelection
        {
            public Guid EquipmentInstanceId { get; set; }

            public bool Selected { get; set; }
        }


        public class HallSelection
        {
            public Guid HallId { get; set; }

            public bool Selected { get; set; }

            public IList<EquipmentSelection> Equipment { get; set; } = new List<EquipmentSelection>();
        }

        public IList<HallSelection> Halls { get; set; } = new List<HallSelection>();
    }

}