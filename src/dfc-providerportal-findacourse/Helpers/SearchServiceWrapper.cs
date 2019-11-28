﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dfc.GeoCoordinate;
using Dfc.ProviderPortal.FindACourse.Interfaces;
using Dfc.ProviderPortal.FindACourse.Models;
using Dfc.ProviderPortal.FindACourse.Services;
using Dfc.ProviderPortal.Packages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dfc.ProviderPortal.FindACourse.Helpers
{
    public class SearchServiceWrapper
    {
        private class LINQComboClass
        {
            public Course Course { get; set; }
            public CourseRun Run { get; set; }
            public string Region { get; set; }
            public AzureSearchVenueModel Venue { get; set; }
        }

        private static readonly Regex _postcode = new Regex("^([A-PR-UWYZ0-9][A-HK-Y0-9][AEHMNPRTVXY0-9]?[ABEHMNPRVWXY0-9]? {1,2}[0-9][ABD-HJLN-UW-Z]{2}|GIR 0AA)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly ILogger _log;
        private readonly ISearchServiceSettings _settings;
        private readonly SearchServiceClient _queryService;
        private readonly SearchServiceClient _adminService;
        private readonly ISearchIndexClient _queryIndex;
        private readonly ISearchIndexClient _adminIndex;
        private readonly ISearchIndexClient _onspdIndex;
        private readonly HttpClient _httpClient;
        private readonly Uri _uri;
        private readonly Uri _providerUri;
        private readonly Uri _larsUri;
        private readonly Uri _onspdUri;

        public SearchServiceWrapper(
            ILogger log,
            ISearchServiceSettings settings)
        {
            Throw.IfNull(log, nameof(log));
            Throw.IfNull(settings, nameof(settings));

            _log = log;
            _settings = settings;

            _queryService = new SearchServiceClient(settings.SearchService, new SearchCredentials(settings.QueryKey));
            _adminService = new SearchServiceClient(settings.SearchService, new SearchCredentials(settings.AdminKey));
            _queryIndex = _queryService?.Indexes?.GetClient(settings.Index);
            _adminIndex = _adminService?.Indexes?.GetClient(settings.Index);
            _onspdIndex = _queryService?.Indexes?.GetClient(settings.onspdIndex);

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("api-key", settings.QueryKey);
            _httpClient.DefaultRequestHeaders.Add("api-version", settings.ApiVersion);
            _httpClient.DefaultRequestHeaders.Add("indexes", settings.Index);

            _uri = new Uri($"{settings.ApiUrl}?api-version={settings.ApiVersion}");
            _providerUri = new Uri($"{settings.ProviderApiUrl}?api-version={settings.ApiVersion}");
            _larsUri = new Uri($"{settings.LARSApiUrl}?api-version={settings.ApiVersion}");
            _onspdUri = new Uri($"{settings.ONSPDApiUrl}?api-version={settings.ApiVersion}");
        }

        public async Task<FACSearchResult> SearchCourses(SearchCriteriaStructure criteria)
        {
            Throw.IfNull(criteria, nameof(criteria));

            _log.LogInformation("FAC search criteria.", criteria);
            _log.LogInformation("FAC search uri.", _uri.ToString());

            var sortBy = criteria.SortBy ?? CourseSearchSortBy.Relevance;

            // Can only sort by distance if a postcode is specified
            if (sortBy == CourseSearchSortBy.Distance && string.IsNullOrWhiteSpace(criteria.Postcode))
            {
                throw new ProblemDetailsException(new ProblemDetails()
                {
                    Detail = "Postcode is required to sort by Distance.",
                    Status = 400,
                    Title = "PostcodeRequired"
                });
            }

            // Validate postcode
            if (!string.IsNullOrWhiteSpace(criteria.Postcode))
            {
                if (!_postcode.IsMatch(criteria.Postcode))
                {
                    throw new ProblemDetailsException(new ProblemDetails()
                    {
                        Detail = "Postcode is not valid.",
                        Status = 400,
                        Title = "InvalidPostcode"
                    });
                }
            }

            var geoFilterRequired = criteria.Distance.GetValueOrDefault(0) > 0 && !string.IsNullOrWhiteSpace(criteria.Postcode);

            // lat/lng required if Distance filter is specified *or* sorting by Distance
            var getBaseCoords = geoFilterRequired || sortBy == CourseSearchSortBy.Distance;
            float? latitude = null;
            float? longitude = null;
            if (getBaseCoords)
            {
                _log.LogInformation($"FAC getting lat/long for location {criteria.Postcode}");

                var coords = await TryGetCoordinatesForPostcode(criteria.Postcode);

                if (!coords.HasValue)
                {
                    throw new ProblemDetailsException(new ProblemDetails()
                    {
                        Detail = "Specified postcode cannot be found.",
                        Status = 400,
                        Title = "PostcodeNotFound"
                    });
                }

                latitude = coords.Value.lat;
                longitude = coords.Value.lng;
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

            if (!string.IsNullOrWhiteSpace(criteria.Town))
            {
                var townEscaped = Uri.EscapeDataString(criteria.Town);
                filterClauses.Add($"search.ismatch('{townEscaped}', 'VenueTown')");
            }

            if (criteria.StudyModes?.Any() ?? false)
            {
                filterClauses.Add($"search.in(VenueStudyMode, '{string.Join("|", criteria.StudyModes)}', '|')");
            }

            if (criteria.AttendanceModes?.Any() ?? false)
            {
                filterClauses.Add($"search.in(VenueAttendancePattern, '{string.Join("|", criteria.AttendanceModes)}', '|')");
            }

            var filter = string.Join(" and ", filterClauses);

            var orderBy = sortBy == CourseSearchSortBy.StartDateDescending ?
                "StartDate desc" : sortBy == CourseSearchSortBy.StartDateAscending ?
                "StartDate asc" : sortBy == CourseSearchSortBy.Distance ?
                $"geo.distance(VenueLocation, geography'POINT({longitude.Value} {latitude.Value})')" :
                "search.score() desc";

            var (limit, start) = ResolvePagingParams(criteria.Limit, criteria.Start);

            var scoringProfile = string.IsNullOrWhiteSpace(_settings.RegionBoostScoringProfile) ? "region-boost" : _settings.RegionBoostScoringProfile;

            // TODO Limit fields that the search works on?
            var results = await _queryIndex.Documents.SearchAsync<AzureSearchCourse>(
                $"{criteria.SubjectKeyword}*",
                new SearchParameters()
                {
                    Facets = new[] { "NotionalNVQLevelv2", "VenueAttendancePattern", "ProviderName", "Region" },
                    Filter = filter,
                    IncludeTotalResultCount = true,
                    ScoringProfile = scoringProfile,
                    SearchMode = SearchMode.All,
                    Top = limit,
                    Skip = start,
                    OrderBy = new[] { orderBy }
                });

            return new FACSearchResult()
            {
                Limit = limit,
                Start = start,
                Total = (int)results.Count.Value,
                Facets = results.Facets,
                Items = results.Results.Select(r => new FACSearchResultItem()
                {
                    Course = r.Document,
                    Distance = getBaseCoords && r.Document.VenueLocation != null ?
                        GeoHelper.DistanceTo(
                            new GeoHelper.Coordinates()
                            {
                                Latitude = latitude.Value,
                                Longitude = longitude.Value
                            },
                            new GeoHelper.Coordinates()
                            {
                                Latitude = r.Document.VenueLocation.Latitude,
                                Longitude = r.Document.VenueLocation.Longitude
                            }) :
                        (double?)null,
                    Score = r.Score
                })
            };
        }

        public async Task<ProviderSearchResult> SearchProviders(ProviderSearchCriteriaStructure criteria)
        {
            Throw.IfNull(criteria, nameof(criteria));
            //_log.LogMethodEnter();

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
            var response = await _httpClient.PostAsync(_providerUri, content);
            _log.LogInformation("Provider search service http response.", response);

            // Handle response and deserialize results
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            _log.LogInformation("Provider search service json response.", json);
            settings = new JsonSerializerSettings
            {
                ContractResolver = new ProviderSearchResultContractResolver()
            };
            settings.Converters.Add(new StringEnumConverter() { CamelCaseText = false });

            ProviderSearchResult searchResult = JsonConvert.DeserializeObject<ProviderSearchResult>(json, settings);
            return searchResult;
        }

        public async Task<LARSSearchResult> SearchLARS(LARSSearchCriteriaStructure criteria)
        {
            Throw.IfNull(criteria, nameof(criteria));
            //_log.LogMethodEnter();

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
            var response = await _httpClient.PostAsync(_larsUri, content);
            _log.LogInformation("LARS search service http response.", response);

            // Handle response and deserialize results
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            _log.LogInformation("LARS search service json response.", json);
            settings = new JsonSerializerSettings
            {
                ContractResolver = new LARSSearchResultContractResolver()
            };
            settings.Converters.Add(new StringEnumConverter() { CamelCaseText = false });

            LARSSearchResult searchResult = JsonConvert.DeserializeObject<LARSSearchResult>(json, settings);
            return searchResult;
        }

        public async Task<PostcodeSearchResult> SearchPostcode(PostcodeSearchCriteriaStructure criteria)
        {
            Throw.IfNull(criteria, nameof(criteria));
            //_log.LogMethodEnter();
            
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
            var response = await _httpClient.PostAsync(_onspdUri, content);
            _log.LogInformation("Postcode search service http response.", response);

            // Handle response and deserialize results
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            _log.LogInformation("Postcode search service json response.", json);
            settings = new JsonSerializerSettings
            {
                ContractResolver = new PostcodeSearchResultContractResolver()
            };
            settings.Converters.Add(new StringEnumConverter() { CamelCaseText = false });

            PostcodeSearchResult searchResult = JsonConvert.DeserializeObject<PostcodeSearchResult>(json, settings);
            return searchResult;
        }

        private (int limit, int start) ResolvePagingParams(int? limit, int? start)
        {
            if (limit.HasValue && limit.Value <= 0)
            {
                throw new ProblemDetailsException(
                    new ProblemDetails()
                    {
                        Detail = "limit parameter is invalid.",
                        Status = 400,
                        Title = "InvalidPagingParameters"
                    });
            }

            if (start.HasValue && start.Value < 0)
            {
                throw new ProblemDetailsException(
                    new ProblemDetails()
                    {
                        Detail = "start parameter is invalid.",
                        Status = 400,
                        Title = "InvalidPagingParameters"
                    });
            }

            var top = limit ?? _settings.DefaultTop;
            var skip = start ?? 0;

            return (top, skip);
        }

        private async Task<(float lat, float lng)?> TryGetCoordinatesForPostcode(string postcode)
        {
            var parameters = new SearchParameters
            {
                SearchFields = new[] { "pcds" },
                Select = new[] { "pcds", "lat", "long" },
                SearchMode = SearchMode.All,
                Top = 1,
                QueryType = QueryType.Full
            };
            var results = await _onspdIndex.Documents.SearchAsync<dynamic>(postcode, parameters);

            if (results.Results.Count > 0)
            {
                return ((float)results.Results.First().Document.lat, (float)results.Results.First().Document.@long);
            }
            else
            {
                return null;
            }
        }
    }
}
