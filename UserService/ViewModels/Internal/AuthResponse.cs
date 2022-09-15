using UserService.Enums;
using Newtonsoft.Json;

namespace UserService.ViewModels.Internal
{
    public class AuthResponse 
    {
        [JsonProperty(PropertyName = "jwt")]
        public string Jwt { get; set; } = null;        

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; } = null;

        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; } = false;

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "messageCode")]
        public AuthMessageCodes MessageCode { get; set; }

        [JsonProperty(PropertyName = "attempts")]
        public int LoginAttempts { get; set; } = 0;
    }
}
