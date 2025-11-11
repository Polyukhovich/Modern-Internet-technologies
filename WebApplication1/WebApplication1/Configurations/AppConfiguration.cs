namespace WebApplication1.Configurations
{
    public class AppConfiguration
    {
        public string ApplicationName { get; set; } = string.Empty;
        public string Theme { get; set; } = string.Empty; // приклад специфічного параметра
        public ApiSettings ApiSettings { get; set; } = new ApiSettings();
        public string DefaultRole { get; set; } = string.Empty;
    }

    public class ApiSettings
    {
        public string ApiKey { get; set; } = string.Empty;
    }
}
