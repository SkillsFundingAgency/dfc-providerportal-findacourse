﻿
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;


namespace Dfc.ProviderPortal.FindACourse.Services
{
    public class LARSSearchResultContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            var names = new Dictionary<string, string>
            {
                { "ODataContext", "@odata.context" },
                { "ODataCount", "@odata.count" },
                { "SearchFacets", "@search.facets" },
                { "AwardOrgCodeODataType", "AwardOrgCode@odata.type" },
                { "NotionalNVQLevelv2ODataType", "NotionalNVQLevelv2@odata.type" },
                { "SectorSubjectAreaTier1ODataType", "SectorSubjectAreaTier1@odata.type" },
                { "SectorSubjectAreaTier2ODataType", "SectorSubjectAreaTier2@odata.type" },
                { "AwardOrgAimRefODataType", "AwardOrgAimRef@odata.type" },
                { "SearchScore", "@search.score" }
            };

            if (names.ContainsKey(propertyName))
            {
                names.TryGetValue(propertyName, out string name);
                return name;
            }

            return propertyName;
        }
    }
}
