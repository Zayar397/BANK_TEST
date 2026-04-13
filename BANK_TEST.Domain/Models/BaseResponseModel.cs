using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BANK_TEST.Domain.Models
{
    public class BaseResponseModel
    {
        public string RespCode { get; set; }
        public string RespDesc { get; set; }
        public EnumRespType RespType { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsError { get { return !IsSuccess; } }

        public static BaseResponseModel Success(string respCode, string respDesc)
        {
            return new BaseResponseModel
            {
                RespCode = respCode,
                RespDesc = respDesc,
                RespType = EnumRespType.Success,
                IsSuccess = true
            };
        }
        public static BaseResponseModel ValidationError(string respCode, string respDesc)
        {
            return new BaseResponseModel
            {
                RespCode = respCode,
                RespDesc = respDesc,
                RespType = EnumRespType.ValidationError,
                IsSuccess = true
            };
        }
        public static BaseResponseModel SystemError(string respCode, string respDesc)
        {
            return new BaseResponseModel
            {
                RespCode = respCode,
                RespDesc = respDesc,
                RespType = EnumRespType.SystemError,
                IsSuccess = true
            };
        }
    }
    public enum EnumRespType
    {
        None,
        Success,
        ValidationError,
        SystemError
    }
}
