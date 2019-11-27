using System.Collections.Generic;
using Dfc.ProviderPortal.FindACourse.Interfaces;
using Microsoft.Azure.Search.Models;

namespace Dfc.ProviderPortal.FindACourse.Models
{
    public class FACSearchResult : IFACSearchResult
    {
        public long ResultCount { get; set; }
        public IDictionary<string, IList<FacetResult>> Facets { get; set; }
        public IEnumerable<FACSearchResultItem> Items { get; set; }
    }
}
