namespace Team_12.Models
{
    public class PaymentNotification
    {
        public string PaymentReference { get; set; }  // This is PayFast's m_payment_id
        public string PaymentStatus { get; set; }  // Status from PayFast (COMPLETE, PENDING, etc.)
        public decimal Amount { get; set; }  // The payment amount
    }
}
