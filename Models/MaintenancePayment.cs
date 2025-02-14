namespace HousingSManagement.Models
{
    public class MaintenancePayment
    {
        public int Id { get; set; }
        public int ResidentId { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime DueDate { get; set; }
        public Resident? Resident { get; set; }  
    }
}