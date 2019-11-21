
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Spatial;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Dfc.GeoCoordinate;
using Dfc.ProviderPortal.Packages;
using Dfc.ProviderPortal.FindACourse.Models;
using Dfc.ProviderPortal.FindACourse.Settings;
using Dfc.ProviderPortal.FindACourse.Interfaces;
using Dfc.ProviderPortal.FindACourse.Services;
using Document = Microsoft.Azure.Documents.Document;


namespace Dfc.ProviderPortal.FindACourse.Helpers
{
    public class SearchServiceWrapper : ISearchServiceWrapper
    {
        public class LINQComboClass
        {
            public Course Course { get; set; }
            public CourseRun Run { get; set; }
            public string Region { get; set; }
            public AzureSearchVenueModel Venue { get; set; }
        }

        private readonly ILogger _log;
        private readonly ISearchServiceSettings _settings;
        //private readonly IProviderServiceSettings _providerServiceSettings;
        //private readonly IVenueServiceSettings _venueServiceSettings;
        private static SearchServiceClient _queryService;
        private static SearchServiceClient _adminService;
        private static ISearchIndexClient _queryIndex;
        private static ISearchIndexClient _adminIndex;
        //private static ISearchIndexClient _providerIndex;
        private static ISearchIndexClient _onspdIndex;
        private HttpClient _httpClient;
        private readonly Uri _uri;
        private readonly Uri _providerUri;
        private readonly Uri _larsUri;
        private readonly Uri _onspdUri;

        public SearchServiceWrapper(
            ILogger log,
            //HttpClient httpClient,
            //IOptions<ProviderServiceSettings> providerServiceSettings,
            //IOptions<VenueServiceSettings> venueServiceSettings,
            ISearchServiceSettings settings)
        {
            Throw.IfNull(log, nameof(log));
            //Throw.IfNull(httpClient, nameof(httpClient));
            //Throw.IfNull(providerServiceSettings, nameof(providerServiceSettings));
            //Throw.IfNull(venueServiceSettings, nameof(venueServiceSettings));
            Throw.IfNull(settings, nameof(settings));

            _log = log;
            _settings = settings;
            //_providerServiceSettings = providerServiceSettings.Value;
            //_venueServiceSettings = venueServiceSettings.Value;

            _queryService = new SearchServiceClient(settings.SearchService, new SearchCredentials(settings.QueryKey));
            _adminService = new SearchServiceClient(settings.SearchService, new SearchCredentials(settings.AdminKey));
            _queryIndex = _queryService?.Indexes?.GetClient(settings.Index);
            _adminIndex = _adminService?.Indexes?.GetClient(settings.Index);
            _onspdIndex = _queryService?.Indexes?.GetClient(settings.onspdIndex);

            _httpClient = new HttpClient(); //httpClient;
            //_httpClient.DefaultRequestHeaders.Accept.Clear();
            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //_httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            //_httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
            _httpClient.DefaultRequestHeaders.Add("api-key", settings.QueryKey);
            _httpClient.DefaultRequestHeaders.Add("api-version", settings.ApiVersion);
            _httpClient.DefaultRequestHeaders.Add("indexes", settings.Index);
            _uri = new Uri($"{settings.ApiUrl}?api-version={settings.ApiVersion}");
            _providerUri = new Uri($"{settings.ProviderApiUrl}?api-version={settings.ApiVersion}");
            _larsUri = new Uri($"{settings.LARSApiUrl}?api-version={settings.ApiVersion}");
            _onspdUri = new Uri($"{settings.ONSPDApiUrl}?api-version={settings.ApiVersion}");
        }

        //public IEnumerable<IndexingResult> UploadBatch(
        //    IEnumerable<AzureSearchProviderModel> providers,
        //    IEnumerable<AzureSearchVenueModel> venues,
        //    IReadOnlyList<Document> documents,
        //    out int succeeded)
        //{
        //    try {
        //        succeeded = 0;
        //        if (documents.Any()) {

        //            IEnumerable<Course> courses = documents.Select(d => new Course()
        //            {
        //                id = d.GetPropertyValue<Guid>("id"),
        //                QualificationCourseTitle = d.GetPropertyValue<string>("QualificationCourseTitle"),
        //                LearnAimRef = d.GetPropertyValue<string>("LearnAimRef"),
        //                NotionalNVQLevelv2 = d.GetPropertyValue<string>("NotionalNVQLevelv2"),
        //                UpdatedDate = d.GetPropertyValue<DateTime?>("UpdatedDate"),
        //                ProviderUKPRN = int.Parse(d.GetPropertyValue<string>("ProviderUKPRN")),
        //                CourseRuns = d.GetPropertyValue<IEnumerable<CourseRun>>("CourseRuns")
        //            });

