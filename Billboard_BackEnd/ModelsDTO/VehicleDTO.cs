using Billboard_BackEnd.Models;
using System.ComponentModel.DataAnnotations;

namespace Billboard_BackEnd.ModelsDTO
{
    public class VehicleDTO
    {
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
    }
}
