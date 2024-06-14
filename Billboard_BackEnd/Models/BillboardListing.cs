using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Billboard_BackEnd.Models
{
    public class BillboardListing
    {
        [Key, Display(Name = "Listing ID")]
        public int ListingId { get; set; }
        [Display(Name = "Vehilce Listing ID")]
        public int VehicleListingId { get; set; }
        [Display(Name = "Cars Listed")]
        public string CarsListed { get; set; } = string.Empty;
        [Display(Name = "Motorbikes Listed")]
        public string MotorbikesListed { get; set; } = string.Empty;
        [JsonIgnore, Display(Name = "Vehicle Listings")]
        List<Vehicle> VehicleListings { get; set; } = [];
    }
}
