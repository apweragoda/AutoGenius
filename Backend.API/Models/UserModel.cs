
using System.ComponentModel.DataAnnotations;

namespace Backend.API.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        public string? Username { get; set; } 
        public string First_name { get; set; } = string.Empty;
        public string Last_name { get; set; } = string.Empty;
        [Phone(ErrorMessage = "Invalid phone number")]
        public string Phone_number { get; set; } = string.Empty;
        [StringLength(100)] 
        public string Address { get; set; } = string.Empty;

    }
}
