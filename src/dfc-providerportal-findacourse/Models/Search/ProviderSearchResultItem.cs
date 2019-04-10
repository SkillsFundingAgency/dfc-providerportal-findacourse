
using System;
//using System.Linq;
//using System.Collections.Generic;


namespace Dfc.ProviderPortal.FindACourse.Models
{
    //[Serializable]
    public class ProviderSearchResultItem //: AzureSearchCourse
    {
        public Guid id { get; set; }
        public string Name { get; set; }
        public string Postcode { get; set; }
        //[NonSerialized]
        //public IEnumerable<string> Town { get; set; } //TownArray; // { get; set; }
        public string Town { get; set; } // { return TownArray.FirstOrDefault(); } }
        public string Region { get; set; }
        public string ProviderId { get; set; }
    }
}
