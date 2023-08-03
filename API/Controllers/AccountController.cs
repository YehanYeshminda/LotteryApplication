using API.Helpers;
using API.Repos.Dtos;
using API.Repos.Interfaces;
using API.Models;
using API.Repos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRegisterRepository _registerRepository;
        private readonly GlobalDataService _globalDataService;
        private readonly LotteryContext _lotteryContext;
        private readonly TwillioSettings _twilioSettings;

        string url = "https://verify.twilio.com/v2/Services/VAc057cb0dc538d2255ff47cbf76e10b3f/Verifications";

        public AccountController(IAccountRepository accountRepository, IRegisterRepository registerRepository, IOptions<TwillioSettings> twilioSettingsOptions, GlobalDataService globalDataService,LotteryContext lotteryContext)
        {
            _accountRepository = accountRepository;
            _registerRepository = registerRepository;
            _globalDataService = globalDataService;
            _lotteryContext = lotteryContext;
            _twilioSettings = twilioSettingsOptions.Value;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tblregister>>> GetUsers()
        {
            var users = await _registerRepository.GetAllUsers();

            if (users == null)
            {
                return NotFound("Unable to find users!");
            }

            return Ok(users);
        }

        [HttpPost("Register")]
        public async Task<ActionResult<RegistrationResult>> RegisterUser(CreateUserDto createUserDto)
        {
            var registrationResult = await _accountRepository.RegisterUser(createUserDto);

            if (registrationResult.IsSuccess)
            {
                return Ok(registrationResult.User);
            }
            else
            {
                return BadRequest(registrationResult.ErrorMessage);
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginReponseDto>> LoginUser(LoginDto loginDto)
        {
            var loginResult = await _accountRepository.LoginUser(loginDto);

            if (loginResult.IsSuccess)
            {
                return Ok(new LoginReponseDto
                {
                    Username = loginResult.Username,
                    Email = loginResult.Email,
                    Hash = loginResult.Hash,
                });
            }
            else
            {
                return BadRequest(loginResult.ErrorMessage);
            }
        }

        public class GetSentOtpResponse 
        {
            public string Number { get; set; }
        }

        [HttpPost("SendOTP")]
        public async Task<ActionResult<GetSentOtpResponse>> SendOTP(SendOtpDto sendOtpDto)
        {
            string secret = _twilioSettings.TWILIO_AUTH_TOKEN;
            string credential = _twilioSettings.TWILIO_ACCOUNT_SID;

            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(credential, secret);
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                NameValueCollection parameters = new NameValueCollection();
                parameters["To"] = sendOtpDto.PhoneNumber;
                parameters["Channel"] = sendOtpDto.Method;
                _globalDataService.PhoneNumber = sendOtpDto.PhoneNumber;

                byte[] responseBytes = client.UploadValues(url, "POST", parameters);
                string responseString = Encoding.UTF8.GetString(responseBytes);
            }

            return Ok(new GetSentOtpResponse { Number  = sendOtpDto.PhoneNumber });

        }

        public class SendOtpVerification
        {
            public string OTP { get; set; }
        }

        [HttpPost("VerifyOTP")]
        public async Task<ActionResult<ValuesDto>> CheckOTPValid(SendOtpVerification sendOtpVerification)
        {
            string secret = _twilioSettings.TWILIO_AUTH_TOKEN;
            string credential = _twilioSettings.TWILIO_ACCOUNT_SID;
            string checkUrl = "https://verify.twilio.com/v2/Services/VAc057cb0dc538d2255ff47cbf76e10b3f/VerificationCheck";
            var newValues = new ValuesDto();
            newValues.Status = false;

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(credential, secret);
                    client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                    NameValueCollection checkParameters = new NameValueCollection();

                    if (_globalDataService.PhoneNumber == null || _globalDataService.PhoneNumber == "")
                    {
                        return BadRequest("Phone number not found!");
                    }

                    checkParameters["To"] = _globalDataService.PhoneNumber;
                    checkParameters["Code"] = sendOtpVerification.OTP;

                    byte[] checkResponseBytes;
                    try
                    {
                        checkResponseBytes = client.UploadValues(checkUrl, "POST", checkParameters);
                    }
                    catch (WebException ex)
                    {
                        if (ex.Response is HttpWebResponse response && response.StatusCode == HttpStatusCode.NotFound)
                        {
                            return Ok(newValues);
                        }
                        return Ok(newValues);
                    }

                    string checkResponseString = Encoding.UTF8.GetString(checkResponseBytes);
                    newValues.Status = checkResponseString.Contains("approved");
                    return Ok(newValues);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error while verying the OTP with the error! " + ex.Message);
            }
        }

        [HttpPost("GetUserInfo")]
        public async Task<ActionResult<GetUserInformationDto>> GetUserInformation([FromBody]AuthDto authDto)
        {
            if (authDto.Hash == null)
            {
                return Unauthorized("Missing Authentication Details");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(authDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == authDto.Hash);

            if (_user == null)
            {
                return Unauthorized("Invalid Authentication Details");
            }

            var decryptedDateWithOffset = decodedValues.Date.AddDays(1);
            var currentDate = DateTime.UtcNow.Date;

            if (currentDate < decryptedDateWithOffset.Date)
            {

                var user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Email == _user.Email);

                if (user == null) return null;

                var userInfo = new GetUserInformationDto
                {
                    AddOn = user.AddOn,
                    AlternatePhone = user.AlternatePhone,
                    ContactNo = user.ContactNo,
                    CustAddress = user.CustAddress,
                    CustName = user.CustName,
                    Email = user.Email,
                    Mobile = user.Mobile,
                    Nic = user.Nic,
                    AccountBalance = user.AccountBalance
                };

                return Ok(userInfo);
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }

        [HttpPost("UpdateUserInfo")]
        public async Task<ActionResult<GetUserInformationDto>> UpdateUserInformation([FromBody] UpdateUserInformationDto data)
        {
            if (data.AuthDto.Hash == null)
            {
                return Unauthorized("Missing Authentication Details");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(data.AuthDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == data.AuthDto.Hash);

            if (_user == null)
            {
                return Unauthorized("Invalid Authentication Details");
            }

            var decryptedDateWithOffset = decodedValues.Date.AddDays(1);
            var currentDate = DateTime.UtcNow.Date;

            if (currentDate < decryptedDateWithOffset.Date)
            {
                try
                {
                    var user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Email == _user.Email);

                    var existingUser = await _registerRepository.GetUserByNicOrContactNo(data.Nic, data.ContactNo, data.CustName);

                    if (existingUser != null)
                    {
                        return null;
                    }

                    if (user == null) return null;

                    user.CustName = data.CustName;
                    user.Nic = data.Nic;
                    user.Email = data.Email;
                    user.CustAddress = data.CustAddress;
                    user.Mobile = data.Mobile;
                    user.AlternatePhone = data.AlternatePhone;
                    user.ContactNo = data.ContactNo;

                    _lotteryContext.Tblregisters.Update(user);
                    await _lotteryContext.SaveChangesAsync();

                    var userInfo = new GetUserInformationDto
                    {
                        AddOn = user.AddOn,
                        AlternatePhone = user.AlternatePhone,
                        ContactNo = user.ContactNo,
                        CustAddress = user.CustAddress,
                        CustName = user.CustName,
                        Email = user.Email,
                        Mobile = user.Mobile,
                        Nic = user.Nic,
                    };

                    return Ok(userInfo);

                }
                catch (Exception ex)
                {
                    return BadRequest("Error while updating user! " + ex.Message);
                }
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }

        [HttpPost("GetSingleUserInfo")]
        public async Task<IActionResult> GetSingleUserInfo(AuthDto authDto)
        {
            if (authDto.Hash == null)
            {
                return Unauthorized("Missing Authentication Details");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(authDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == authDto.Hash);

            if (_user == null)
            {
                return Unauthorized("Invalid Authentication Details");
            }

            var decryptedDateWithOffset = decodedValues.Date.AddDays(1);
            var currentDate = DateTime.UtcNow.Date;

            if (currentDate < decryptedDateWithOffset.Date)
            {
                return Ok(_user);
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }
    }
}
