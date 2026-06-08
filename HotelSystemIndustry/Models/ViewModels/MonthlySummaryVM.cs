namespace HotelSystemIndustry.Models.ViewModels
{
    public class MonthlySummaryVM
    {
        public Guid HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; } = string.Empty;

        public int TotalReservations { get; set; }
        public int TotalGuests { get; set; }
        public int TotalNights { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AvgRevenuePerNight { get; set; }
        public decimal AvgStayLength { get; set; }

        public List<ReservationRowVM> Reservations { get; set; } = new();
    }

    public class ReservationRowVM
    {
        public string RoomNumber { get; set; } = string.Empty;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Nights { get; set; }
        public decimal Revenue { get; set; }
        public string GuestNames { get; set; } = string.Empty;
    }
}