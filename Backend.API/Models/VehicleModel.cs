namespace Backend.API.Models
{
    public class VehicleModel
    {
        public int Id { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public int? Mileage { get; set; }
        public string? Vin { get; set; }
        public int? User_id { get; set; }
    }
}
