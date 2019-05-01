﻿
using System;


namespace Dfc.ProviderPortal.FindACourse.Models
{
    public class ProviderSearchResultItem //: AzureSearchCourse
    {
        public Guid id { get; set; }
        public string Name { get; set; }
        public string Postcode { get; set; }
        public string Town { get; set; }
        public string Region { get; set; }
        public string UKPRN { get; set; }
    }
}
