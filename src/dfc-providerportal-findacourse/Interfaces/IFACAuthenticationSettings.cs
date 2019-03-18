
using System;
using System.Collections.Generic;
using Dfc.ProviderPortal.FindACourse.Models;


namespace Dfc.ProviderPortal.FindACourse.Interfaces
{
    public interface IFACAuthenticationSettings
    {
        IEnumerable<APIUser> Users { get; }
    }
}
