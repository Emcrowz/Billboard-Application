using System.ComponentModel.DataAnnotations;

namespace Billboard_BackEnd.Models
{
    public class MotorbikeListing : BillboardListing
    {
        [Key]
        [Display(Name = "Motorbike Listing ID")]
        public int MotorbikeListingId { get; set; }
        [Display(Name = "Listed Motorbike")]
        public Motorbike? ListedMotorbike { get; set; }
    }
}
