using API.Repos.Dtos;

namespace API.Repos.Interfaces
{
    public interface IAccountRepository
    {
        Task<RegistrationResult> RegisterUser(CreateUserDto createUserDto);
        Task<LoginResult> LoginUser(LoginDto loginDto);
    }
}
