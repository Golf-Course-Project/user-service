using Newtonsoft.Json;

namespace UserService.ViewModels.Identity
{
    public class ProfileGet
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "avatar_url")]
        public string Avatar_Url { get; set; }
        [JsonProperty(PropertyName = "isLocked")]
        public bool IsLocked { get; set; } = false;
    }
}
