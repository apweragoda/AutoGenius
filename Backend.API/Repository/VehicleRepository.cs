using Backend.API.Contracts;
using Backend.API.Helpers;
using Backend.API.Models;
using Dapper;
using System.Data;

namespace Backend.API.Repository
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly DataContext _context;

        public VehicleRepository(DataContext context)
        {
            _context =  context;
        }
        public async Task<IEnumerable<VehicleModel>> GetAllVehicles()
        {
            var query = "sp_GetAllVehicles";
            using (var connection = _context.CreateConnection())
            {
                var vehicles = await connection.QueryAsync<VehicleModel>(query);
                return vehicles.ToList();
            }
        }

        public async Task<VehicleModel> GetVehicleByUserId(int id)
        {
            var procedureName = "sp_GetVehicleByUserId";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", id, DbType.Int32, ParameterDirection.Input);
            using (var connection = _context.CreateConnection())
            {
                var vehicle = await connection.QueryFirstOrDefaultAsync<VehicleModel>
                    (procedureName, parameters, commandType: CommandType.StoredProcedure);
                return vehicle;
            }
        }

        public async Task<VehicleModel> AddVehicle(VehicleModel vehicle)
        {
            var procedureName = "sp_InsertVehicle";
            var parameters = new DynamicParameters();
            parameters.Add("make", vehicle.Make, DbType.String, ParameterDirection.Input);
            parameters.Add("model", vehicle.Model, DbType.String, ParameterDirection.Input);
            parameters.Add("year", vehicle.Year, DbType.Int32, ParameterDirection.Input);
            parameters.Add("mileage", vehicle.Mileage, DbType.Int32, ParameterDirection.Input);
            parameters.Add("vin", vehicle.Vin, DbType.String, ParameterDirection.Input);
            parameters.Add("user_id", vehicle.User_id, DbType.Int32, ParameterDirection.Input);

            using var connection = _context.CreateConnection();
            var vehicleDb = await connection.QueryFirstOrDefaultAsync<VehicleModel>
                (procedureName, parameters, commandType: CommandType.StoredProcedure);
            return vehicleDb;

        }


        public async Task<VehicleModel> UpdateVehicle(VehicleModel vehicle)
        {
            var procedureName = "sp_UpdateVehicle";
            var parameters = new DynamicParameters();
            parameters.Add("id", vehicle.Id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("make", vehicle.Make, DbType.String, ParameterDirection.Input);
            parameters.Add("model", vehicle.Model, DbType.String, ParameterDirection.Input);
            parameters.Add("year", vehicle.Year, DbType.String, ParameterDirection.Input);
            parameters.Add("mileage", vehicle.Mileage, DbType.String, ParameterDirection.Input);
            parameters.Add("vin", vehicle.Vin, DbType.String, ParameterDirection.Input);
            parameters.Add("user_id", vehicle.User_id, DbType.String, ParameterDirection.Input);

            using (var connection = _context.CreateConnection())
            {
                var vehicleDb = await connection.QueryFirstOrDefaultAsync<VehicleModel>
                    (procedureName, parameters, commandType: CommandType.StoredProcedure);
                return vehicleDb;
            }
        }

        public async Task<VehicleModel> DeleteVehicle(string vin)
        {
            var procedureName = "sp_DeleteVehicle";
            var parameters = new DynamicParameters();
            parameters.Add("Vin", vin, DbType.String, ParameterDirection.Input);
            using (var connection = _context.CreateConnection())
            {
                var vehicle = await connection.QueryFirstOrDefaultAsync<VehicleModel>
                    (procedureName, parameters, commandType: CommandType.StoredProcedure);
                return vehicle;
            }
        }


    }
}
