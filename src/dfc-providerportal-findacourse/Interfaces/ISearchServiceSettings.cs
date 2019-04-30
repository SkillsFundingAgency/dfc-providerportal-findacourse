
using System;


namespace Dfc.ProviderPortal.FindACourse.Interfaces
{
    public interface ISearchServiceSettings
    {
        string SearchService { get; }
        string ApiUrl { get; }
        string ProviderApiUrl { get; }
        string ApiVersion { get; }
        string QueryKey { get; }
        string AdminKey { get; }
        string Index { get; }
        string onspdIndex { get; }
        int DefaultTop { get; }
        string RegionBoostScoringProfile { get; }
        int ThresholdVenueCount { get; }
    }
}
