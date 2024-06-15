using System.ComponentModel.DataAnnotations;

namespace Billboard_BackEnd.Models;

public class Vehicle 
{
    [Key, Display(Name = "Vehicle ID")]
    public int VehicleId { get; set; }
    [MaxLength(50)]
    public string Make { get; set; } = string.Empty;
    [MaxLength(50)]
    public string Model { get; set; } = string.Empty;
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }
    [Display(Name = "Creation Date")]
    public DateTime CreationDate { get; set; }
    [Display(Name = "Last Technical Check")]
    public DateTime LastTechnicalCheck { get; set; }
}
