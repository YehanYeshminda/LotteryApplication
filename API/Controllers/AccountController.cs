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
using Quartz;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRegisterRepository _registerRepository;
        private readonly GlobalDataService _globalDataService;
        private readonly LotteryContext _lotteryContext;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly TwillioSettings _twilioSettings;

        string url = "https://verify.twilio.com/v2/Services/VAc057cb0dc538d2255ff47cbf76e10b3f/Verifications";

        public AccountController(IAccountRepository accountRepository, IRegisterRepository registerRepository, IOptions<TwillioSettings> twilioSettingsOptions, GlobalDataService globalDataService,LotteryContext lotteryContext, ISchedulerFactory schedulerFactory)
        {
            _accountRepository = accountRepository;
            _registerRepository = registerRepository;
            _globalDataService = globalDataService;
            _lotteryContext = lotteryContext;
            _schedulerFactory = schedulerFactory;
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

        [HttpGet("nextexecution-easydraw")]
        public async Task<IActionResult> GetNextExecutionTime()
        {
            try
            {
                var scheduler = await _schedulerFactory.GetScheduler();
                var jobKey = new JobKey("EasyDrawJob");
                var triggerKey = new TriggerKey("EasyDrawTrigger", "MyTriggerGroup");

                var trigger = await scheduler.GetTrigger(triggerKey);
                var nextFireTime = trigger.GetNextFireTimeUtc();

                return Ok(nextFireTime?.ToLocalTime());
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("nextexecution-lotti")]
        public async Task<IActionResult> GetNextExecutionTimeLotti()
        {
            try
            {
                var scheduler = await _schedulerFactory.GetScheduler();
                var jobKey = new JobKey("LottiJob");
                var triggerKey = new TriggerKey("LotiTrigger", "MyTriggerGroup");

                var trigger = await scheduler.GetTrigger(triggerKey);
                var nextFireTime = trigger.GetNextFireTimeUtc();

                return Ok(nextFireTime?.ToLocalTime());
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("nextexecution-megadraw")]
        public async Task<IActionResult> GetNextExecutionTimeMega()
        {
            try
            {
                var scheduler = await _schedulerFactory.GetScheduler();
                var jobKey = new JobKey("MegaDrawJob");
                var triggerKey = new TriggerKey("MegaDrawTrigger", "MyTriggerGroup");

                var trigger = await scheduler.GetTrigger(triggerKey);
                var nextFireTime = trigger.GetNextFireTimeUtc();

                return Ok(nextFireTime?.ToLocalTime());
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
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

        [HttpPost("AssignUser")]
        public async Task<ActionResult> AssignUser(AssignUserToAdminDto model)
        {
            if (model.AuthDto.Hash == null)
            {
                return Unauthorized("Missing Authentication Details");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(model.AuthDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == model.AuthDto.Hash);

            if (_user == null)
            {
                return Unauthorized("Invalid Authentication Details");
            }

            var decryptedDateWithOffset = decodedValues.Date.AddDays(1);
            var currentDate = DateTime.UtcNow.Date;

            if (currentDate < decryptedDateWithOffset.Date)
            {
                if (_user.Role == "Admin")
                {
                    var getUserWithId = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Email == model.UserEmail);

                    if (getUserWithId == null)
                    {
                        return BadRequest("Unable to find user with this id");
                    }

                    getUserWithId.Role = model.Role;
                    _lotteryContext.Tblregisters.Update(getUserWithId);
                    await _lotteryContext.SaveChangesAsync();
                    return Ok();
                } else
                {
                    return BadRequest("You are not a admin!");
                }
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
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
                    Role = loginResult.Role,
                    Phone = loginResult.Phone
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


                    //var existingUser = await _registerRepository.GetUserByNicOrContactNo(data.Nic, data.ContactNo, data.CustName);

                    var existingUser = await _registerRepository.GetUserByNicOremail(data.Nic, data.Email);

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

        [HttpPost("SetAvatarNo")]
        public async Task<IActionResult> SetAvatarNo(SetAvatarDto setAvatarDto)
        {
            if (setAvatarDto.AuthDto.Hash == null)
            {
                return Unauthorized("Missing Authentication Details");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(setAvatarDto.AuthDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == setAvatarDto.AuthDto.Hash);

            if (_user == null)
            {
                return Unauthorized("Invalid Authentication Details");
            }

            var decryptedDateWithOffset = decodedValues.Date.AddDays(1);
            var currentDate = DateTime.UtcNow.Date;

            if (currentDate < decryptedDateWithOffset.Date)
            {
                _user.AvatarNo = setAvatarDto.AvatarNo;
                _lotteryContext.Tblregisters.Update(_user);
                await _lotteryContext.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }

        public class GetUserBalance
        {
            public string UserBalance { get; set; }
        }

        [HttpPost("GetUserBalance")]
        public async Task<ActionResult<GetUserBalance>> GetUserWalletBalance(AuthDto authDto)
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
                return Ok(new GetUserBalance
                {
                    UserBalance = _user.AccountBalance.ToString()
                });
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }

        [HttpGet("Userreport")]
        public async Task<ActionResult<Tblregister>> ReturnHtmlReport()
        {
            var users = await _lotteryContext.Tblregisters.Where(x => x.Role == "Customer").ToListAsync();


            string html = @"
            <html>
            <head>
                <title>User</title> 
                <style>
                     body { font-family: Tahoma; }
                        .header { display: flex; align-items: center; padding: 20px; background-color: #f5f5f5; }
                        .logo { width: 100px; height: 100px; }
                        .company-info { display: flex; flex-direction: column; margin-left: 10px; }
                        .company-name { font-size: 24px; font-weight: bold; margin-bottom: 10px; }
                        .address { font-size: 16px; margin-bottom: 10px; }
                        .telephone { font-size: 16px; }
                        .containerheader { text-align: center; }
                        table { width: 100%; border-collapse: collapse; }
                        td, th { padding: 8px; text-align: left; }
                        th { background-color: #f2f2f2; }
                        .auto-style5, .auto-style6, .auto-style4 { width: 150px; }
                        .border { border: 1px solid #ccc; }  
                </style>
            </head>
            <body>
                <div class='header' style='text-align:left;'>
                   
                    <div class='company-info'>
                        <div class='company-name'>ABCD CLUBS</div>
                        <div class='address'>Near North Villa</div>
                        <div class='telephone'>9087654</div>
                    </div>
                </div>
               
                 <div class='report-container'>
                        <table>
                            <tr>
                                <th class='border'>Sl No</th>
                                <th class='border'>Customer Name</th>
                                <th class='border'>NIC</th>
                                <th class='border'>Email</th>
                                <th class='border'>Address</th>
                                <th class='border'>Mobile</th>
                                <th class='border'>Alternate Number</th>
                                <th class='border'>Acc Balance</th>
                            </tr>";


            foreach (var items in users)
            {


                html += $@"
                                <tr>
                                    <td class='border'>{items.Id}</td>
                                    <td class='border'>{items.CustName}</td>
                                    <td class='border'>{items.Nic}</td>
                                    <td class='border'>{items.Email}</td>
                                    <td class='border'>{items.CustAddress}</td>
                                    <td class='border'>{items.Mobile}</td>
                                    <td class='border'>{items.AlternatePhone}</td>
                                    <td class='border'>{items.AccountBalance}</td>
                                </tr>
                                ";
            }

            html += $@"
                           
                </table>
                </div>
                </body>
                </html>";

            var response = new HtmlResponseDto { Content = html };

            return Ok(response);
        }


        [HttpGet("Adminreport")]
        public async Task<ActionResult<Tblregister>> ReturnAdminHtmlReport()
        {
            //var tableForReport = await _lotteryContext.Tblregisters.ToListAsync();

            //var company = await _lotteryContext.tbl.FirstOrDefaultAsync();
            var admins = await _lotteryContext.Tblregisters.Where(x => x.Role == "Admin").ToListAsync();


            string html = @"
            <html>
            <head>
                <title>User</title> 
                <style>
                     body { font-family: Tahoma; }
                        .header { display: flex; align-items: center; padding: 20px; background-color: #f5f5f5; }
                        .logo { width: 100px; height: 100px; }
                        .company-info { display: flex; flex-direction: column; margin-left: 10px; }
                        .company-name { font-size: 24px; font-weight: bold; margin-bottom: 10px; }
                        .address { font-size: 16px; margin-bottom: 10px; }
                        .telephone { font-size: 16px; }
                        .containerheader { text-align: center; }
                        table { width: 100%; border-collapse: collapse; }
                        td, th { padding: 8px; text-align: left; }
                        th { background-color: #f2f2f2; }
                        .auto-style5, .auto-style6, .auto-style4 { width: 150px; }
                        .border { border: 1px solid #ccc; }  
                </style>
            </head>
            <body>
                <div class='header' style='text-align:left;'>
                   
                    <div class='company-info'>
                        <div class='company-name'>ABCD CLUBS</div>
                        <div class='address'>Near North Villa</div>
                        <div class='telephone'>9087654</div>
                    </div>
                </div>
               
                 <div class='report-container'>
                        <table>
                            <tr>
                                <th class='border'>Sl No</th>
                                <th class='border'>Customer Name</th>
                                <th class='border'>NIC</th>
                                <th class='border'>Email</th>
                                <th class='border'>Address</th>
                                <th class='border'>Mobile</th>
                                <th class='border'>Alternate Number</th>
                                <th class='border'>Acc Balance</th>
                            </tr>";


            foreach (var items in admins)
            {


                html += $@"
                                <tr>
                                    <td class='border'>{items.Id}</td>
                                    <td class='border'>{items.CustName}</td>
                                    <td class='border'>{items.Nic}</td>
                                    <td class='border'>{items.Email}</td>
                                    <td class='border'>{items.CustAddress}</td>
                                    <td class='border'>{items.Mobile}</td>
                                    <td class='border'>{items.AlternatePhone}</td>
                                    <td class='border'>{items.AccountBalance}</td>
                                </tr>
                                ";
            }

            html += $@"
                           
                </table>
                </div>
                </body>
                </html>";

            var response = new HtmlResponseDto { Content = html };

            return Ok(response);
        }
    }
}
