using API.Helpers;
using API.Repos.Dtos;
using API.Repos.Interfaces;
using API.Models;
using System.Text;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace API.Repos.Services
{
    public class AccountService : IAccountRepository
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly LotteryContext _lotteryContext;

        public AccountService(IRegisterRepository registerRepository, LotteryContext lotteryContext)
        {
            _registerRepository = registerRepository;
            _lotteryContext = lotteryContext;
        }

        public async Task<RegistrationResult> RegisterUser(CreateUserDto createUserDto)
        {
            if (createUserDto == null)
            {
                return RegistrationResult.Error("User data is empty!");
            }

            var existingUserWithEmail = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Email == createUserDto.Email);

            if (existingUserWithEmail != null)
            {
                return RegistrationResult.Error("User already exists with Email!");
            }

            var existingUserName = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.CustName == createUserDto.CustName);

            if (existingUserName != null)
            {
                return RegistrationResult.Error("User already exists with this username!");
            }

            var existingUserNic = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Nic == createUserDto.Nic);

            if (existingUserNic != null)
            {
                return RegistrationResult.Error("User already exists with this NIC!");
            }

            var existingUserContactNo = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.ContactNo == createUserDto.ContactNo);

            if (existingUserContactNo != null)
            {
                return RegistrationResult.Error("User already exists with this contact No!");
            }

            var existingUserContactMobile = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Mobile == createUserDto.ContactNo);

            if (existingUserContactMobile != null)
            {
                return RegistrationResult.Error("User already exists with this contact No!");
            }

            createUserDto.CustPassword = GetHashedPassword(createUserDto.CustPassword);

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
                Mobile = createUserDto.ContactNo,
                Otp = "This will be OTP",
                Photo = "",
                Hash = refreshToken,
                Role = createUserDto.Role,
                Dob = createUserDto.Dob
            };

            await _registerRepository.AddUser(newUser);
            return RegistrationResult.Success(newUser);
        }

        private string GetHashedPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
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


            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, existingUser.CustPassword))
            {
                return LoginResult.Error("Invalid password!");
            }
            
            existingUser.Hash = EncodeValue(existingUser.Id);
            await _registerRepository.UpdateUser(existingUser);

            return LoginResult.Success(existingUser.CustName, existingUser.Email, existingUser.Hash, existingUser.Role, existingUser.ContactNo);
        }

        private string EncodeValue(int userId)
        {
            string datetimeString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            string valueToEncode = $"{userId}!{datetimeString}";

            byte[] bytes = Encoding.UTF8.GetBytes(valueToEncode);
            string encodedValue = Convert.ToBase64String(bytes);

            return encodedValue;
        }

        public async Task<GetUserInformationDto> GetUserInfoByEmail(string email)
        {
            var availableUserInfo = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Email == email);

            if (availableUserInfo == null)
            {
                return null;
            }

            var userToReturn = new GetUserInformationDto
            {
                Email = availableUserInfo.Email,
                AddOn = availableUserInfo.AddOn,
                AlternatePhone = availableUserInfo.AlternatePhone,
                ContactNo = availableUserInfo.ContactNo,
                CustAddress = availableUserInfo.CustAddress,
                CustName = availableUserInfo.CustName,
                Mobile = availableUserInfo.Mobile,
                Nic = availableUserInfo.Nic
            };

            return userToReturn;
        }

        //public async Task<GetUserInformationDto> UpdateUserInfoByEmail
    }
}
