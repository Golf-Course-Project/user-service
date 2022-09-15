using Newtonsoft.Json;
using System;

namespace UserService.ViewModels.Internal
{
    public class ValidateTokenResponse
    {
        public ValidateTokenResponse()
        {
            Success = false;
            Message = String.Empty;
            Value = String.Empty;
        }       

        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
}
