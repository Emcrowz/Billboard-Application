using System.ComponentModel.DataAnnotations;

namespace Billboard_BackEnd.Models;

public class Vehicle 
{
    [Key, Display(Name = "Vehicle ID")]
    public int VehicleId { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public decimal Price { get; set; }
    [Display(Name = "Creation Date")]
    public DateTime CreationDate { get; set; }
    [Display(Name = "Last Technical Check")]
    public DateTime LastTechnicalCheck { get; set; }
}
