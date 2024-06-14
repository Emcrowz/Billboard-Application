using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Billboard_BackEnd.Models
{
    public class Car : Vehicle
    {
        [Key, JsonIgnore, Display(Name = "Car ID")]
        public int CarId { get; set; }
        [Display(Name = "Door Count")]
        public int DoorCount { get; set; }
        [Range(0,17)]
        public EngineType Engine { get; set; }
    }
}
