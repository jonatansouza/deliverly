namespace DeliverlyCore.Services
{
    public interface IConfigurationService
    {
        T Get<T>(string key, string defaultValue = "");
    }
}
