using System.ComponentModel.DataAnnotations;

namespace Billboard_BackEnd.Models
{
    public class BillboardListing
    {
        [Key]
        [Display(Name = "Listing ID")]
        public int ListingId { get; set; }
        [Display(Name = "Vehicle Listings")]
        List<Vehicle> VeicleListings { get; set; } = [];
    }
}
