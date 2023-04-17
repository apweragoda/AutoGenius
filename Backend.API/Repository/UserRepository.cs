using Backend.API.Contracts;
using Backend.API.Helpers;
using Backend.API.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
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
            using (var connection = _context.CreateConnection())
            {
                var users = await connection.QueryAsync<UserModel>(query);
                return users.ToList();
            }
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


    }
}
