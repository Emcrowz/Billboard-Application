using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Billboard_BackEnd.Models
{
    public class CarListing : BillboardListing
    {
        [Key, Display(Name = "Car Listing ID")]
        public int CarListingId { get; set; }
        [Display(Name = "Cars Listed")]
        public string CarsListed { get; set; } = string.Empty;
        [JsonIgnore, Display(Name = "Car List")]
        public List<Car> CarList { get; set; } = [];
    }
}
