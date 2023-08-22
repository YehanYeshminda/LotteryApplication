using API.Helpers;
using API.Models;
using API.Repos.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using static API.Repos.Dtos.DrawDto;

namespace API.Controllers
{
    public class UpiController : BaseApiController
    {
        private readonly LotteryContext _lotteryContext;
        private readonly IHttpClientFactory _clientFactory;
        private readonly Generators _generators;
        private readonly string baseUrl = "https://app.pinwallet.in/api/";
        private readonly ResponseDto _response;

        public UpiController(LotteryContext lotteryContext, IHttpClientFactory clientFactory, Generators generators)
        {
            _lotteryContext = lotteryContext;
            _clientFactory = clientFactory;
            _generators = generators;
            _response = new ResponseDto();
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

        [NonAction]
        public string GenerateUniquRequestNumber()
        {
            string requestNo;
            do
            {
                requestNo = _generators.GenerateRandomStringForTblRequestOrders(10);
            } while (!_generators.IsUniqueOrderForOrders(requestNo));

            return requestNo;
        }

        [HttpPost("MakeUpiWithDrawalRequest")]
        public async Task<ResponseDto> MakeUpiWithdrawalRequest(MakeUpiWithdrawalRequestDto makeUpiWithdrawalRequest)
        {
            if (makeUpiWithdrawalRequest == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Missing Authentication Details";
                return _response;
            }

            if (makeUpiWithdrawalRequest.AuthDto.Hash == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Missing Authentication Details";
                return _response;
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(makeUpiWithdrawalRequest.AuthDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == makeUpiWithdrawalRequest.AuthDto.Hash);

            var decryptedDateWithOffset = decodedValues.Date.AddDays(1);
            var currentDate = DateTime.UtcNow.Date;

            if (currentDate < decryptedDateWithOffset.Date)
            {
                try
                {
                    // MAKE EDIT BANK DETAILS
                    // MAKE THE VALUES FROM THE WALLET GO DOWN FROM THE AMOUNT WITHDRAWN

                    var existingBank = await _lotteryContext.Tblbankdetails.FirstOrDefaultAsync(x => x.UserId == _user.Id.ToString());

                    if (existingBank == null)
                    {
                        var newBank = new Tblbankdetail
                        {
                            BenificiaryAccountNo = makeUpiWithdrawalRequest.BenificiaryAccountNo,
                            BenificiaryIfscCode = makeUpiWithdrawalRequest.BenificiaryIfscCode,
                            BenificiaryName = makeUpiWithdrawalRequest.BenificiaryName,
                            UserId = _user.Id.ToString()
                        };

                        await _lotteryContext.Tblbankdetails.AddAsync(newBank);
                        await _lotteryContext.SaveChangesAsync();

                        var newRequest = new Tblrequestwithdrawal
                        {
                            Amount = makeUpiWithdrawalRequest.Amount,
                            BankId = newBank.Id,
                            RequestUniqueId = GenerateUniquRequestNumber(),
                            UserId = _user.Id,
                            Status = "0",
                            Longitude = makeUpiWithdrawalRequest.Longitude,
                            Latitude = makeUpiWithdrawalRequest.Latitude,
                        };

                        await _lotteryContext.Tblrequestwithdrawals.AddAsync(newRequest);
                        await _lotteryContext.SaveChangesAsync();

                        _response.IsSuccess = true;
                        _response.Message = "Successfully added transaction";
                        _response.Result = newRequest;
                        return _response;
                    } else
                    {
                        var newRequest = new Tblrequestwithdrawal
                        {
                            Amount = makeUpiWithdrawalRequest.Amount,
                            BankId = existingBank.Id,
                            RequestUniqueId = GenerateUniquRequestNumber(),
                            UserId = _user.Id,
                            Status = "0",
                            Longitude = makeUpiWithdrawalRequest.Longitude,
                            Latitude = makeUpiWithdrawalRequest.Latitude,
                        };

                        await _lotteryContext.Tblrequestwithdrawals.AddAsync(newRequest);
                        await _lotteryContext.SaveChangesAsync();

                        _response.IsSuccess = true;
                        _response.Message = "Successfully added transaction";
                        _response.Result = newRequest;
                        return _response;
                    }
                }
                catch (Exception ex)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Error while fetching current user history! " + ex.Message;
                    return _response;
                }

            }
            else
            {
                _response.IsSuccess = false;
                _response.Message = "Missing Authentication Details";
                return _response;
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

                                if (upiResponse.success)
                                {
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
                                }
                                else
                                {
                                    return Ok(upiResponse.message);
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

    }
}
