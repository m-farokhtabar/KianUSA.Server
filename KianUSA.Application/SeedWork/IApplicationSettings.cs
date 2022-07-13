namespace KianUSA.Application.SeedWork
{
    public interface IApplicationSettings
    {
        public string WwwRootPath { get; }        
        public string CatalogEmailSetting { get; set; }
        public string SigningKey { get; set; }
        public int UserAuthorizationTokenExpireTimeInDay { get; set; }
    }
}
