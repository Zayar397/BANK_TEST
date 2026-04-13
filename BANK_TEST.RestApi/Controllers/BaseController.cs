using BANK_TEST.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BANK_TEST.RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public IActionResult Execute(object model)
        {
            JObject jObject = JObject.Parse(JsonConvert.SerializeObject(model));
            if (jObject.ContainsKey("Response"))
            {
                BaseResponseModel baseResponseModel = JsonConvert.DeserializeObject<BaseResponseModel>(jObject["Response"].ToString())!;
                if (baseResponseModel.RespType == EnumRespType.ValidationError)
                {
                    return BadRequest(model);
                }
                if (baseResponseModel.RespType == EnumRespType.SystemError)
                {
                    return StatusCode(500,model);
                }
                return Ok(model);
            }
            return StatusCode(500,"Invalid Response Model. Please add BaseResponseModel.");
        }
    }
}
