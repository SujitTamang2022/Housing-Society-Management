namespace HousingSManagement.Models
{
    public class Complaint
    {
        public int Id { get; set; }
        public int ResidentId { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsResolved { get; set; }
        public Resident? Resident { get; set; }
    }
}