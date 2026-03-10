namespace DeliverlyCore.Services
{
    public class ConfigurationService: IConfigurationService
    {
        public T Get<T>(string key, string defaultValue = "")
        {
            var value = Environment.GetEnvironmentVariable(key);

            return typeof(T) switch
            {
                var t when t == typeof(string) => (T)(object)(Environment.GetEnvironmentVariable(key) ?? ""),
                var t when t == typeof(int) => (T)(object)int.Parse(Environment.GetEnvironmentVariable(key) ?? "0"),
                var t when t == typeof(decimal) => (T)(object)decimal.Parse(Environment.GetEnvironmentVariable(key) ?? "0"),
                _ => throw new NotSupportedException($"Type {typeof(T).Name} is not allowed.")
            };
        }
    }
}
