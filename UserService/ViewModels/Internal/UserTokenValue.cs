using UserService.Enums;
using Newtonsoft.Json;

namespace UserService.ViewModels.Internal
{
    public class UserTokenValue
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; } = null;        

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; } = null;
       
    }
}
