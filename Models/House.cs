using System.ComponentModel.DataAnnotations;

namespace HousingSManagement.Models
{
    public class House
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string OwnerName { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string HouseNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Block { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
