using API.Repos.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repos.Services
{
    public class RegisterService : IRegisterRepository
    {
        private readonly LotteryContext _lotteryContext;

        public RegisterService(LotteryContext lotteryContext)
        {
            _lotteryContext = lotteryContext;
        }

        public async Task<IEnumerable<Tblregister>> GetAllUsers()
        {
            return await _lotteryContext.Tblregisters.ToListAsync();
        }

        public async Task<Tblregister> GetUserByEmailOrNicOrContactNo(string email, string nic, string contactNo, string custName)
        {
            var user = await _lotteryContext.Tblregisters
                .FirstOrDefaultAsync(x => x.Email == email || x.Nic == nic || x.ContactNo == contactNo || x.CustName == custName);

            if (user != null)
            {
                return user;
            }

            return null;
        }

        public async Task<Tblregister> GetUserByNicOrContactNo(string nic, string contactNo, string custName)
        {
            var user = await _lotteryContext.Tblregisters
                .FirstOrDefaultAsync(x => x.Nic == nic || x.ContactNo == contactNo || x.CustName == custName);

            if (user != null)
            {
                return user;
            }

            return null;
        }

        public async Task<Tblregister> GetUserByUsername(string username)
        {
            var user = await _lotteryContext.Tblregisters
                .FirstOrDefaultAsync(x => x.CustName == username);

            if (user != null)
            {
                return user;
            }

            return null;
        }

        public async Task AddUser(Tblregister user)
        {
            _lotteryContext.Tblregisters.Add(user);
            await SaveChangesAsync();
        }

        public async Task UpdateUser(Tblregister user)
        {
            _lotteryContext.Tblregisters.Update(user);
            await SaveChangesAsync();
        }

        public async Task<Tblregister> GetUserById(int id)
        {
            return await _lotteryContext.Tblregisters.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _lotteryContext.SaveChangesAsync();
        }
    }
}
