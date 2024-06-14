using System.ComponentModel.DataAnnotations;

namespace Billboard_BackEnd.Models
{
    public class CarListing : BillboardListing
    {
        [Key]
        [Display(Name = "Car Listing ID")]
        public int CarListingId { get; set; }
        [Display(Name = "Listed Car")]
        public Car? ListedCar { get; set; } 
    }
}
