namespace UserService.Misc
{
    public class AppSettings: IAppSettings
    {
        public string IdentityServiceConnectionString { get; set; } 
        public string IdentityService { get; set; }
        public string StorageConnectionString { get; set; }

    }

    public interface IAppSettings
    {
        string IdentityServiceConnectionString { get; set; }
        string IdentityService { get; set; }
        string StorageConnectionString { get; set; }   
    }
}

