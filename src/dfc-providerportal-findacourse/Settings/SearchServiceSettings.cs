
using System;
using Dfc.ProviderPortal.FindACourse.Interfaces;


namespace Dfc.ProviderPortal.FindACourse.Settings
{
    public class SearchServiceSettings : ISearchServiceSettings
    {
        public string SearchService { get; set; }
        public string ApiUrl { get; set; }
        public string ApiVersion { get; set; }
        public string QueryKey { get; set; }
        public string AdminKey { get; set; }
        public string Index { get; set; }
        public string onspdIndex { get; set; }
        public int DefaultTop { get; set; }
        public int ThresholdVenueCount { get; set; }
    }
}
