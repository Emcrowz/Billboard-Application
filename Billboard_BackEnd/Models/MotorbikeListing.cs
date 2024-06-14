using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Billboard_BackEnd.Models
{
    public class MotorbikeListing : BillboardListing
    {
        [Key, Display(Name = "Motorbike Listing ID")]
        public int MotorbikeListingId { get; set; }
        [Display(Name = "Motorbikes Listed")]
        public string MotorbikesListed { get; set; } = string.Empty;
        [JsonIgnore, Display(Name = "Motorbike List")]
        public List<Motorbike> MotorbikeList { get; set; } = [];
    }
}
