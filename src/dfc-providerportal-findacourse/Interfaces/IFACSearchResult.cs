
using System;
using System.Collections.Generic;
using Dfc.ProviderPortal.FindACourse.Models;


namespace Dfc.ProviderPortal.FindACourse.Interfaces
{
    public interface IFACSearchResult
    {
        string ODataContext { get; set; }
        int? ODataCount { get; set; }
        dynamic SearchFacets { get; set; } //FACSearchFacets SearchFacets { get; set; }
        IEnumerable<FACSearchResultItem> Value { get; set; }
    }
}
