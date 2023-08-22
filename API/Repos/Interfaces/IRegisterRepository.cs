using API.Models;

namespace API.Repos.Interfaces
{
    public interface IRegisterRepository
    {
        Task<IEnumerable<Tblregister>> GetAllUsers();
        Task<Tblregister> GetUserByEmailOrNicOrContactNo(string email, string nic, string contactNo, string custName);
        Task<Tblregister> GetUserByNicOrContactNo(string nic, string contactNo, string custName);
        Task<Tblregister> GetUserByNicOremail(string nic, string email);
        Task<Tblregister> GetUserByUsername(string username);
        Task<Tblregister> GetUserByPhoneNo(string phoneNo);
        Task AddUser(Tblregister user);
        Task UpdateUser(Tblregister user);
        Task<Tblregister> GetUserById(int id);
        Task SaveChangesAsync();
    }
}