        //            _log.LogInformation("Creating batch of course data to index");

        //            // Courses run in classrooms have an associated venue
        //            IEnumerable<LINQComboClass> classroom =    from Course c in courses
        //                                                       from CourseRun r in c.CourseRuns ?? new List<CourseRun>()
        //                                                       from AzureSearchVenueModel v in venues.Where(x => r.VenueId == x.id)
        //                                                                                             .DefaultIfEmpty()
        //                                                       select new LINQComboClass() { Course = c, Run = r, Region = (string)null, Venue = v };
        //            _log.LogInformation($"{classroom.Count()} classroom courses (with VenueId and no regions)");

        //            // Courses run elsewhere have regions instead (online, work-based, ...)
        //            IEnumerable<LINQComboClass> nonclassroom = from Course c in courses
        //                                                       from CourseRun r in c.CourseRuns ?? new List<CourseRun>()
        //                                                       from string region in r.Regions?.DefaultIfEmpty() ?? new List<string>()
        //                                                       select new LINQComboClass() { Course = c, Run = r, Region = region, Venue = (AzureSearchVenueModel)null };
        //            _log.LogInformation($"{nonclassroom.Count()} other courses (with regions but no VenueId)");

        //            var batchdata = from LINQComboClass x in classroom.Union(nonclassroom)
        //                            join AzureSearchProviderModel p in providers
        //                            on x.Course?.ProviderUKPRN equals p.UnitedKingdomProviderReferenceNumber
        //                            where (x.Run?.RecordStatus != RecordStatus.Pending && (x.Venue != null || x.Region != null))
        //                            select new AzureSearchCourse()
        //                            {
        //                                id = x.Run?.id,
        //                                CourseId = x.Course?.id,
        //                                QualificationCourseTitle = x.Course?.QualificationCourseTitle,
        //                                LearnAimRef = x.Course?.LearnAimRef,
        //                                NotionalNVQLevelv2 = x.Course?.NotionalNVQLevelv2,
        //                                VenueName = x.Venue?.VENUE_NAME,
        //                                VenueAddress = string.Format("{0}{1}{2}{3}{4}",
        //                                               string.IsNullOrWhiteSpace(x.Venue?.ADDRESS_1) ? "" : x.Venue?.ADDRESS_1 + ", ",
        //                                               string.IsNullOrWhiteSpace(x.Venue?.ADDRESS_2) ? "" : x.Venue?.ADDRESS_2 + ", ",
        //                                               string.IsNullOrWhiteSpace(x.Venue?.TOWN) ? "" : x.Venue?.TOWN + ", ",
        //                                               string.IsNullOrWhiteSpace(x.Venue?.COUNTY) ? "" : x.Venue?.COUNTY + ", ",
        //                                               x.Venue?.POSTCODE),
        //                                VenueAttendancePattern = ((int)x.Run?.AttendancePattern).ToString(),
        //                                VenueLocation = GeographyPoint.Create(x.Venue?.Latitude ?? 0, x.Venue?.Longitude ?? 0),
        //                                ProviderName = p?.ProviderName,
        //                                Region = x.Region,
        //                                Status = (int?)x.Run?.RecordStatus,
        //                                UpdatedOn = x.Run?.UpdatedDate
        //                            };

        //            if (batchdata.Any()) {
        //                IndexBatch<AzureSearchCourse> batch = IndexBatch.MergeOrUpload(batchdata);

        //                _log.LogInformation("Merging docs to azure search index: course");
        //                Task<DocumentIndexResult> task = _adminIndex.Documents.IndexAsync(batch);
        //                task.Wait();
        //                succeeded = batchdata.Count();
        //            }
        //            _log.LogInformation($"*** Successfully merged {succeeded} docs into Azure search index: course");
        //        }

        //    } catch (IndexBatchException ex) {
        //        IEnumerable<IndexingResult> failed = ex.IndexingResults.Where(r => !r.Succeeded);
        //        _log.LogError(ex, string.Format("Failed to index some of the documents: {0}",
        //                                        string.Join(", ", failed)));
        //        //_log.LogError(ex.ToString());
        //        succeeded = ex.IndexingResults.Count(x => x.Succeeded);
        //        return failed;

        //    } catch (Exception e) {
        //        throw e;
        //    }

