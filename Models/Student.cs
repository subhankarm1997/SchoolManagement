using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        [Required]
        [MaxLength(256)]
        public string FullName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserType { get; set; } // You can use enum for better structure


        public string? ProfileImageUrl { get; set; }
    }
}

