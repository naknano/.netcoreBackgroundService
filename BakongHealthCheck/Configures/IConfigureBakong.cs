namespace BakongHealthCheck.Configures
{
    public interface IConfigureBakong
    {
        public string BakongBaseUrl { get; set; }
        public string BakongHealthCheck { get; set; }
        public string BakongTimeService { get; set; }

    }
}
