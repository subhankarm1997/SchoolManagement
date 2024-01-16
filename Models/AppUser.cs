using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Net;

namespace SchoolManagement.Models
{
    public class AppUser : IdentityUser
    {

        [Required]
        [MaxLength(256)]
        public string FullName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [EmailAddress]
        public override string Email { get; set; }

        [Required]
        public string UserType { get; set; } // You can use enum for better structure

        
        public string? ProfileImageUrl { get; set; }
    }
}
