using Newtonsoft.Json;

namespace UserService.ViewModels.Identity
{
    public class ProfilePatch
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }     
    }
}
