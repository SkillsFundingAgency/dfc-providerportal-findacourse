
using System;


namespace Dfc.ProviderPortal.FindACourse.Models
{
    public class FACSearchResultItem : AzureSearchCourse
    {
        public dynamic SearchScore { get; set; }
        public new dynamic VenueLocation { get; set; }
        public double? GeoSearchDistance { get; set; }

    }
}
