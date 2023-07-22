using API.Helpers;
using API.Repos.Dtos;
using API.Repos.Interfaces;
using API.Repos.Models;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace API.Repos.Services
{
    public class AccountService : IAccountRepository
    {
        private readonly IRegisterRepository _registerRepository;

        public AccountService(IRegisterRepository registerRepository, IRegisterRepository registerRepository1)
        {
            _registerRepository = registerRepository;
        }

        public async Task<RegistrationResult> RegisterUser(CreateUserDto createUserDto)
        {
            if (createUserDto == null)
            {
                return RegistrationResult.Error("User data is empty!");
            }

            var existingUser = await _registerRepository.GetUserByEmailOrNicOrContactNo(createUserDto.Email, createUserDto.Nic, createUserDto.ContactNo, createUserDto.CustName);

            if (existingUser != null)
            {
                return RegistrationResult.Error("User already exists with this information!");
            }

            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(createUserDto.CustPassword));
                var encryptedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                createUserDto.CustPassword = encryptedPassword;
            }

            string refreshToken = TokenHelpers.GenerateToken();

            var newUser = new Tblregister
            {
                ContactNo = createUserDto.ContactNo,
                Nic = createUserDto.Nic,
                AccountBalance = createUserDto.AccountBalance,
                AddOn = DateTime.UtcNow,
                AlternatePhone = createUserDto.AlternatePhone,
                CustAddress = createUserDto.CustAddress,
                CustName = createUserDto.CustName,
                CustPassword = createUserDto.CustPassword,
                CustStatus = 1,
                Email = createUserDto.Email,
                Mobile = createUserDto.Mobile,
                Otp = "This will be OTP",
                Photo = "",
                Hash = refreshToken
            };

            await _registerRepository.AddUser(newUser);

            return RegistrationResult.Success(newUser);
        }

        public async Task<LoginResult> LoginUser(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return LoginResult.Error("User data is empty!");
            }

            var existingUser = await _registerRepository.GetUserByUsername(loginDto.Username);

            if (existingUser == null)
            {
                return LoginResult.Error("User does not exist!");
            }

            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
                var enteredPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                if (enteredPassword != existingUser.CustPassword)
                {
                    return LoginResult.Error("Invalid password!");
                }
            }

            existingUser.Hash = EncodeValue(existingUser.Id);
            await _registerRepository.UpdateUser(existingUser);

            return LoginResult.Success(existingUser.CustName, existingUser.Email, existingUser.Hash);
        }

        private string EncodeValue(int userId)
        {
            string datetimeString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            string valueToEncode = $"{userId}!{datetimeString}";

            byte[] bytes = Encoding.UTF8.GetBytes(valueToEncode);
            string encodedValue = Convert.ToBase64String(bytes);

            return encodedValue;
        }
    }
}
