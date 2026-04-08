using BANK_TEST.Database.DataModel;
using BANK_TEST.Database.Models;
using BANK_TEST.Domain.Features.Bank;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BANK_TEST.RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class USER_PROFILE_SERVICE : ControllerBase
    {
        private readonly REST_API_SERVICE _restApiService = new REST_API_SERVICE();
        [HttpGet("GET_USERS")]
        public IActionResult GetUserProfile()
        {
            var userProfileList = _restApiService.GetUserProfiles();
            return Ok(userProfileList);
        }

        [HttpGet("GET_USER_WITH_ID")]
        public IActionResult GetUserProfileById(int id)
        {
            var userProfile = _restApiService.GetUserProfileById(id);
            if (userProfile is null)
            {
                return NotFound();
            }
            return Ok(userProfile);
        }

        [HttpPost("INSERT_USER")]
        public IActionResult CreateUserProfile(UserProfile userProfile)
        {
            bool isCreated = _restApiService.CreateUserProfile(userProfile);
            return Ok(isCreated == true ? "Record is inserted successfully." : "Failed to insert record");
        }

        [HttpPut("UPDATE_USER")]
        public IActionResult UpdateUserProfile(int id, UserProfile userProfile)
        {
            string message = _restApiService.UpdateUserProfile(id, userProfile);
            return Ok(message);
        }

        [HttpPatch("UPDATE_USER_PATCH")]
        public IActionResult UpdateUserProfileWithPatch(int id, UserProfile userProfile)
        {
            string message = _restApiService.UpdateUserProfileEachField(id, userProfile);
            return Ok(message);
        }

        [HttpDelete("DELETE_USER")]
        public IActionResult DeleteUserProfile(int id)
        {
            string message = _restApiService.DeleteUserProfile(id);
            return Ok(message);
        }

        [HttpPatch("DEPOSIT_REQ")]
        public IActionResult DepositReq(DEPOSTI_WITHDRAW_REQ depositReq)
        {
            string message = _restApiService.DepositReq(depositReq);
            return Ok(message);
        }

        [HttpPatch("WITHDRAW_REQ")]
        public IActionResult WithdrawReq(DEPOSTI_WITHDRAW_REQ withdrawReq)
        {
            string message = _restApiService.WithdrawReq(withdrawReq);
            return Ok(message);
        }

        [HttpPatch("TRANSFER_REQ")]
        public IActionResult TransferReq(TRANSFER_REQ transferReq)
        {
            string message = _restApiService.TransferReq(transferReq);
            return Ok(message);
        }
    }
}