        //    // Return empty list of failed IndexingResults
        //    return new List<IndexingResult>();
        //}


        public FACSearchResult SearchCourses(SearchCriteriaStructure criteria)
        {
            Throw.IfNull(criteria, nameof(criteria));

            //_log.LogMethodEnter();

            try
            {
                _log.LogInformation("FAC search criteria.", criteria);
                _log.LogInformation("FAC search uri.", _uri.ToString());

                var sortBy = criteria.SortBy ?? CourseSearchSortBy.Relevance;

                var geoFilterRequired = criteria.Distance.GetValueOrDefault(0) > 0 && !string.IsNullOrWhiteSpace(criteria.TownOrPostcode);

                // lat/lng required if Distance filter is specified *or* sorting by Distance
                var getBaseCoords = geoFilterRequired || sortBy == CourseSearchSortBy.Distance;
                float? latitude = null;
                float? longitude = null;
                if (getBaseCoords)
                {
                    _log.LogInformation($"FAC getting lat/long for location {criteria.TownOrPostcode}");

                    SearchParameters parameters = new SearchParameters
                    {
                        Select = new[] { "pcds", "lat", "long" },
                        SearchMode = SearchMode.All,
                        Top = 1,
                        QueryType = QueryType.Full
                    };
                    DocumentSearchResult<dynamic> results = _onspdIndex.Documents.Search<dynamic>(criteria.TownOrPostcode, parameters);
                    latitude = (float?)results?.Results?.FirstOrDefault()?.Document?.lat;
                    longitude = (float?)results?.Results?.FirstOrDefault()?.Document?.@long;

                    if (!latitude.HasValue || !longitude.HasValue)
                    {
                        return null;
                    }
                }

                var filterClauses = new List<string>()
                {
                    "Status eq 1"  // only search live courses
                };

                if (criteria.StartDateFrom.HasValue)
                {
                    filterClauses.Add($"StartDate ge {criteria.StartDateFrom.Value:o}");
                }

                if (criteria.StartDateTo.HasValue)
                {
                    filterClauses.Add($"StartDate le {criteria.StartDateTo.Value:o}");
                }

                if (criteria.AttendancePatterns?.Any() ?? false)
                {
                    filterClauses.Add($"search.in(VenueAttendancePattern, '{string.Join("|", criteria.AttendancePatterns)}', '|')");
                }

                if (criteria.QualificationLevels?.Any() ?? false)
                {
                    filterClauses.Add($"search.in(NotionalNVQLevelv2, '{string.Join("|", criteria.QualificationLevels)}', '|')");
                }

                if (geoFilterRequired)
                {
                    filterClauses.Add($"geo.distance(VenueLocation, geography'POINT({longitude.Value} {latitude.Value})') le {criteria.Distance}");
                }

                var filter = string.Join(" and ", filterClauses);

                var orderBy = sortBy == CourseSearchSortBy.StartDateDescending ?
                    "StartDate desc" : sortBy == CourseSearchSortBy.StartDateAscending ?
                    "StartDate asc" : sortBy == CourseSearchSortBy.Distance ?
                    $"geo.distance(VenueLocation, geography'POINT({longitude.Value} {latitude.Value})')" :
                    "search.score() desc";

                // Create a search criteria object for azure search service
                IFACSearchCriteria facCriteria = new FACSearchCriteria()
                {
                    scoringProfile = string.IsNullOrWhiteSpace(_settings.RegionBoostScoringProfile) ? "region-boost" : _settings.RegionBoostScoringProfile,
                    search = $"{criteria.SubjectKeyword}*",
                    searchMode = "all",
                    filter = filter,
                    facets = new string[] { "NotionalNVQLevelv2", "VenueAttendancePattern", "ProviderName", "Region" },
                    top = criteria.TopResults ?? _settings.DefaultTop,
                    skip = (criteria.PageNo.HasValue && criteria.TopResults.HasValue && criteria.PageNo.Value > 0) ? ((criteria.PageNo.Value - 1) * criteria.TopResults.Value) : 0,
                    count = true,
                    orderby = orderBy
                };

                // Create json ready for posting
                JsonSerializerSettings settings = new JsonSerializerSettings {
                    //ContractResolver = new FACSearchCriteriaContractResolver()
                };
                settings.Converters.Add(new StringEnumConverter() { CamelCaseText = false });
                StringContent content = new StringContent(JsonConvert.SerializeObject(facCriteria, settings), Encoding.UTF8, "application/json");

                // Do the search
                _log.LogInformation("FAC search POST body", JsonConvert.SerializeObject(facCriteria, settings));
                Task<HttpResponseMessage> task = _httpClient.PostAsync(_uri, content);
                task.Wait();
                HttpResponseMessage response = task.Result;
                _log.LogInformation("FAC search service http response.", response);

                // Handle response and deserialize results
                if (response.IsSuccessStatusCode) {
                    var json = response.Content.ReadAsStringAsync().Result;

                    _log.LogInformation("FAC search service json response.", json);
                    settings = new JsonSerializerSettings {
                        ContractResolver = new FACSearchResultContractResolver()
                    };
                    settings.Converters.Add(new StringEnumConverter() { CamelCaseText = false });

                    FACSearchResult searchResult = JsonConvert.DeserializeObject<FACSearchResult>(json, settings);
                    if (getBaseCoords && latitude.HasValue && longitude.HasValue) {
                        foreach (FACSearchResultItem ri in searchResult.Value) {
                            if (ri.VenueLocation != null && ri?.VenueLocation["coordinates"][0] != 0 && ri?.VenueLocation["coordinates"][1] != 0)
                                ri.GeoSearchDistance = Math.Round(
                                    GeoHelper.DistanceTo(
                                        new GeoHelper.Coordinates() { Latitude = latitude.Value,
                                                                      Longitude = longitude.Value },
                                        new GeoHelper.Coordinates() { Latitude = ri?.VenueLocation["coordinates"][1],
                                                                      Longitude = ri?.VenueLocation["coordinates"][0] }),
                                2);
                        }
                    }
                    return searchResult;

                } else {
                    _log.LogWarning($"FAC unexpected response: {response.StatusCode}", response);
                    //return Result.Fail<IFACSearchResult>("FAC search service unsuccessfull http response.");
                    return null;
                }

            } catch (HttpRequestException hre) {
                _log.LogError("FAC search service http request error.", hre);
                //return Result.Fail<IFACSearchResult>("FAC search service http request error.");
                return null;

            } catch (Exception e) {
                _log.LogError("FAC search service unknown error.", e);
                //return Result.Fail<IFACSearchResult>("FAC search service unknown error.");
                return null;

            } finally {
                //_log.LogMethodExit();
            }
        }

