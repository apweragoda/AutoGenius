using Backend.API.Models;

namespace Backend.API.Contracts
{
    public interface IUserRepository
    {
        public Task<IEnumerable<UserModel>> GetUsers();
        public Task<UserModel> GetUser(int id);
        public Task<UserModel> CheckLogin(string email, string password);
    }
}
