using System.ComponentModel.DataAnnotations;

namespace Billboard_BackEnd.Models
{
    public class BillboardListing
    {
        [Key, Display(Name = "Listing ID")]
        public int ListingId { get; set; }
        
        [Required, Display(Name = "Vehicle ID")]
        public int VehicleId { get; set; }

        [Required, Display(Name = "User ID")]
        public int UserId { get; set; }

        [MaxLength(25)]
        public string ListingType { get; set; } = string.Empty; // Will derive from Vehicle child object name.
    }
}
