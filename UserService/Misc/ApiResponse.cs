using System;
using UserService.Enums;
using Newtonsoft.Json;

namespace UserService.Misc
{
    public class ApiResponse : IApiResponse
    {
        public ApiResponse()
        {
            Success = false;
            Count = 0;
            Value = null;
            Message = String.Empty;
            MessageCode = ApiMessageCodes.Failed;
        }

        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "messageCode")]
        public ApiMessageCodes MessageCode { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
    }

    public interface IApiResponse
    {
        bool Success { get; set; }
        string Message { get; set; }
        ApiMessageCodes MessageCode { get; set; }
        object Value { get; set; }
        int Count { get; set; }
    }

    public class AuthApiResponse : IAuthApiResponse
    {
        public AuthApiResponse()
        {
            Success = false;
            Count = 0;
            Value = null;
            Message = String.Empty;
            MessageCode = AuthMessageCodes.Failed;
        }

        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "messageCode")]
        public AuthMessageCodes MessageCode { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
    }

    public interface IAuthApiResponse
    {
        bool Success { get; set; }
        string Message { get; set; }
        AuthMessageCodes MessageCode { get; set; }
        object Value { get; set; }
        int Count { get; set; }
    }
}
