﻿
using System;
using Dfc.ProviderPortal.Packages;


namespace Dfc.ProviderPortal.FindACourse.Models
{
    public class ProviderSearchCriteriaStructure
    {
        //public string APIKeyField { get; set; }
        public string Keyword { get; set; }
        public string[] Towns { get; set; }
        public string[] Regions { get; set; }

        public int? TopResults { get; set; }


        public ProviderSearchCriteriaStructure()
        {
            //if (TopResults.HasValue)
            //    Throw.IfLessThan(1, TopResults.Value, "");
        }
    }
}
