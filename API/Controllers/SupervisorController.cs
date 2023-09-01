using API.Helpers;
using API.Models;
using API.Repos.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using static API.Repos.Dtos.DrawDto;

namespace API.Controllers
{
    public class SupervisorController : BaseApiController
    {
        private readonly LotteryContext _db;
        private readonly ResponseDto _response;

        public SupervisorController(LotteryContext context)
        {
            _db = context;
            _response = new ResponseDto();
        }

        [HttpPost("AddNewSupervisor")]
        public async Task<ResponseDto> AddNewSupervisor(AssignSupervisorDto assignSupervisorDto)
        {
            if (assignSupervisorDto.AuthDto == null)
            {
                _response.Message = "Invalid data!";
                _response.IsSuccess = false;
                return _response;
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(assignSupervisorDto.AuthDto.Hash);

            var _user = await _db.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == assignSupervisorDto.AuthDto.Hash);

            if (_user == null)
            {
                _response.Message = "Invalid Authentication Details";
                _response.IsSuccess = false;
                return _response;
            }

            if (_user.Role == "Admin")
            {
                _response.Message = "Invalid Authentication Details";
                _response.IsSuccess = false;
                return _response;
            }

            var decryptedDateWithOffset = decodedValues.Date.AddDays(1);
            var currentDate = DateTime.UtcNow.Date;

            if (currentDate < decryptedDateWithOffset.Date)
            {
                try
                {
                    var existingSupervisor = await _db.Tblregisters.FirstOrDefaultAsync(x => x.Id == assignSupervisorDto.SupervisorId);
                    var existingUser = await _db.Tblregisters.FirstOrDefaultAsync(x => x.Id == assignSupervisorDto.AssignedIntoUser);
                    var existingCoupon = await _db.Tblsupervisors.FirstOrDefaultAsync(x => x.CouponNo == assignSupervisorDto.Coupon);

                    if (existingSupervisor == null)
                    {
                        _response.Message = "Unable to find this supervisor";
                        _response.IsSuccess = false;
                        return _response;
                    }

                    if (existingCoupon != null)
                    {
                        _response.Message = "Coupon already exists";
                        _response.IsSuccess = false;
                        return _response;
                    }

                    if (existingUser == null)
                    {
                        _response.Message = "Unable to find this user with this Id";
                        _response.IsSuccess = false;
                        return _response;
                    }

                    if (existingSupervisor.Role != "Supervisor")
                    {
                        _response.Message = "Unable to find this supervisor with this role";
                        _response.IsSuccess = false;
                        return _response;
                    }

                    var newSupervisor = new Tblsupervisor
                    {
                        SupervisorId = existingSupervisor.Id,
                        UnderUserId = existingUser.Id,
                        CouponNo = assignSupervisorDto.Coupon
                    };

                    await _db.Tblsupervisors.AddAsync(newSupervisor);
                    await _db.SaveChangesAsync();

                    _response.Message = "Supervisor Assigned Successfully!";
                    _response.IsSuccess = true;
                    _response.Result = newSupervisor;

                    return _response;
                }
                catch (Exception ex)
                {
                    _response.Message = "Error occurred while buying lotto! " + ex.Message;
                    _response.IsSuccess = false;
                    return _response;
                }
            }
            else
            {
                _response.Message = "Invalid Authentication Details";
                _response.IsSuccess = false;
                return _response;
            }
        }
    }
}
