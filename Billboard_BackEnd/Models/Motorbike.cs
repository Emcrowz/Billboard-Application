using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Billboard_BackEnd.Models
{
    public class Motorbike : Vehicle
    {
        [Key, JsonIgnore, Display(Name = "Motorbike ID")]
        public int MotorbikeId { get; set; }

        [Display(Name = "Cylinder Volume")]
        public int CylinderVolume { get; set; }
    }
}
