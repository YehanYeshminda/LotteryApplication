using API.Helpers;
using API.Repos;
using API.Repos.Dtos;
using API.Repos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly LotteryContext _lotteryContext;

        public AccountController(LotteryContext lotteryContext)
        {
            _lotteryContext = lotteryContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tblregister>>> GetUsers()
        {
            var users = await _lotteryContext.Tblregisters.ToListAsync();

            if (users == null)
            {
                return NotFound("Unable to find users!");
            }

            return users;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<Tblregister>> RegisterUser(CreateUserDto createUserDto)
        {   
            if (createUserDto == null)
            {
                return BadRequest("User data is empty!");
            }

            var existingUser = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Email == createUserDto.Email || x.Nic == createUserDto.Nic || x.ContactNo == createUserDto.ContactNo || x.CustName == createUserDto.CustName);

            if (existingUser != null)
            {
                return BadRequest("User already exist with this information!");
            };

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

            _lotteryContext.Tblregisters.Add(newUser);
            await _lotteryContext.SaveChangesAsync();

            return Ok(existingUser);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginReponseDto>> LoginUser(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return BadRequest("User data is empty!");
            }

            var existingUser = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.CustName == loginDto.Username);

            if (existingUser == null)
            {
                return BadRequest("User does not exist!");
            }

            // Decrypt and compare the passwords
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
                var enteredPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                if (enteredPassword != existingUser.CustPassword)
                {
                    return BadRequest("Invalid password!");
                }
            }

            if (existingUser.CustName != loginDto.Username)
            {
                return BadRequest("Invalid Username!");
            }

            existingUser.Hash = TokenHelpers.GenerateToken();
            _lotteryContext.Tblregisters.Update(existingUser);
            await _lotteryContext.SaveChangesAsync();

            return Ok(new LoginReponseDto
            {
                Username = existingUser.CustName,
                Email = existingUser.Email,
                Hash = existingUser.Hash
            });
        }

    }
}
