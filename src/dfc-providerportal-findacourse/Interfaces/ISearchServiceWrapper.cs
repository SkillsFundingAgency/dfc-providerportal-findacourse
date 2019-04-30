﻿
using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Search.Models;
using Dfc.ProviderPortal.FindACourse.Models;
using Document = Microsoft.Azure.Documents.Document;


namespace Dfc.ProviderPortal.FindACourse.Interfaces
{
    public interface ISearchServiceWrapper
    {
        //IEnumerable<IndexingResult> UploadBatch(
        //    IEnumerable<AzureSearchProviderModel> providers,
        //    IEnumerable<AzureSearchVenueModel> venues,
        //    IReadOnlyList<Document> documents,
        //    out int succeeded);
        //DocumentSearchResult<AzureSearchCourse> SearchCourses(SearchCriteriaStructure criteria); // string SearchText);
        FACSearchResult SearchCourses(SearchCriteriaStructure criteria);
    }
}
