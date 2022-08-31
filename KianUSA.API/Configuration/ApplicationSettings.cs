﻿using KianUSA.Application.SeedWork;

namespace KianUSA.API.Configuration
{
    public class ApplicationSettings : IApplicationSettings
    {
        public string WwwRootPath { get; set; }        
        public string CatalogEmailSetting { get; set; }
        public string ContactUsEmailSetting { get; set; }
        public string SigningKey { get; set; }
        public int UserAuthorizationTokenExpireTimeInDay { get; set; }
        public string ImportPath { get; set; }
        public int StartIndexOfImageForUsingInCatalog { get; set; }        
    }
}
