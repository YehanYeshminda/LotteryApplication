using API.Helpers;
using API.Repos.Dtos;
using API.Repos.Interfaces;
using API.Repos.Models;
using API.Repos.Services;
using Microsoft.AspNetCore.Mvc;
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
        private readonly TwillioSettings _twilioSettings;

        string url = "https://verify.twilio.com/v2/Services/VAc057cb0dc538d2255ff47cbf76e10b3f/Verifications";
        string checkUrl = "https://verify.twilio.com/v2/Services/VAc057cb0dc538d2255ff47cbf76e10b3f/VerificationCheck";

        public AccountController(IAccountRepository accountRepository, IRegisterRepository registerRepository, IOptions<TwillioSettings> twilioSettingsOptions, GlobalDataService globalDataService)
        {
            _accountRepository = accountRepository;
            _registerRepository = registerRepository;
            _globalDataService = globalDataService;
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
                    Hash = loginResult.Hash
                });
            }
            else
            {
                return BadRequest(loginResult.ErrorMessage);
            }
        }

        [HttpPost("SendOTP")]
        public async Task<ActionResult> SendOTP(SendOtpDto sendOtpDto)
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

            return Ok("OTP has been sent for the number: " + sendOtpDto.PhoneNumber);
        }


        [HttpPost("VerifyOTP")]
        public async Task<ActionResult<ValuesDto>> CheckOTPValid(string OTP)
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
                    checkParameters["Code"] = OTP;

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

    }
}
