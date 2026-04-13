  using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BANK_TEST.Database.DataModel;
using BANK_TEST.Database.Models;
using BANK_TEST.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BANK_TEST.Domain.Features.Bank
{
    public class REST_API_SERVICE
    {
        private readonly AppDbContext _db = new AppDbContext();
        public Task<List<UserProfile>> GetUserProfileAsync()
        {
            var userList = _db.UserProfiles.AsNoTracking().ToListAsync();
            return userList;
        }
        public Task<UserProfile> GetUserProfileByIdAsync(int id)
        {
            var user = _db.UserProfiles
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.Id == id);
            return user;
        }
        public async Task<bool> CreateUserProfileAsync(UserProfile userProfile)
        {
            await _db.UserProfiles.AddAsync(userProfile);
            int recordCount = await _db.SaveChangesAsync();
            return recordCount > 0;
        }
        public string UpdateUserProfile(int id, UserProfile userProfile)
        {
            var item = _db.UserProfiles
                            .AsNoTracking()
                            .FirstOrDefault(x => x.Id == id);
            if (item is null)
            {
                return "User not found";
            }
            item.FullName = userProfile.FullName;
            item.MobileNo = userProfile.MobileNo;
            item.Balance = userProfile.Balance;
            item.Pin = userProfile.Pin;
            item.CreatedDate = DateTime.Now;

            _db.Entry(item).State = EntityState.Modified;
            int recordCount = _db.SaveChanges();
            return recordCount > 0 ? "Record is updated successfully." : "Failed to update record";
        }
        public string UpdateUserProfileEachField(int id, UserProfile userProfile)
        {
            var item = _db.UserProfiles
                            .AsNoTracking()
                            .FirstOrDefault(x => x.Id == id);
            if (item is null)
            {
                return "User not found";
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
            return recordCount > 0 ? "Record is updated successfully." : "Failed to update record";
        }
        public string DeleteUserProfile(int id)
        {
            var item = _db.UserProfiles
                            .AsNoTracking()
                            .FirstOrDefault(x => x.Id == id);
            if (item is null)
            {
                return "User not found";
            }
            item.IsActive = false;
            item.CreatedDate = DateTime.Now;

            _db.Entry(item).State = EntityState.Modified;
            int recordCount = _db.SaveChanges();
            return recordCount > 0 ? "Record is deleted successfully." : "Failed to delete record";
        }
        public string DepositReq(DEPOSTI_WITHDRAW_REQ depositReq)
        {
            if (string.IsNullOrWhiteSpace(depositReq.MobileNo))
            {
                return "Mobile number is required.";
            }
            if (depositReq.Balance <= 0)
            {
                return "Balance must be greater than zero.";
            }
            var item = _db.UserProfiles.AsNoTracking().FirstOrDefault(x => x.MobileNo == depositReq.MobileNo && x.IsActive == true);
            if (item is null)
            {
                return "User not found";
            }
            item.Balance = item.Balance + depositReq.Balance;
            item.CreatedDate = DateTime.Now;

            _db.Entry(item).State = EntityState.Modified;
            int recordCount = _db.SaveChanges();

            return recordCount > 0 ? "Deposit successful." : "Deposit fail.";
        }
        public string WithdrawReq(DEPOSTI_WITHDRAW_REQ withdrawReq)
        {
            if (string.IsNullOrWhiteSpace(withdrawReq.MobileNo))
            {
                return "Mobile number is required.";
            }
            if (withdrawReq.Balance <= 0)
            {
                return "Balance must be greater than zero.";
            }

            var item = _db.UserProfiles
                        .AsNoTracking()
                        .FirstOrDefault(x => x.MobileNo == withdrawReq.MobileNo && x.IsActive == true);

            if (item is null)
            {
                return "User not found";
            }
            if (item.Balance - withdrawReq.Balance <= 1000)
            {
                return "Unable to withdraw as balance is less than 1,000.";
            }

            item.Balance = item.Balance - withdrawReq.Balance;
            item.CreatedDate = DateTime.Now;

            _db.Entry(item).State = EntityState.Modified;
            int recordCount = _db.SaveChanges();

            return recordCount > 0 ? "Withdraw successful." : "Withdraw fail.";
        }
        public async Task<TransferResponseModel> TransferReqAsync(TRANSFER_REQ transferReq)
        {
            TransferResponseModel model = new TransferResponseModel();
            if (string.IsNullOrWhiteSpace(transferReq.frMobileNo) || string.IsNullOrWhiteSpace(transferReq.toMobileNo))
            {
                //return "Both sender and receiver mobile numbers are required.";
                model.Response = BaseResponseModel.ValidationError("999", "Both sender and receiver mobile numbers are required.");
                goto Result;
            }
            if (transferReq.frMobileNo == transferReq.toMobileNo)
            {
                //return "Sender and receiver mobile numbers must not be same.";
                model.Response = BaseResponseModel.ValidationError("999", "Sender and receiver mobile numbers must not be same.");
                goto Result;
            }
            if (transferReq.Balance <= 0)
            {
                //return "Balance must be greater than zero.";
                model.Response = BaseResponseModel.ValidationError("999", "Balance must be greater than zero.");
                goto Result;
            }

            var senderItem = await _db.UserProfiles
                                        .FirstOrDefaultAsync(x => x.MobileNo == transferReq.frMobileNo && x.IsActive == true);

            if (senderItem is null)
            {
                //return "Sender user not found";
                model.Response = BaseResponseModel.ValidationError("999", "Sender user not found");
                goto Result;
            }
            if (senderItem.Pin != transferReq.Pin)
            {
                //return "Password is not correct.";
                model.Response = BaseResponseModel.ValidationError("999", "Password is not correct.");
                goto Result;
            }
            if (senderItem.Balance - transferReq.Balance <= 1000)
            {
                //return "Unable to transfer as balance is less than 1,000.";
                model.Response = BaseResponseModel.ValidationError("999", "Unable to transfer as balance is less than 1,000.");
                goto Result;
            }

            var receiverItem = await _db.UserProfiles
                                            .FirstOrDefaultAsync(x => x.MobileNo == transferReq.toMobileNo && x.IsActive == true);

            if (receiverItem is null)
            {
                //return "Receiver user not found";
                model.Response = BaseResponseModel.ValidationError("999", "Receiver user not found");
                goto Result;
            }

            senderItem.Balance = senderItem.Balance - transferReq.Balance;
            senderItem.CreatedDate = DateTime.Now;
            await _db.SaveChangesAsync();

            receiverItem.Balance = receiverItem.Balance + transferReq.Balance;
            await _db.SaveChangesAsync();

            //_db.Entry(senderItem).State = EntityState.Modified;
            //int recordCount_1 = _db.SaveChanges();

            //_db.Entry(receiverItem).State = EntityState.Modified;
            //int recordCount_2 = _db.SaveChanges();

            TransferAmt transferAmt = new TransferAmt
            {
                FrMobileNo = transferReq.frMobileNo,
                ToMobileNo = transferReq.toMobileNo,
                Amount = transferReq.Balance,
                CreatedDate = DateTime.Now
            };
            await _db.TransferAmts.AddAsync(transferAmt);
            await _db.SaveChangesAsync();

            //return recordCount_1 > 0 && recordCount_2 > 0 ? "Transfer successful." : "Transfer fail.";
            model.Response = BaseResponseModel.Success("000", "Transfer successful.");

        Result:
            return model;
        }
    }
}
