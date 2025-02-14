namespace HousingSManagement.Models
{
    public class Visitor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
    }
}