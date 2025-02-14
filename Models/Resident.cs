namespace HousingSManagement.Models
{
    public class Resident
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ApartmentNumber { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsOwner { get; set; } 
    }
}