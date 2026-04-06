using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BANK_TEST.Database.DataModel;
using BANK_TEST.Database.Models;
using BANK_TEST.RestApi.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BANK_TEST.Domain.Features.Bank
{
    public class REST_API_SERVICE
    {
        private readonly AppDbContext _db = new AppDbContext();
        public List<UserProfile> GetUserProfiles()
        {
            var userList = _db.UserProfiles.AsNoTracking().ToList();
            return userList;
        }
        public UserProfile GetUserProfileById(int id)
        {
            var user = _db.UserProfiles
                            .AsNoTracking()
                            .FirstOrDefault(x => x.Id == id);
            return user;
        }
        public bool CreateUserProfile(UserProfile userProfile)
        {
            _db.UserProfiles.Add(userProfile);
            int recordCount = _db.SaveChanges();
            return recordCount > 0;
        }
        public string UpdateUserProfile(int id, UserProfile userProfile)
        {
            var item = _db.UserProfiles
                            .AsNoTracking()
                            .FirstOrDefault(x => x.Id == id);   
            if(item is null)
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
            if (item.Balance <= 1000)
            {
                return "Unable to withdraw as balance is less than 1,000.";
            }
            if (item.Balance <= withdrawReq.Balance)
            {
                return "The amount to be withdrawn is greater than the available balance.";
            }

            item.Balance = item.Balance - withdrawReq.Balance;
            item.CreatedDate = DateTime.Now;

            _db.Entry(item).State = EntityState.Modified;
            int recordCount = _db.SaveChanges();

            return recordCount > 0 ? "Withdraw successful." : "Withdraw fail.";
        }
        public string TransferReq(TRANSFER_REQ transferReq)
        {
            if (string.IsNullOrWhiteSpace(transferReq.frMobileNo) || string.IsNullOrWhiteSpace(transferReq.toMobileNo))
            {
                return "Both sender and receiver mobile numbers are required.";
            }
            if (transferReq.frMobileNo == transferReq.toMobileNo)
            {
                return "Sender and receiver mobile numbers must not be same.";
            }
            if (transferReq.Balance <= 0)
            {
                return "Balance must be greater than zero.";
            }

            var senderItem = _db.UserProfiles
                                .AsNoTracking()
                                .FirstOrDefault(x => x.MobileNo == transferReq.frMobileNo && x.IsActive == true);

            if (senderItem is null)
            {
                return "Sender user not found";
            }
            if (senderItem.Pin != transferReq.Pin)
            {
                return "Password is not correct.";
            }
            if (senderItem.Balance <= 1000)
            {
                return "Unable to transfer as balance is less than 1,000.";
            }
            if (senderItem.Balance <= transferReq.Balance)
            {
                return "The amount to be transfer is greater than the available balance.";
            }

            var receiverItem = _db.UserProfiles
                                .AsNoTracking()
                                .FirstOrDefault(x => x.MobileNo == transferReq.toMobileNo && x.IsActive == true);

            if (receiverItem is null)
            {
                return "Receiver user not found";
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

            return recordCount_1 > 0 && recordCount_2 > 0 ? "Transfer successful." : "Transfer fail.";
        }
    }
}
