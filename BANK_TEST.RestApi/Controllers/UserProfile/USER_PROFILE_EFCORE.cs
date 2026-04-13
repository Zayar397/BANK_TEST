using BANK_TEST.Database.DataModel;
using BANK_TEST.Database.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BANK_TEST.RestApi.Controllers.UserProfile
{
    [Route("api/[controller]")]
    [ApiController]
    public class USER_PROFILE_EFCORE : ControllerBase
    {
        private readonly AppDbContext _db = new AppDbContext();
        [HttpGet("GET_USERS")]
        public IActionResult GetUserProfile()
        {
            var userProfileList = _db.UserProfiles.Where(x => x.IsActive == true).ToList();
            return Ok(userProfileList);
        }

        [HttpGet("GET_USER_WITH_ID")]
        public IActionResult GetUserProfileById(int id)
        {
            var userProfile = _db.UserProfiles.AsNoTracking().FirstOrDefault(x => x.Id == id && x.IsActive == true);
            if (userProfile is null)
            {
                return NotFound();
            }
            return Ok(userProfile);
        }

        [HttpPost("INSERT_USER")]
        public IActionResult CreateUserProfile(UserProfile userProfile)
        {
            _db.UserProfiles.Add(userProfile);
            int recordCount = _db.SaveChanges();
            return Ok(recordCount > 0 ? "Record is inserted successfully." : "Failed to insert record");
        }

        [HttpPut("UPDATE_USER")]
        public IActionResult UpdateUserProfile(int id, UserProfile userProfile)
        {
            var item = _db.UserProfiles.AsNoTracking().FirstOrDefault(x => x.Id == id);
            if (item is null)
            {
                return NotFound();
            }
            item.FullName = userProfile.FullName;
            item.MobileNo = userProfile.MobileNo;
            item.Balance = userProfile.Balance;
            item.Pin = userProfile.Pin;
            item.CreatedDate = DateTime.Now;

            _db.Entry(item).State = EntityState.Modified;
            int recordCount = _db.SaveChanges();

            return Ok(recordCount > 0 ? "Record is updated successfully." : "Failed to update record");
        }

        [HttpPatch("UPDATE_USER_PATCH")]
        public IActionResult UpdateUserProfileWithPatch(int id, UserProfile userProfile)
        {
            var item = _db.UserProfiles.AsNoTracking().FirstOrDefault(x => x.Id == id);
            if (item is null)
            {
                return NotFound();
            }
            if (!string.IsNullOrWhiteSpace(userProfile.FullName))
            {
                item.FullName = userProfile.FullName;
            }
            if (!string.IsNullOrWhiteSpace(userProfile.MobileNo))
            {
                item.MobileNo = userProfile.MobileNo;
            }
            if (userProfile.Balance.HasValue)
            {
                item.Balance = userProfile.Balance;
            }
            if (!string.IsNullOrWhiteSpace(userProfile.Pin))
            {
                item.Pin = userProfile.Pin;
            }
            item.CreatedDate = DateTime.Now;


            _db.Entry(item).State = EntityState.Modified;
            int recordCount = _db.SaveChanges();

            return Ok(recordCount > 0 ? "Record is updated successfully." : "Failed to update record");
        }

        [HttpDelete("DELETE_USER")]
        public IActionResult DeleteUserProfile(int id)
        {
            var item = _db.UserProfiles.AsNoTracking().FirstOrDefault(x => x.Id == id);
            if (item is null)
            {
                return NotFound();
            }
            item.IsActive = false;
            item.CreatedDate = DateTime.Now;

            _db.Entry(item).State = EntityState.Modified;
            int recordCount = _db.SaveChanges();

            return Ok(recordCount > 0 ? "Record is deleted successfully." : "Failed to delete record");
        }

        [HttpPatch("DEPOSIT_REQ")]
        public IActionResult DepositReq(DEPOSTI_WITHDRAW_REQ depositReq)
        {
            if (string.IsNullOrWhiteSpace(depositReq.MobileNo))
            {
                return BadRequest("Mobile number is required.");
            }
            if (depositReq.Balance <= 0)
            {
                return BadRequest("Balance must be greater than zero.");
            }
            var item = _db.UserProfiles.AsNoTracking().FirstOrDefault(x => x.MobileNo == depositReq.MobileNo && x.IsActive == true);
            if (item is null)
            {
                return NotFound("User not found");
            }
            item.Balance = item.Balance + depositReq.Balance;
            item.CreatedDate = DateTime.Now;

            _db.Entry(item).State = EntityState.Modified;
            int recordCount = _db.SaveChanges();

            return Ok(recordCount > 0 ? "Deposit successful." : "Deposit fail.");
        }

        [HttpPatch("WITHDRAW_REQ")]
        public IActionResult WithdrawReq(DEPOSTI_WITHDRAW_REQ withdrawReq)
        {
            if (string.IsNullOrWhiteSpace(withdrawReq.MobileNo))
            {
                return BadRequest("Mobile number is required.");
            }
            if (withdrawReq.Balance <= 0)
            {
                return BadRequest("Balance must be greater than zero.");
            }

            var item = _db.UserProfiles
                        .AsNoTracking()
                        .FirstOrDefault(x => x.MobileNo == withdrawReq.MobileNo && x.IsActive == true);

            if (item is null)
            {
                return NotFound("User not found");
            }
            if (item.Balance <= 1000)
            {
                return BadRequest("Unable to withdraw as balance is less than 1,000.");
            }
            if (item.Balance <= withdrawReq.Balance)
            {
                return BadRequest("The amount to be withdrawn is greater than the available balance.");
            }

            item.Balance = item.Balance - withdrawReq.Balance;
            item.CreatedDate = DateTime.Now;

            _db.Entry(item).State = EntityState.Modified;
            int recordCount = _db.SaveChanges();

            return Ok(recordCount > 0 ? "Withdraw successful." : "Withdraw fail.");
        }

        [HttpPatch("TRANSFER_REQ")]
        public IActionResult TransferReq(TRANSFER_REQ transferReq)
        {
            if (string.IsNullOrWhiteSpace(transferReq.frMobileNo) || string.IsNullOrWhiteSpace(transferReq.toMobileNo))
            {
                return BadRequest("Both sender and receiver mobile numbers are required.");
            }
            if (transferReq.frMobileNo == transferReq.toMobileNo)
            {
                return BadRequest("Sender and receiver mobile numbers must not be same.");
            }
            if (transferReq.Balance <= 0)
            {
                return BadRequest("Balance must be greater than zero.");
            }

            var senderItem = _db.UserProfiles
                                .AsNoTracking()
                                .FirstOrDefault(x => x.MobileNo == transferReq.frMobileNo && x.IsActive == true);

            if (senderItem is null)
            {
                return NotFound("Sender user not found");
            }
            if (senderItem.Pin != transferReq.Pin)
            {
                return BadRequest("Password is not correct.");
            }
            if (senderItem.Balance <= 1000)
            {
                return BadRequest("Unable to transfer as balance is less than 1,000.");
            }
            if (senderItem.Balance <= transferReq.Balance)
            {
                return BadRequest("The amount to be transfer is greater than the available balance.");
            }

            var receiverItem = _db.UserProfiles
                                .AsNoTracking()
                                .FirstOrDefault(x => x.MobileNo == transferReq.toMobileNo && x.IsActive == true);

            if (receiverItem is null)
            {
                return NotFound("Receiver user not found");
            }

            senderItem.Balance = senderItem.Balance - transferReq.Balance;
            senderItem.CreatedDate = DateTime.Now;

            receiverItem.Balance = receiverItem.Balance + transferReq.Balance;

            _db.Entry(senderItem).State = EntityState.Modified;
            int recordCount_1 = _db.SaveChanges();

            _db.Entry(receiverItem).State = EntityState.Modified;
            int recordCount_2 = _db.SaveChanges();

            TransferAmt transferAmt = new TransferAmt
            {
                FrMobileNo = transferReq.frMobileNo,
                ToMobileNo = transferReq.toMobileNo,
                Amount = transferReq.Balance,
                CreatedDate = DateTime.Now
            };
            _db.TransferAmts.Add(transferAmt);
            _db.SaveChanges();

            return Ok(recordCount_1 > 0 && recordCount_2 > 0 ? "Transfer successful." : "Transfer fail.");
        }
    }
}
