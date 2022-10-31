using Newtonsoft.Json;

namespace UserService.ViewModels.Identity
{
    public class UserAvatarPatch
    {
        [JsonProperty(PropertyName = "imageUrl")]
        public string imageUrl { get; set; }
    }
}
