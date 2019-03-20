
using System;
using System.Collections.Generic;
using Dfc.ProviderPortal.FindACourse.Models;


namespace Dfc.ProviderPortal.FindACourse.Interfaces
{
    public interface IFACAuthenticationSettings
    {
        string UserName { get; }
        string Password { get; }
    }
}
