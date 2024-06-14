using System.ComponentModel.DataAnnotations;

namespace Billboard_BackEnd.Models
{
    public class User
    {
        [Key]
        [Display(Name = "User ID")]
        public int UserId { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = "NO NAME";
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = "NO SURNAME";
        
        public string Password { get; set; } = string.Empty;

        public UserType UserCategory { get; set; } = 0;

        [Display(Name = "User Listings")]
        public List<BillboardListing> UserListings = [];
    }
}
