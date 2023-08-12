using API.Helpers;
using API.Models;
using API.Repos.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace API.Controllers
{
    public class UpiController : BaseApiController
    {
        private readonly LotteryContext _lotteryContext;
        private readonly IHttpClientFactory _clientFactory;
        private readonly string baseUrl = "https://app.pinwallet.in/api/";

        public UpiController(LotteryContext lotteryContext, IHttpClientFactory clientFactory)
        {
            _lotteryContext = lotteryContext;
            _clientFactory = clientFactory;
        }

        public class GenerateUpiDto
        {
            public AuthDto authDto { get; set; }
            public string OrderNo { get; set; }
            public string Total { get; set; }
        }

        public class QrCode
        {
            public string Qr { get; set; }
        }

        [HttpPost("generate-upi")]
        public async Task<IActionResult> GenerateUPIAsync(GenerateUpiDto generateUpiDto)
        {
            if (generateUpiDto == null)
            {
                return BadRequest("Invalid data!");
            }

            if (generateUpiDto.authDto.Hash == null)
            {
                return Unauthorized("Missing Authentication Details");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(generateUpiDto.authDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == generateUpiDto.authDto.Hash);

            if (_user == null && _user.Role != "Admin")
            {
                return Unauthorized("Invalid Authentication Details");
            }

            var decryptedDateWithOffset = decodedValues.Date.AddDays(1);
            var currentDate = DateTime.UtcNow.Date;

            if (currentDate < decryptedDateWithOffset.Date)
            {
                try
                {
                    if (!string.IsNullOrEmpty(_user.Email))
                    {
                        // Get Auth Details for User
                        var upiPerson = await _lotteryContext.Tblregisters.Where(x => x.Role == "UPIPerson").FirstOrDefaultAsync();

                        if (upiPerson == null)
                        {
                            return BadRequest("UPI Person does not exist");
                        }

                        var newUpiPersonData = new GetUpiPerson
                        {
                            password = upiPerson.CustPassword,
                            username = upiPerson.CustName,
                        };

                        BaseResponse authResponse = await GetAuthDetailsAsync(newUpiPersonData);

                        if (authResponse.success)
                        {

                            if (upiPerson != null)
                            {
                                DataToSendUpi sendData = new DataToSendUpi
                                {
                                    amount = generateUpiDto.Total,
                                    Email = _user.Email,
                                    Name = _user.CustName,
                                    Phone = _user.ContactNo,
                                    ReferenceId = generateUpiDto.OrderNo
                                };

                                BaseResponseFromUPi upiResponse = await GenerateUpiAsync(sendData, authResponse.data.token, upiPerson.CustPassword);

                                if (upiResponse.success)
                                {
                                    var newItem = new QrCode
                                    {
                                        Qr = upiResponse.data.qr
                                    };

                                    return Ok(newItem);
                                }
                                else
                                {
                                    return BadRequest($"Error while generating UPI: {upiResponse.message}");
                                }
                            }
                        }
                        else
                        {
                            return BadRequest($"Error while getting auth: {authResponse.message}");
                        }
                    }

                    return BadRequest("Invalid username from UPI user.");
                }
                catch (Exception ex)
                {
                    return BadRequest("Error occured while creating Draw!" + ex.Message);
                }

            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }

        private async Task<BaseResponse> GetAuthDetailsAsync(GetUpiPerson data)
        {
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.PostAsJsonAsync("https://app.pinwallet.in/api/token/create", data);
            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BaseResponse>(responseContent);
        }

        private async Task<BaseResponseFromUPi> GenerateUpiAsync(DataToSendUpi data, string token, string password)
        {
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("AuthKey", password);
            client.DefaultRequestHeaders.Add("IPAddress", "16.16.243.167");

            HttpResponseMessage response = await client.PostAsJsonAsync("https://app.pinwallet.in/api/DyupiV2/V4/GenerateUPI", data);
            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BaseResponseFromUPi>(responseContent);
        }

        private async Task<GetStatusCheckResponse> DoStatusCheck(string transactionId, string token, string password)
        {
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("IPAddress", "16.16.243.167");

            var data = new
            {
                TransactionId = transactionId
            };

            HttpResponseMessage response = await client.PostAsJsonAsync("http://app.pinwallet.in/api/upipayout/docheckstatus", data);
            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetStatusCheckResponse>(responseContent);
        }

        public class AuthDetails
        {
            public string username { get; set; }
            public string password { get; set; }
        }

        public class GetUpiPerson
        {
            public string username { get; set; }
            public string password { get; set; }
        }

        public class BaseResponse
        {
            public int responseCode { get; set; }
            public bool success { get; set; }
            public string message { get; set; }
            public Data data { get; set; }
        }

        public class Data
        {
            public string token { get; set; }
            public Partner partner { get; set; }
        }

        public class Partner
        {
            public int id { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string phone { get; set; }
        }

        public class BaseResponseFromUPi
        {
            public int responseCode { get; set; }
            public bool success { get; set; }
            public string message { get; set; }
            public DataBackFromUPi data { get; set; }
        }

        public class DataBackFromUPi
        {
            public string qr { get; set; }
            public string pinWalletTransactionId { get; set; }
            public string userTrasnactionId { get; set; }
            public string status { get; set; }
            public string statusMessage { get; set; }
            public string statusCode { get; set; }
        }

        public class DataToSendUpi
        {
            public string Name { get; set; }
            public string ReferenceId { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string amount { get; set; }
        }

        public class MakeUpiStatusCheck
        {
            public string TransactionId { get; set; }
        }

        public class MakeUpiStatusCheckDto
        {
            public AuthDto AuthDto { get; set; }
            public string TransactionId { get; set; }
        }

        public class GetStatusCheckResponse
        {
            public int responseCode { get; set; }
            public bool success { get; set; }
            public string message { get; set; }
            public object data { get; set; }
        }

        public class ReturnStatusCheck
        {
            public string Status { get; set; }
        }

        [HttpPost("checkstatusupi")]
        public async Task<ActionResult<ReturnStatusCheck>> CheckUPIStatusAsync(MakeUpiStatusCheckDto makeUpiStatusCheckDto)
        {
            if (makeUpiStatusCheckDto.AuthDto == null)
            {
                return BadRequest("Invalid data!");
            }

            if (makeUpiStatusCheckDto.AuthDto.Hash == null)
            {
                return Unauthorized("Missing Authentication Details");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(makeUpiStatusCheckDto.AuthDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == makeUpiStatusCheckDto.AuthDto.Hash);

            if (_user == null && _user.Role != "Admin")
            {
                return Unauthorized("Invalid Authentication Details");
            }

            var decryptedDateWithOffset = decodedValues.Date.AddDays(1);
            var currentDate = DateTime.UtcNow.Date;

            if (currentDate < decryptedDateWithOffset.Date)
            {
                try
                {
                    if (!string.IsNullOrEmpty(_user.Email))
                    {
                        var upiPerson = await _lotteryContext.Tblregisters.Where(x => x.Role == "UPIPerson").FirstOrDefaultAsync();

                        if (upiPerson == null)
                        {
                            return BadRequest("UPI Person does not exist");
                        }

                        var newUpiPersonData = new GetUpiPerson
                        {
                            password = upiPerson.CustPassword,
                            username = upiPerson.CustName,
                        };

                        BaseResponse authResponse = await GetAuthDetailsAsync(newUpiPersonData);

                        if (authResponse.success)
                        {

                            if (upiPerson != null)
                            {

                                GetStatusCheckResponse upiResponse = await DoStatusCheck(makeUpiStatusCheckDto.TransactionId, authResponse.data.token, upiPerson.CustPassword);

                                //if (upiResponse.success)
                                //{
                                    var existingTrancation = await _lotteryContext.Tblpackageorderhistories.FirstOrDefaultAsync(x => x.PackageOrderUniqueId == makeUpiStatusCheckDto.TransactionId);

                                    if (existingTrancation == null)
                                    {
                                        return BadRequest("Transaction does not exist!");
                                    }

                                    existingTrancation.OrderStatus = 1;
                                    await _lotteryContext.SaveChangesAsync();

                                    _user.AccountBalance += Convert.ToDecimal(existingTrancation.PackagePrice);
                                    var newCheck = new ReturnStatusCheck
                                    {
                                        Status = existingTrancation.OrderStatus.ToString()
                                    };

                                    return Ok(newCheck);
                                //}
                                //else
                                //{
                                //    return Ok(upiResponse.message);
                                //}
                            }
                        }
                        else
                        {
                            return BadRequest($"Error while getting auth: {authResponse.message}");
                        }
                    }

                    return BadRequest("Invalid username from UPI user.");
                }
                catch (Exception ex)
                {
                    return BadRequest("Error occured while creating Draw!" + ex.Message);
                }

            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }

    }
}
