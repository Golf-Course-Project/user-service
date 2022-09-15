namespace UserService.Misc
{
    public class AppSettings: IAppSettings
    {
        public string IdentityService { get; set; } 
        public string StorageConnectionString { get; set; }
    }

    public interface IAppSettings
    {
        string IdentityService { get; set; }
        string StorageConnectionString { get; set; }   
    }
}

