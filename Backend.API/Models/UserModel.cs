
using System.ComponentModel.DataAnnotations;

namespace Backend.API.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string First_name { get; set; } = string.Empty;
        public string Last_name { get; set; } = string.Empty;
        public string Phone_number { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

    }
}
