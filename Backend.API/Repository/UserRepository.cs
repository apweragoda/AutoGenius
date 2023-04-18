using Backend.API.Contracts;
using Backend.API.Helpers;
using Backend.API.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Net;

namespace Backend.API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context =  context;
        }

        public async Task<UserModel> GetUser(int id)
        {
            var procedureName = "sp_GetUserById";
            var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int32, ParameterDirection.Input);
                using (var connection = _context.CreateConnection())
            {
                var user = await connection.QueryFirstOrDefaultAsync<UserModel>
                    (procedureName, parameters, commandType: CommandType.StoredProcedure);
                return user;
            }
        }

        public async Task<IEnumerable<UserModel>> GetUsers()
        {
            var query = "sp_GetAllUsers";
            using var connection = _context.CreateConnection();
            var users = await connection.QueryAsync<UserModel>(query);
            return users.ToList();
        }

        public async Task<UserModel> CheckLogin(string email, string password)
        {
            var procedureName = "sp_CheckLogin";
            var parameters = new DynamicParameters();
            parameters.Add("Email", email, DbType.String, ParameterDirection.Input);
            parameters.Add("Password", password, DbType.String, ParameterDirection.Input);
        
            using (var connection = _context.CreateConnection())
            {
                var user = await connection.QueryFirstOrDefaultAsync<UserModel>
                    (procedureName, parameters, commandType: CommandType.StoredProcedure);
                return user;
            }
        }

        public async Task<UserModel> Register(UserModel user)
        {
            var procedureName = "sp_InsertUser";
            var parameters = new DynamicParameters();
            parameters.Add("email", user.Email, DbType.String, ParameterDirection.Input);
            parameters.Add("password", user.Password, DbType.String, ParameterDirection.Input);
            parameters.Add("username", user.Username, DbType.String, ParameterDirection.Input);
            parameters.Add("first_name", user.First_name, DbType.String, ParameterDirection.Input);
            parameters.Add("last_name", user.Last_name, DbType.String, ParameterDirection.Input);
            parameters.Add("phone_number", user.Phone_number, DbType.String, ParameterDirection.Input);
            parameters.Add("address", user.Address, DbType.String, ParameterDirection.Input);

            using var connection = _context.CreateConnection();
            UserModel userDb = await connection.QueryFirstOrDefaultAsync<UserModel>
                (procedureName, parameters, commandType: CommandType.StoredProcedure);
            return userDb;
        }


        public async Task<UserModel> UpdateUser(UserModel user)
        {
            var procedureName = "sp_UpdateUser";
            var parameters = new DynamicParameters();
            parameters.Add("id", user.Id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("email", user.Email, DbType.String, ParameterDirection.Input);
            parameters.Add("password", user.Password, DbType.String, ParameterDirection.Input);
            parameters.Add("username", user.Username, DbType.String, ParameterDirection.Input);
            parameters.Add("first_name", user.First_name, DbType.String, ParameterDirection.Input);
            parameters.Add("last_name", user.Last_name, DbType.String, ParameterDirection.Input);
            parameters.Add("phone_number", user.Phone_number, DbType.String, ParameterDirection.Input);
            parameters.Add("address", user.Address, DbType.String, ParameterDirection.Input);

            using (var connection = _context.CreateConnection())
            {
                var userDb = await connection.QueryFirstOrDefaultAsync<UserModel>
                    (procedureName, parameters, commandType: CommandType.StoredProcedure);
                return userDb;
            }
        }

        public async Task<UserModel> DeleteUser(string email)
        {
            var procedureName = "sp_DeleteUser";
            var parameters = new DynamicParameters();
            parameters.Add("Email", email, DbType.String, ParameterDirection.Input);
            using (var connection = _context.CreateConnection())
            {
                var user = await connection.QueryFirstOrDefaultAsync<UserModel>
                    (procedureName, parameters, commandType: CommandType.StoredProcedure);
                return user;
            }
        }

    }
}