        public ProviderSearchResult SearchProviders(ProviderSearchCriteriaStructure criteria)
        {
            Throw.IfNull(criteria, nameof(criteria));
            //_log.LogMethodEnter();

            try
            {
                _log.LogInformation("Provider search criteria.", criteria);
                _log.LogInformation("Provider search uri.", _uri.ToString());

                // Create filter string for indexed fields
                // Use a pipe char to delimit; default commas and spaces can't be used as may be in facet values
                List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
                list.Add(new KeyValuePair<string, string>("Town", string.Join("|", criteria.Town ?? new string[] { })));
                //list.Add(new KeyValuePair<string, string>("Region", string.Join("|", criteria.Region ?? new string[] { })));
                string filter = string.Join(" and ", list.Where(x => !string.IsNullOrWhiteSpace(x.Value))
                                                         .Select(x => "search.in(" + x.Key + ", '" + x.Value + "', '|')"));

                //// Index array fields are a little different
                //filter = (string.IsNullOrWhiteSpace(filter) ? "" : filter + " and ")
                //                + "Town/any(t: search.in(t, '" 
                //                + string.Join("|", criteria.Towns ?? new string[] { })
                //                + "', '|'))";

                // Create a search criteria object for azure search service
                ISearchCriteria providerCriteria = new ProviderSearchCriteria()
                {
                    search = $"{criteria.Keyword}*",
                    searchMode = "all",
                    top = criteria.TopResults ?? _settings.DefaultTop,
                    filter = filter,
                    facets = new string[] { "Town" }, //, "Region" },
                    count = true
                };

                // Create json ready for posting
                JsonSerializerSettings settings = new JsonSerializerSettings {
                    //ContractResolver = new ProviderSearchCriteriaContractResolver()
                };
                settings.Converters.Add(new StringEnumConverter() { CamelCaseText = false });
                StringContent content = new StringContent(JsonConvert.SerializeObject(providerCriteria, settings), Encoding.UTF8, "application/json");

                // Do the search
                _log.LogInformation("Provider search POST body", JsonConvert.SerializeObject(providerCriteria, settings));
                Task<HttpResponseMessage> task = _httpClient.PostAsync(_providerUri, content);
                task.Wait();
                HttpResponseMessage response = task.Result;
                _log.LogInformation("Provider search service http response.", response);

                // Handle response and deserialize results
                if (response.IsSuccessStatusCode) {
                    var json = response.Content.ReadAsStringAsync().Result;

                    _log.LogInformation("Provider search service json response.", json);
                    settings = new JsonSerializerSettings {
                        ContractResolver = new ProviderSearchResultContractResolver()
                    };
                    settings.Converters.Add(new StringEnumConverter() { CamelCaseText = false });

                    ProviderSearchResult searchResult = JsonConvert.DeserializeObject<ProviderSearchResult>(json, settings);
                    return searchResult;

                } else {
                    _log.LogWarning($"Provider search unexpected response: {response.StatusCode}", response);
                    //return Result.Fail<IProviderSearchResult>("Provider search service unsuccessfull http response.");
                    return null;
                }

            } catch (HttpRequestException hre) {
                _log.LogError("Provider search service http request error.", hre);
                //return Result.Fail<IProviderSearchResult>("Provider search service http request error.");
                return null;

            } catch (Exception e) {
                _log.LogError("Provider search service unknown error.", e);
                //return Result.Fail<IProviderSearchResult>("Provider search service unknown error.");
                return null;

            } finally {
                //_log.LogMethodExit();
            }

        }

