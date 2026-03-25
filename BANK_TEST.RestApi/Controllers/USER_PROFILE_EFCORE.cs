using BANK_TEST.Database.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BANK_TEST.RestApi.Controllers
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
            item.IsActive = true;

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
            if (item.FullName is not null)
            {
                item.FullName = userProfile.FullName;
            }
            if (item.MobileNo is not null)
            {
                item.MobileNo = userProfile.MobileNo;
            }
            if (item.Balance > 0)
            {
                item.Balance = userProfile.Balance;
            }
            if (item.Pin is not null)
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
    }
}
