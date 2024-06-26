namespace Lab11.Models
{
    public static class EnvService
    {
        public static IConfigurationRoot? ConfigRoot { get; set; }

        public static void Config(string environmentName)
        {
            ConfigRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .Build();
        }
    }
}
