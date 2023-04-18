
namespace Backend.API.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string? Email { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string? Username { get; set; } = string.Empty;
        public string? First_name { get; set; } = string.Empty;
        public string? Last_name { get; set; } = string.Empty;
        public string? Phone_number { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;

    }
}
