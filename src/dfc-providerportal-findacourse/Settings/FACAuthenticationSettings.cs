
using System;
using System.Collections.Generic;
using Dfc.ProviderPortal.FindACourse.Interfaces;
using Dfc.ProviderPortal.FindACourse.Models;


namespace Dfc.ProviderPortal.FindACourse.Settings
{
    public class FACAuthenticationSettings : IFACAuthenticationSettings
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
