using Backend.API.Models;

namespace Backend.API.Contracts
{
    public interface IVehicleRepository
    {
        Task<IEnumerable<VehicleModel>> GetAllVehicles();
        Task<VehicleModel> GetVehicleByUserId(int id);        
        Task<VehicleModel> AddVehicle(VehicleModel vehicle);
        Task<VehicleModel> UpdateVehicle(VehicleModel vehicle);
        Task<VehicleModel> DeleteVehicle(string vin);
    }
}
