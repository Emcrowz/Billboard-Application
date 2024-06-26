﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Billboard_BackEnd.Models
{
    public class User
    {
        [Key, Display(Name = "User ID")]
        public int UserId { get; set; }

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

        [JsonIgnore, Range(0,2)]
        public UserType UserCategory { get; set; } = 0;
    }
}