        public LARSSearchResult SearchLARS(LARSSearchCriteriaStructure criteria)
        {
            Throw.IfNull(criteria, nameof(criteria));
            //_log.LogMethodEnter();

            try
            {
                _log.LogInformation("LARS search criteria.", criteria);
                _log.LogInformation("LARS search uri.", _uri.ToString());

                // Create filter string for indexed fields
                // Use a pipe char to delimit; default commas and spaces can't be used as may be in facet values
                List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
                list.Add(new KeyValuePair<string, string>("AwardOrgCode", string.Join("|", criteria.AwardOrgCode ?? new string[] { })));
                list.Add(new KeyValuePair<string, string>("NotionalNVQLevelv2", string.Join("|", criteria.NotionalNVQLevelv2 ?? new string[] { })));
                list.Add(new KeyValuePair<string, string>("SectorSubjectAreaTier1", string.Join("|", criteria.SectorSubjectAreaTier1 ?? new string[] { })));
                list.Add(new KeyValuePair<string, string>("SectorSubjectAreaTier2", string.Join("|", criteria.SectorSubjectAreaTier2 ?? new string[] { })));
                list.Add(new KeyValuePair<string, string>("AwardOrgAimRef", string.Join("|", criteria.AwardOrgAimRef ?? new string[] { })));
                string filter = string.Join(" and ", list.Where(x => !string.IsNullOrWhiteSpace(x.Value))
                                                         .Select(x => "search.in(" + x.Key + ", '" + x.Value + "', '|')"));

                //// Index array fields are a little different
                //filter = (string.IsNullOrWhiteSpace(filter) ? "" : filter + " and ")
                //                + "Town/any(t: search.in(t, '" 
                //                + string.Join("|", criteria.Towns ?? new string[] { })
                //                + "', '|'))";

                // Create a search criteria object for azure search service
                ISearchCriteria larsCriteria = new LARSSearchCriteria()
                {
                    search = $"{criteria.Keyword}*",
                    searchMode = "all",
                    top = criteria.TopResults ?? _settings.DefaultTop,
                    filter = filter,
                    facets = new string[] { "AwardOrgCode", "NotionalNVQLevelv2", "SectorSubjectAreaTier1", "SectorSubjectAreaTier2", "AwardOrgAimRef" },
                    count = true
                };

                // Create json ready for posting
                JsonSerializerSettings settings = new JsonSerializerSettings {
                    ContractResolver = new LARSSearchResultContractResolver()
                };
                settings.Converters.Add(new StringEnumConverter() { CamelCaseText = false });
                StringContent content = new StringContent(JsonConvert.SerializeObject(larsCriteria, settings), Encoding.UTF8, "application/json");

                // Do the search
                _log.LogInformation("LARS search POST body", JsonConvert.SerializeObject(larsCriteria, settings));
                Task<HttpResponseMessage> task = _httpClient.PostAsync(_larsUri, content);
                task.Wait();
                HttpResponseMessage response = task.Result;
                _log.LogInformation("LARS search service http response.", response);

                // Handle response and deserialize results
                if (response.IsSuccessStatusCode) {
                    var json = response.Content.ReadAsStringAsync().Result;

                    _log.LogInformation("LARS search service json response.", json);
                    settings = new JsonSerializerSettings {
                        ContractResolver = new LARSSearchResultContractResolver()
                    };
                    settings.Converters.Add(new StringEnumConverter() { CamelCaseText = false });

                    LARSSearchResult searchResult = JsonConvert.DeserializeObject<LARSSearchResult>(json, settings);
                    return searchResult;

                } else {
                    _log.LogWarning($"LARS search unexpected response: {response.StatusCode}", response);
                    //return Result.Fail<ILARSSearchResult>("LARS search service unsuccessfull http response.");
                    return null;
                }

            } catch (HttpRequestException hre) {
                _log.LogError("LARS search service http request error.", hre);
                //return Result.Fail<ILARSSearchResult>("LARS search service http request error.");
                return null;

            } catch (Exception e) {
                _log.LogError("LARS search service unknown error.", e);
                //return Result.Fail<ILARSSearchResult>("LARS search service unknown error.");
                return null;

            } finally {
                //_log.LogMethodExit();
            }
        }

