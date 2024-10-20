namespace Team_12.DTOs
{
    public class BookingResponseDto
    {
        public int BookingId { get; set; }
        public string Status { get; set; }
        public decimal TotalCost { get; set; }
        public decimal DiscountApplied { get; set; }
        public decimal FinalPrice { get; set; }
        public string PaymentUrl { get; set; }
    }
}
