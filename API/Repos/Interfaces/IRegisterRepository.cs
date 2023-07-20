

using API.API.Repos.Models;

namespace API.Repos.Interfaces
{
    public interface IRegisterRepository
    {
        Task<IEnumerable<Tblregister>> GetAllUsers();
        Task<Tblregister> GetUserByEmailOrNicOrContactNo(string email, string nic, string contactNo, string custName);
        Task<Tblregister> GetUserByUsername(string username);
        Task AddUser(Tblregister user);
        Task UpdateUser(Tblregister user);
        Task<Tblregister> GetUserById(int id);
        Task SaveChangesAsync();
    }
}