        public PostcodeSearchResult SearchPostcode(PostcodeSearchCriteriaStructure criteria)
        {
            Throw.IfNull(criteria, nameof(criteria));
            //_log.LogMethodEnter();

            try
            {
                _log.LogInformation("Postcode search criteria.", criteria);
                _log.LogInformation("Postcode search uri.", _uri.ToString());

                // Create filter string for indexed fields
                // Use a pipe char to delimit; default commas and spaces can't be used as may be in facet values
                List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
                //list.Add(new KeyValuePair<string, string>("Town", string.Join("|", criteria.Town ?? new string[] { })));
                //string filter = string.Join(" and ", list.Where(x => !string.IsNullOrWhiteSpace(x.Value))
                //                                         .Select(x => "search.in(" + x.Key + ", '" + x.Value + "', '|')"));

                //// Index array fields are a little different
                //filter = (string.IsNullOrWhiteSpace(filter) ? "" : filter + " and ")
                //                + "Town/any(t: search.in(t, '" 
                //                + string.Join("|", criteria.Towns ?? new string[] { })
                //                + "', '|'))";

                // Create a search criteria object for azure search service
                ISearchCriteria postcodeCriteria = new PostcodeSearchCriteria()
                {
                    search = $"{criteria.Keyword}*",
                    searchMode = "all",
                    top = criteria.TopResults ?? _settings.DefaultTop,
                    filter = "", //filter,
                    facets = new string[] { },
                    count = true
                };

                // Create json ready for posting
                JsonSerializerSettings settings = new JsonSerializerSettings {
                    ContractResolver = new PostcodeSearchResultContractResolver()
                };
                settings.Converters.Add(new StringEnumConverter() { CamelCaseText = false });
                StringContent content = new StringContent(JsonConvert.SerializeObject(postcodeCriteria, settings), Encoding.UTF8, "application/json");

                // Do the search
                _log.LogInformation("Postcode search POST body", JsonConvert.SerializeObject(postcodeCriteria, settings));
                Task<HttpResponseMessage> task = _httpClient.PostAsync(_onspdUri, content);
                task.Wait();
                HttpResponseMessage response = task.Result;
                _log.LogInformation("Postcode search service http response.", response);

                // Handle response and deserialize results
                if (response.IsSuccessStatusCode) {
                    var json = response.Content.ReadAsStringAsync().Result;

                    _log.LogInformation("Postcode search service json response.", json);
                    settings = new JsonSerializerSettings {
                        ContractResolver = new PostcodeSearchResultContractResolver()
                    };
                    settings.Converters.Add(new StringEnumConverter() { CamelCaseText = false });

                    PostcodeSearchResult searchResult = JsonConvert.DeserializeObject<PostcodeSearchResult>(json, settings);
                    return searchResult;

                } else {
                    _log.LogWarning($"Postcode search unexpected response: {response.StatusCode}", response);
                    //return Result.Fail<IPostcodeSearchResult>("Postcode search service unsuccessfull http response.");
                    return null;
                }

            } catch (HttpRequestException hre) {
                _log.LogError("Postcode search service http request error.", hre);
                //return Result.Fail<IPostcodeSearchResult>("Postcode search service http request error.");
                return null;

            } catch (Exception e) {
                _log.LogError("Postcode search service unknown error.", e);
                //return Result.Fail<IPostcodeSearchResult>("Postcode search service unknown error.");
                return null;

            } finally {
                //_log.LogMethodExit();
            }

        }
    }
}
