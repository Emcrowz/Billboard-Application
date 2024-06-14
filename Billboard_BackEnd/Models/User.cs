using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Billboard_BackEnd.Models
{
    public class User
    {
        [Key, Display(Name = "User ID")]
        public int UserId { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = "NO NAME";
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = "NO SURNAME";
        [Required, MinLength(5)]
        public string Username { get; set; } = string.Empty;
        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;
        [JsonIgnore, Range(0,2)]
        public UserType UserCategory { get; set; } = 0;

        [Display(Name = "Vehicle Listing IDs")]
        public string VehicleListingIds { get; set; } = string.Empty;

        [JsonIgnore, Display(Name = "User Billboard Listings")]
        public List<BillboardListing> UserListings = [];

        [BsonId]
        public ObjectId InternalUserId { get; set; }
    }
}
