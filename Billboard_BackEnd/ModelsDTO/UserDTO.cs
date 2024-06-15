using System.ComponentModel.DataAnnotations;

namespace Billboard_BackEnd.ModelsDTO
{
    public class UserDTO
    {
        [Display(Name = "First Name"), MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name"), MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(5), MaxLength(25)]
        public string Username { get; set; } = string.Empty;

        [Required, MinLength(8), MaxLength(25)]
        public string Password { get; set; } = string.Empty;
    }
}
