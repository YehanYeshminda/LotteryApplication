using API.Helpers;
using API.Models;
using API.Repos.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;
using static API.Repos.Dtos.DrawDto;

namespace API.Controllers
{
    public class OrderController : BaseApiController
    {
        private readonly LotteryContext _lotteryContext;
        private ResponseDto _response;
        public OrderController(LotteryContext lotteryContext)
        {
            _lotteryContext = lotteryContext;
            _response = new ResponseDto();
        }

        public class ItemsToReturnForOrders
        {
            public string PackageName { get; set; }
            public string PackageUniqueId { get; set; }
            public string PackagePrice { get; set; }
            public string PackageOrderUniqueId { get; set; }
            public DateTime? AddOn { get; set; }
            public int OrderStatus { get; set; }
        }

        [HttpPost]
        public async Task<ActionResult<ItemsToReturnForOrders>> GetAllUserOrders(AuthDto authDto)
        {
            if (authDto.Hash == null)
            {
                return Unauthorized("Missing Authentication Details");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(authDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == authDto.Hash);

            if (_user == null)
            {
                return Unauthorized("Missing Authentication Details");
            }

            var decryptedDateWithOffset = decodedValues.Date.AddDays(1);
            var currentDate = DateTime.UtcNow.Date;

            if (currentDate < decryptedDateWithOffset.Date)
            {
                try
                {
                    DateTime threeDaysAgo = DateTime.Now.AddDays(-3);
                    var existingUserPackageOrders = await _lotteryContext.Tblpackageorderhistories.Where(x => x.UserId == _user.Id && x.AddOn >= threeDaysAgo).ToListAsync();

                    var newItems = new List<ItemsToReturnForOrders>();
                    if (existingUserPackageOrders == null)
                    {
                        return BadRequest("No orders found!");
                    }

                    foreach (var item in existingUserPackageOrders)
                    {
                        var newData = new ItemsToReturnForOrders
                        {
                            AddOn = item.AddOn,
                            OrderStatus = (int)item.OrderStatus,
                            PackageName = item.PackageName,
                            PackageOrderUniqueId = item.PackageOrderUniqueId,
                            PackagePrice = item.PackagePrice,
                            PackageUniqueId = item.PackageUniqueId
                        };

                        newItems.Add(newData);
                    }

                    return Ok(newItems);
                }
                catch (Exception ex)
                {
                    return BadRequest("Error occured while fetching orders! " + ex.Message);
                }
            }
            else
            {
                return Unauthorized("Missing Authentication Details");
            }
        }

       public class OrderUpdateDto
        {
            public AuthDto AuthDto { get; set; }
            public string OrderReferenceId { get; set; }
        }

        [HttpPost("UpdateStatus")]
        public async Task<ResponseDto> UpdateOrderStatus(OrderUpdateDto orderUpdateDto)
        {
            if (orderUpdateDto.AuthDto.Hash == null)
            {
                _response.Message = "Missing Authentication Details";
                _response.IsSuccess = false;
                return _response;
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(orderUpdateDto.AuthDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == orderUpdateDto.AuthDto.Hash);

            if (_user == null)
            {
                _response.Message = "Missing Authentication Details";
                _response.IsSuccess = false;
                return _response;
            }

            var decryptedDateWithOffset = decodedValues.Date.AddDays(1);
            var currentDate = DateTime.UtcNow.Date;

            if (currentDate < decryptedDateWithOffset.Date)
            {
                try
                {
                    return _response;
                }
                catch (Exception ex)
                {
                    _response.Message = "Error occured while updating order status! " + ex.Message;
                    _response.IsSuccess = false;
                    return _response;
                }
            }
            else
            {
                _response.Message = "Missing Authentication Details";
                _response.IsSuccess = false;
                return _response;
            }
        }
    }
}
