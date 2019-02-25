
using System;
using System.Collections.Generic;
using Dfc.ProviderPortal.FindACourse.Interfaces;


namespace Dfc.ProviderPortal.FindACourse.Models
{
    public class FACSearchResult : IFACSearchResult
    {
        public string ODataContext { get; set; }
        public int? ODataCount { get; set; }
        public dynamic SearchFacets { get; set; } //FACSearchFacets SearchFacets { get; set; }
        public IEnumerable<FACSearchResultItem> Value { get; set; }
    }
}
