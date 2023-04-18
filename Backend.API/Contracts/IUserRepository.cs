using Backend.API.Models;

namespace Backend.API.Contracts
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserModel>> GetUsers();
        Task<UserModel> GetUser(int id);
        Task<UserModel> CheckLogin(string email, string password);
        Task<UserModel> Register(UserModel user);
        Task<UserModel> UpdateUser(UserModel user);
        Task<UserModel> DeleteUser(string email);

    }
}
