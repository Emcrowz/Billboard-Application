using Billboard_BackEnd.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Billboard_BackEnd.ModelsDTO
{
    public class BillboardListingDTO
    {
        [Key, JsonIgnore, Display(Name = "Listing ID")]
        public int ListingId { get; set; }

        [Display(Name = "Listed Item Type")]
        public string ListingType { get; set; } = string.Empty;

        // User Info
        [JsonIgnore]
        public int UserId { get; set; }

        [Display(Name = "First Name"), MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name"), MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        // Vehicle Info
        [JsonIgnore]
        public int VehicleId { get; set; }

        [MaxLength(50)]
        public string Make { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Model { get; set; } = string.Empty;

        [DataType(DataType.Currency)]
        public decimal Price { get; set; } = 0;

        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; }

        [Display(Name = "Last Technical Check")]
        public DateTime LastTechnicalCheck { get; set; }

        [Display(Name = "Door Count")]
        public int DoorCount { get; set; }

        [Range(0, 18)]
        public EngineType Engine { get; set; }

        [Display(Name = "Cylinder Volume")]
        public int CylinderVolume { get; set; }

        [BsonId]
        public ObjectId InternalBillboardListingId { get; set; }
    }
}
